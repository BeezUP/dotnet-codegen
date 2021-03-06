﻿using Microsoft.OpenApi;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using Microsoft.OpenApi.Services;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CodegenUP.Schemas
{
    public class OpenApiSchemaLoader : ISchemaLoader
    {
        static readonly HttpClient httpClient = new HttpClient { };

        public async Task<JToken> LoadSchemaAsync(IEnumerable<string> documentUris, string? authorization)
        {
            var docs = documentUris.ToArray();
            if (docs.Length != 1)
                throw new ArgumentException($"Cannot load multiple OpenApiSchema using this loader [{nameof(OpenApiSchemaLoader)}]", nameof(documentUris));
            var documentUri = docs[0];

            var settings = new OpenApiReaderSettings()
            {
                //ReferenceResolution = ReferenceResolutionSetting.ResolveAllReferences,
            };

            using (var stream = await httpClient.GetStreamAsync(documentUri))
            {
                var openApiDocument = new OpenApiStreamReader(settings).Read(stream, out var diagnostic);

                var resolver = new OpenApiReferenceResolver(openApiDocument);
                var walker = new OpenApiWalker(resolver);
                walker.Walk(openApiDocument);
                foreach (var item in resolver.Errors)
                {
                    diagnostic.Errors.Add(item);
                }

                if (diagnostic.Errors.Count != 0)
                {
                    var message = string.Join(Environment.NewLine, diagnostic.Errors);
                    throw new InvalidOperationException(message);
                }

                var apiV3Json = openApiDocument.Serialize(OpenApiSpecVersion.OpenApi3_0, OpenApiFormat.Json);
                return JToken.Parse(apiV3Json);
            }
        }

        /// <summary>
        /// This class is used to walk an OpenApiDocument and convert unresolved references to references to populated objects
        /// Taken from here : https://github.com/Microsoft/OpenAPI.NET/blob/master/src/Microsoft.OpenApi.Readers/Services/OpenApiReferenceResolver.cs
        /// </summary>
        internal class OpenApiReferenceResolver : OpenApiVisitorBase
        {
            private readonly OpenApiDocument _currentDocument;
            private readonly bool _resolveRemoteReferences;
            private readonly List<OpenApiError> _errors = new List<OpenApiError>();

            public OpenApiReferenceResolver(OpenApiDocument currentDocument, bool resolveRemoteReferences = true)
            {
                _currentDocument = currentDocument;
                _resolveRemoteReferences = resolveRemoteReferences;
            }

            public IEnumerable<OpenApiError> Errors
            {
                get
                {
                    return _errors;
                }
            }

            public override void Visit(OpenApiDocument doc)
            {
                if (doc.Tags != null)
                {
                    ResolveTags(doc.Tags);
                }
            }

            public override void Visit(OpenApiComponents components)
            {
                ResolveMap(components.Parameters);
                ResolveMap(components.RequestBodies);
                ResolveMap(components.Responses);
                ResolveMap(components.Links);
                ResolveMap(components.Callbacks);
                ResolveMap(components.Examples);
                ResolveMap(components.Schemas);
                ResolveMap(components.SecuritySchemes);
            }

            public override void Visit(IDictionary<string, OpenApiCallback> callbacks)
            {
                ResolveMap(callbacks);
            }

            /// <summary>
            /// Resolve all references used in an operation
            /// </summary>
            public override void Visit(OpenApiOperation operation)
            {
                ResolveObject(operation.RequestBody, r => operation.RequestBody = r);
                ResolveList(operation.Parameters);

                if (operation.Tags != null)
                {
                    ResolveTags(operation.Tags);
                }
            }

            /// <summary>
            /// Resolve all references using in mediaType object
            /// </summary>
            /// <param name="mediaType"></param>
            public override void Visit(OpenApiMediaType mediaType)
            {
                ResolveObject(mediaType.Schema, r => mediaType.Schema = r);
            }

            /// <summary>
            /// Resolve all references to examples
            /// </summary>
            /// <param name="examples"></param>
            public override void Visit(IDictionary<string, OpenApiExample> examples)
            {
                ResolveMap(examples);
            }

            /// <summary>
            /// Resolve all references to responses
            /// </summary>
            public override void Visit(OpenApiResponses response)
            {
                ResolveMap(response);
            }

            /// <summary>
            /// Resolve all references to SecuritySchemes
            /// </summary>
            public override void Visit(OpenApiSecurityRequirement securityRequirement)
            {
                foreach (var scheme in securityRequirement.Keys.ToList())
                {
                    ResolveObject(scheme, (resolvedScheme) =>
                    {
                        if (resolvedScheme != null)
                        {
                            // If scheme was unresolved
                            // copy Scopes and remove old unresolved scheme
                            var scopes = securityRequirement[scheme];
                            securityRequirement.Remove(scheme);
                            securityRequirement.Add(resolvedScheme, scopes);
                        }
                    });
                }
            }

            /// <summary>
            /// Resolve all references to parameters
            /// </summary>
            public override void Visit(IList<OpenApiParameter> parameters)
            {
                ResolveList(parameters);
            }

            /// <summary>
            /// Resolve all references to links
            /// </summary>
            public override void Visit(IDictionary<string, OpenApiLink> links)
            {
                ResolveMap(links);
            }

            /// <summary>
            /// Resolve all references used in a schema
            /// </summary>
            public override void Visit(OpenApiSchema schema)
            {
                ResolveObject(schema.Items, r => schema.Items = r);
                ResolveList(schema.OneOf);
                ResolveList(schema.AllOf);
                ResolveList(schema.AnyOf);
                ResolveMap(schema.Properties);
                ResolveObject(schema.AdditionalProperties, r => schema.AdditionalProperties = r);
            }

            /// <summary>
            /// Replace references to tags with either tag objects declared in components, or inline tag object
            /// </summary>
            private void ResolveTags(IList<OpenApiTag> tags)
            {
                for (int i = 0; i < tags.Count; i++)
                {
                    var tag = tags[i];
                    if (IsUnresolvedReference(tag))
                    {
                        var resolvedTag = ResolveReference<OpenApiTag>(tag.Reference);

                        if (resolvedTag == null)
                        {
                            resolvedTag = new OpenApiTag()
                            {
                                Name = tag.Reference.Id
                            };
                        }
                        tags[i] = resolvedTag;
                    }
                }
            }

            private void ResolveObject<T>(T entity, Action<T> assign) where T : class, IOpenApiReferenceable, new()
            {
                if (entity == null) return;

                if (IsUnresolvedReference(entity) && entity.Reference != null)
                {
                    var resolved = ResolveReference<T>(entity.Reference);
                    if (resolved != null)
                    {
                        assign(resolved);
                    }
                }
            }

            private void ResolveList<T>(IList<T> list) where T : class, IOpenApiReferenceable, new()
            {
                if (list == null) return;

                for (int i = 0; i < list.Count; i++)
                {
                    var entity = list[i];
                    if (IsUnresolvedReference(entity))
                    {
                        if (entity.Reference == null) continue;
                        var resolved = ResolveReference<T>(entity.Reference);
                        if (resolved == null) continue;
                        list[i] = resolved;
                    }
                }
            }

            private void ResolveMap<T>(IDictionary<string, T> map) where T : class, IOpenApiReferenceable, new()
            {
                if (map == null) return;

                foreach (var key in map.Keys.ToList())
                {
                    var entity = map[key];
                    if (IsUnresolvedReference(entity))
                    {
                        if (entity.Reference == null) continue;
                        var resolved = ResolveReference<T>(entity.Reference);
                        if (resolved == null) continue;
                        map[key] = resolved;
                    }
                }
            }

            private T? ResolveReference<T>(OpenApiReference reference) where T : class, IOpenApiReferenceable, new()
            {
                if (string.IsNullOrEmpty(reference.ExternalResource))
                {
                    try
                    {
                        return _currentDocument.ResolveReference(reference) as T;
                    }
                    catch (OpenApiException ex)
                    {
                        _errors.Add(new OpenApiError(ex));
                        return null;
                    }
                }
                else if (_resolveRemoteReferences)
                {
                    // TODO: Resolve Remote reference (Targeted for 1.1 release)
                    return new T()
                    {
                        UnresolvedReference = true,
                        Reference = reference
                    };
                }
                else
                {
                    // Leave as unresolved reference
                    return new T()
                    {
                        UnresolvedReference = true,
                        Reference = reference
                    };
                }
            }

            private bool IsUnresolvedReference(IOpenApiReferenceable possibleReference)
            {
                return (possibleReference != null && possibleReference.UnresolvedReference);
            }
        }
    }
}
