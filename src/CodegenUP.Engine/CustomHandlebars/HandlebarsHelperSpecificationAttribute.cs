using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using YamlDotNet.Serialization;

namespace Dotnet.CodeGen.CustomHandlebars
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class HandlebarsHelperSpecificationAttribute : Attribute
    {
        /// <param name="jsonOrFile">plain json document or path to a file containing json or yaml</param>
        /// <param name="template">handlebars template</param>
        /// <param name="expectedOutput">expected output</param>
        public HandlebarsHelperSpecificationAttribute(string jsonOrFile, string template, string expectedOutput)
        {
            JsonOrFile = jsonOrFile;
            Template = template;
            ExpectedOutput = expectedOutput;
        }

        /// <summary>
        /// 
        /// </summary>
        private string JsonOrFile { get; }
        public string Template { get; }
        public string ExpectedOutput { get; }

        static readonly Deserializer YamlDeserializer = new Deserializer();

        public string GetJsonDocument()
        {
            try
            {
                var path = Path.GetFullPath(JsonOrFile);

                object? yamlObject;
                using (var sr = new StreamReader(File.OpenRead(path)))
                {
                    yamlObject = YamlDeserializer.Deserialize(sr);
                }

                var json = JsonConvert.SerializeObject(yamlObject);
                return json;
            }
            catch (Exception) { }

            return JsonOrFile;
        }
    }
}
