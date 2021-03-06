﻿namespace CodegenUP.CustomHandlebars
{
#if DEBUG
    public static class GlobalSpecs
    {
        public const string SWAGGER_SAMPLE = @"
{
  ""swagger"": ""2.0"",
    ""info"": {
        ""title"": ""Marketplace Gateway API - Feeds"",
        ""description"": ""This API has to be implemented by the marketplace to be compatible with BeezUP."",
        ""version"": ""1.0"",
        ""x-logo"": {
            ""url"": ""https://avatars0.githubusercontent.com/u/25665430"",
            ""backgroundColor"": ""#FFFFFF""
        },
        ""contact"": {
            ""email"": ""help@beezup.com""
        },
        ""license"": {
            ""name"": ""BeezUP"",
            ""url"": ""http://www.beezup.com""
        }
    },
  ""host"": ""mkpapigateway.beezup.com"",
  ""basePath"": ""/marketplace/feeds/v1"",
  ""x-beezup-ops"": {
    ""codeGenType"": ""webapi"",
    ""repo"": ""BeezUP"",
    ""product"": ""BeezUP"",
    ""group"": ""adpt-feedpub"",
    ""applicationShortName"": ""MKP.ADPT.Feeds.WS"",
    ""appRoot"": ""BZ.MKP.ADPT.Feeds.WebService"",
    ""namespace"": ""BZ.MKP.ADPT.Feeds.WebService"",
    ""apiName"": ""MkpGatewayOffersV1"",
    ""useBeezUPFrameworkVersion2"": ""true"",
    ""comment"": ""First document\n"",
    ""using"": [
      ""BeezUP2.Framework.Messaging"",
      ""BeezUP2.Framework.Business""
    ],
    ""packages"": [
      ""BeezUPWebApi""
    ]
  },
  ""x-basePath"": ""/marketplace/feeds/v1"",
  ""schemes"": [
    ""https""
  ],
  ""consumes"": [
    ""application/json""
  ],
  ""produces"": [
    ""application/json""
  ],
  ""tags"": [
    {
      ""name"": ""Publication""
    }
  ],
  ""parameters"": {
    ""marketplaceBusinessCodeParameter"": {
      ""name"": ""marketplaceBusinessCode"",
      ""in"": ""path"",
      ""required"": ""true"",
      ""type"": ""string""
    },
    ""accountIdParameter"": {
      ""name"": ""accountId"",
      ""in"": ""path"",
      ""required"": ""true"",
      ""type"": ""integer""
    },
    ""publicationIdParameter"": {
      ""name"": ""publicationId"",
      ""in"": ""path"",
      ""required"": ""true"",
      ""type"": ""string"",
      ""format"": ""guid""
    },
    ""feedTypeParameter"": {
      ""name"": ""feedType"",
      ""in"": ""path"",
      ""required"": ""true"",
      ""type"": ""string"",
      ""format"": ""FeedType""
    },
    ""credentialParameter"": {
      ""name"": ""x-BeezUP-Credential"",
      ""in"": ""header"",
      ""required"": ""true"",
      ""type"": ""string"",
      ""description"": ""It's the merchant configuration related to the marketplace, serialiazed in json in base64.""
    }
  },
  ""paths"": {
    ""/{marketplaceBusinessCode}/ping"": {
      ""post"": {
        ""tags"": [
          ""Publication""
        ],
        ""summary"": ""Ping the status of your feed system"",
        ""operationId"": ""PingGatewayFeedApi"",
        ""parameters"": [
          {
            ""$ref"": ""#/parameters/marketplaceBusinessCodeParameter""
          }
        ],
        ""responses"": {
          ""200"": {
            ""description"": ""The status of the API is up!""
          },
          ""503"": {
            ""description"": ""The status of your API is down...""
          },
          ""default"": {
            ""$ref"": ""#/responses/GeneralError""
          }
        }
      },
      ""get"": {
        ""tags"": [
          ""Publication""
        ],
        ""summary"": ""Fake the status of your feed system"",
        ""operationId"": ""Fake"",
        ""parameters"": [
          {
            ""$ref"": ""#/parameters/marketplaceBusinessCodeParameter""
          }
        ],
        ""responses"": {
          ""200"": {
            ""description"": ""The status of the API is up!""
          }
        }
      }
    },
    ""/{marketplaceBusinessCode}/{accountId}/publications/{publicationId}/start"": {
      ""post"": {
        ""tags"": [
          ""Publication""
        ],
        ""summary"": ""Start the feed publication"",
        ""operationId"": ""StartFeedPublication"",
        ""parameters"": [
          {
            ""$ref"": ""#/parameters/marketplaceBusinessCodeParameter""
          },
          {
            ""$ref"": ""#/parameters/accountIdParameter""
          },
          {
            ""$ref"": ""#/parameters/publicationIdParameter""
          },
          {
            ""$ref"": ""#/parameters/credentialParameter""
          },
          {
            ""name"": ""request"",
            ""in"": ""body"",
            ""required"": ""true"",
            ""schema"": {
              ""$ref"": ""#/definitions/startFeedPublicationRequest""
            }
          }
        ],
        ""responses"": {
          ""204"": {
            ""description"": ""The merchant feeds changes have been saved!""
          },
          ""default"": {
            ""$ref"": ""#/responses/GeneralError""
          }
        }
      }
    },
    ""/{marketplaceBusinessCode}/{accountId}/publications/{publicationId}/complete"": {
      ""post"": {
        ""tags"": [
          ""Publication""
        ],
        ""summary"": ""Complete the feed publication"",
        ""operationId"": ""CompleteFeedPublication"",
        ""parameters"": [
          {
            ""$ref"": ""#/parameters/marketplaceBusinessCodeParameter""
          },
          {
            ""$ref"": ""#/parameters/accountIdParameter""
          },
          {
            ""$ref"": ""#/parameters/publicationIdParameter""
          },
          {
            ""$ref"": ""#/parameters/credentialParameter""
          },
          {
            ""name"": ""request"",
            ""in"": ""body"",
            ""required"": ""true"",
            ""schema"": {
              ""$ref"": ""#/definitions/completeFeedPublicationRequest""
            }
          }
        ],
        ""responses"": {
          ""204"": {
            ""description"": ""The merchant feeds changes have been saved!""
          },
          ""default"": {
            ""$ref"": ""#/responses/GeneralError""
          }
        }
      }
    },
    ""/{marketplaceBusinessCode}/{accountId}/publications/{publicationId}/events"": {
      ""post"": {
        ""tags"": [
          ""Publication""
        ],
        ""summary"": ""Push a batch of merchant marketplace feeds events"",
        ""operationId"": ""PushMerchantFeedEvents"",
        ""parameters"": [
          {
            ""$ref"": ""#/parameters/marketplaceBusinessCodeParameter""
          },
          {
            ""$ref"": ""#/parameters/accountIdParameter""
          },
          {
            ""$ref"": ""#/parameters/publicationIdParameter""
          },
          {
            ""$ref"": ""#/parameters/credentialParameter""
          },
          {
            ""name"": ""request"",
            ""in"": ""body"",
            ""schema"": {
              ""$ref"": ""#/definitions/feedEvents""
            }
          }
        ],
        ""responses"": {
          ""202"": {
            ""description"": ""The merchant feeds changes have been saved!""
          },
          ""default"": {
            ""$ref"": ""#/responses/GeneralError""
          }
        }
      }
    },
    ""/{marketplaceBusinessCode}/{accountId}/publications/{publicationId}/status"": {
      ""get"": {
        ""tags"": [
          ""Publication""
        ],
        ""x-tagGroups"": ""Publication"",
        ""operationId"": ""CheckPublicationStatus"",
        ""summary"": ""Check the status of the publication"",
        ""parameters"": [
          {
            ""$ref"": ""#/parameters/marketplaceBusinessCodeParameter""
          },
          {
            ""$ref"": ""#/parameters/accountIdParameter""
          },
          {
            ""$ref"": ""#/parameters/publicationIdParameter""
          },
          {
            ""$ref"": ""#/parameters/credentialParameter""
          }
        ],
        ""responses"": {
          ""200"": {
            ""description"": ""Publication status retrieved"",
            ""schema"": {
              ""$ref"": ""#/definitions/feedPublicationStatusResponse""
            }
          },
          ""409"": {
            ""description"": ""Publication status not yet available""
          }
        }
      }
    },
    ""/{marketplaceBusinessCode}/{accountId}/publications/{publicationId}/errorReporting"": {
      ""get"": {
        ""tags"": [
          ""Publication""
        ],
        ""x-tagGroups"": ""Publication"",
        ""operationId"": ""GetPublicationErrorReporting"",
        ""summary"": ""Get the error reporting of the publication"",
        ""parameters"": [
          {
            ""$ref"": ""#/parameters/marketplaceBusinessCodeParameter""
          },
          {
            ""$ref"": ""#/parameters/accountIdParameter""
          },
          {
            ""$ref"": ""#/parameters/publicationIdParameter""
          },
          {
            ""$ref"": ""#/parameters/credentialParameter""
          }
        ],
        ""responses"": {
          ""200"": {
            ""description"": ""The error reporting is retrieved"",
            ""schema"": {
              ""$ref"": ""#/definitions/feedPublicationErrorReportingResponse""
            }
          },
          ""409"": {
            ""description"": ""Error reporting not yet available""
          }
        }
      }
    }
  },
  ""definitions"": {
    ""startFeedPublicationRequest"": {
      ""type"": ""object"",
      ""required"": [
        ""feedType""
      ],
      ""properties"": {
        ""feedType"": {
          ""$ref"": ""#/definitions/feedType""
        }
      }
    },
    ""completeFeedPublicationRequest"": {
      ""type"": ""object"",
      ""required"": [
        ""processingStatus""
      ],
      ""properties"": {
        ""processingStatus"": {
          ""$ref"": ""#/definitions/processingStatus""
        },
        ""rollbarId"": {
          ""type"": ""string""
        },
        ""errorMsg"": {
          ""type"": ""string""
        }
      }
    },
    ""feedEvents"": {
      ""type"": ""object"",
      ""properties"": {
        ""events"": {
          ""type"": ""array"",
          ""items"": {
            ""$ref"": ""#/definitions/feedEvent""
          }
        }
      }
    },
    ""feedPublicationStatusResponse"": {
      ""description"": ""Publication Status Response"",
      ""type"": ""object"",
      ""required"": [
        ""status""
      ],
      ""properties"": {
        ""status"": {
          ""$ref"": ""#/definitions/feedPublicationStatus""
        }
      }
    },
    ""feedPublicationErrorReportingResponse"": {
      ""description"": ""Publication Error Reporting Response"",
      ""type"": ""object"",
      ""properties"": {
        ""errors"": {
          ""type"": ""array"",
          ""items"": {
            ""$ref"": ""#/definitions/feedPublicationError""
          }
        },
        ""warnings"": {
          ""type"": ""array"",
          ""items"": {
            ""$ref"": ""#/definitions/feedPublicationError""
          }
        }
      }
    },
    ""BeezUP.Common.ErrorResponseMessage"": {
      ""type"": ""object"",
      ""required"": [
        ""errors""
      ],
      ""properties"": {
        ""errors"": {
          ""type"": ""array"",
          ""uniqueItems"": ""false"",
          ""items"": {
            ""$ref"": ""#/definitions/BeezUP.Common.UserErrorMessage""
          }
        }
      }
    },
    ""BeezUP.Common.PageSize"": {
      ""description"": ""Indicate the item count per page"",
      ""type"": ""integer"",
      ""format"": ""int32"",
      ""default"": ""100"",
      ""minimum"": ""25"",
      ""maximum"": ""100"",
      ""example"": ""100""
    },
    ""BeezUP.Common.PageNumber"": {
      ""description"": ""Indicates the page number"",
      ""format"": ""int32"",
      ""type"": ""integer"",
      ""example"": ""1"",
      ""minimum"": ""1"",
      ""default"": ""1""
    },
    ""BeezUP.Common.AdditionalProductFilters"": {
      ""type"": ""object"",
      ""description"": ""Describe a filter on a product's column.\nThe key is the column identifier of your catalog or a custom column.\n"",
      ""additionalProperties"": {
        ""$ref"": ""#/definitions/BeezUP.Common.AdditionalProductFiltersValue""
      }
    },
    ""BeezUP.Common.ProductColumnFilterOperatorName"": {
      ""type"": ""string"",
      ""description"": ""Indicate the operator you want to make on the columnId"",
      ""x-lov"": ""ProductColumnFilterOperatorName""
    },
    ""BeezUP.Common.AdditionalProductFiltersValue"": {
      ""type"": ""object"",
      ""properties"": {
        ""operatorName"": {
          ""$ref"": ""#/definitions/BeezUP.Common.ProductColumnFilterOperatorName""
        },
        ""values"": {
          ""description"": ""Must be null if the operator is \""IsNull\"" or \""IsNotNull\"".\nCan contains multiple value in case of \""InList\"" operator. Otherwise a single value is expected.\n"",
          ""type"": ""array"",
          ""items"": {
            ""type"": ""string""
          },
          ""example"": [
            ""My value""
          ]
        }
      },
      ""example"": {
        ""672644c7-5bd0-4e25-88c1-1f732bda5e4c"": {
          ""operatorName"": ""GreaterTo"",
          ""values"": [
            ""100""
          ]
        }
      }
    },
    ""BeezUP.Common.ChannelId"": {
      ""type"": ""string"",
      ""format"": ""guid"",
      ""description"": ""The channel identifier"",
      ""example"": ""2dc136a7-0d3d-4cc9-a825-a28a42c53e28""
    },
    ""BeezUP.Common.ChannelName"": {
      ""type"": ""string"",
      ""description"": ""The channel name"",
      ""example"": ""Amazon FRA""
    },
    ""BeezUP.Common.HttpUrl"": {
      ""description"": ""The URL <a href=\""https://en.wikipedia.org/wiki/URL\"">https://en.wikipedia.org/wiki/URL</a>"",
      ""type"": ""string"",
      ""pattern"": ""^(https?:\\/\\/)?([\\da-z\\.-]+)\\.([a-z\\.]{2,6})([\\/\\w \\.-]*)*\\/?$"",
      ""example"": ""http://www.mydomain.com""
    },
    ""BeezUP.Common.PaginationResultLinks"": {
      ""description"": ""The navigation links 'first', 'last', 'next', 'previous'"",
      ""type"": ""object"",
      ""required"": [
        ""first"",
        ""last""
      ],
      ""properties"": {
        ""first"": {
          ""$ref"": ""#/definitions/BeezUP.Common.Link3""
        },
        ""last"": {
          ""$ref"": ""#/definitions/BeezUP.Common.Link3""
        },
        ""previous"": {
          ""$ref"": ""#/definitions/BeezUP.Common.Link3""
        },
        ""next"": {
          ""$ref"": ""#/definitions/BeezUP.Common.Link3""
        }
      }
    },
    ""BeezUP.Common.Link3"": {
      ""type"": ""object"",
      ""required"": [
        ""href""
      ],
      ""properties"": {
        ""label"": {
          ""type"": ""string"",
          ""description"": ""The label corresponding to the link. This label is automatically translated based on the Accept-Language http header."",
          ""example"": ""The translated label""
        },
        ""docUrl"": {
          ""$ref"": ""#/definitions/BeezUP.Common.DocUrl""
        },
        ""description"": {
          ""type"": ""string"",
          ""description"": ""The description of the link"",
          ""example"": ""This is a description link""
        },
        ""href"": {
          ""$ref"": ""#/definitions/BeezUP.Common.Href""
        },
        ""operationId"": {
          ""$ref"": ""#/definitions/BeezUP.Common.OperationId""
        },
        ""method"": {
          ""$ref"": ""#/definitions/BeezUP.Common.HttpMethod""
        },
        ""parameters"": {
          ""type"": ""object"",
          ""additionalProperties"": {
            ""$ref"": ""#/definitions/BeezUP.Common.LinkParameter3""
          }
        },
        ""urlTemplated"": {
          ""type"": ""boolean"",
          ""description"": ""indicates whether the href is templated or not""
        },
        ""allRequiredParamsProvided"": {
          ""type"": ""boolean"",
          ""description"": ""indicates whether all required params have been provided""
        },
        ""allOptionalParamsProvided"": {
          ""type"": ""boolean"",
          ""description"": ""indicates whether all optionals params have been provided""
        },
        ""info"": {
          ""$ref"": ""#/definitions/BeezUP.Common.InfoSummaries""
        }
      }
    },
    ""BeezUP.Common.DocUrl"": {
      ""type"": ""string"",
      ""format"": ""uri"",
      ""description"": ""The documentation related to this operation."",
      ""example"": ""https://api-docs.beezup.com/#operation/EnableChannelCatalog""
    },
    ""BeezUP.Common.Href"": {
      ""type"": ""string"",
      ""example"": ""/v2/marketplaces/orders/{marketplaceTechnicalCode}/{accountId}/{beezUPOrderId}"",
      ""description"": ""Indicate the relative uri for this link""
    },
    ""BeezUP.Common.OperationId"": {
      ""type"": ""string"",
      ""description"": ""The operationId to call."",
      ""example"": ""GetOrder""
    },
    ""BeezUP.Common.HttpMethod"": {
      ""type"": ""string"",
      ""enum"": [
        ""GET"",
        ""POST"",
        ""PATCH"",
        ""DELETE"",
        ""PUT"",
        ""HEAD""
      ],
      ""default"": ""GET"",
      ""example"": ""GET"",
      ""description"": ""Indicate the http method to use on this link""
    },
    ""BeezUP.Common.LinkParameter3"": {
      ""type"": ""object"",
      ""required"": [
        ""in""
      ],
      ""properties"": {
        ""label"": {
          ""type"": ""string"",
          ""description"": ""The label corresponding to the link parameter. This label is automatically translated based on the Accept-Language http header."",
          ""example"": ""The translated label""
        },
        ""value"": {
          ""type"": ""object"",
          ""description"": ""The value of the parameter. It can be an integer a string or an object."",
          ""example"": ""1234""
        },
        ""required"": {
          ""type"": ""boolean"",
          ""default"": ""false"",
          ""example"": ""true""
        },
        ""in"": {
          ""$ref"": ""#/definitions/BeezUP.Common.ParameterIn""
        },
        ""type"": {
          ""$ref"": ""#/definitions/BeezUP.Common.ParameterType""
        },
        ""lovLink"": {
          ""$ref"": ""#/definitions/BeezUP.Common.LOVLink3""
        },
        ""lovRequired"": {
          ""type"": ""boolean"",
          ""description"": ""If true, you MUST indicate a value from the list of values otherwise it's a freetext"",
          ""example"": ""true""
        },
        ""description"": {
          ""type"": ""string"",
          ""description"": ""description of the parameter"",
          ""example"": ""the store identifier""
        },
        ""schema"": {
          ""type"": ""string"",
          ""description"": ""schema of the parameter"",
          ""example"": ""orderListRequest""
        },
        ""properties"": {
          ""description"": ""If the parameter is an object with flexible properties (additionProperties/dictionary), we will describe the properties of the object."",
          ""type"": ""object"",
          ""additionalProperties"": {
            ""$ref"": ""#/definitions/BeezUP.Common.LinkParameterProperty3""
          },
          ""example"": {
            ""shipOrder"": {
              ""type"": ""string""
            }
          }
        }
      }
    },
    ""BeezUP.Common.InfoSummaries"": {
      ""type"": ""object"",
      ""properties"": {
        ""successes"": {
          ""type"": ""array"",
          ""items"": {
            ""$ref"": ""#/definitions/BeezUP.Common.SuccessSummary""
          }
        },
        ""errors"": {
          ""type"": ""array"",
          ""items"": {
            ""$ref"": ""#/definitions/BeezUP.Common.ErrorSummary""
          }
        },
        ""warnings"": {
          ""type"": ""array"",
          ""items"": {
            ""$ref"": ""#/definitions/BeezUP.Common.WarningSummary""
          }
        },
        ""informations"": {
          ""type"": ""array"",
          ""items"": {
            ""$ref"": ""#/definitions/BeezUP.Common.InfoSummary""
          }
        }
      }
    },
    ""BeezUP.Common.LinkParameter2"": {
      ""type"": ""object"",
      ""x-deprecated"": ""true"",
      ""required"": [
        ""name""
      ],
      ""properties"": {
        ""name"": {
          ""type"": ""string"",
          ""description"": ""The name of the parameter"",
          ""example"": ""marketplaceTechnicalCode""
        },
        ""value"": {
          ""type"": ""string"",
          ""description"": ""The value of the parameter"",
          ""example"": ""1234""
        },
        ""required"": {
          ""type"": ""boolean"",
          ""example"": ""true""
        },
        ""in"": {
          ""$ref"": ""#/definitions/BeezUP.Common.ParameterIn""
        },
        ""type"": {
          ""$ref"": ""#/definitions/BeezUP.Common.ParameterType""
        },
        ""lovLink"": {
          ""description"": ""This parameter expect the values indicated in this list of values."",
          ""$ref"": ""#/definitions/BeezUP.Common.LOVLink2""
        },
        ""lovRequired"": {
          ""type"": ""boolean"",
          ""description"": ""If true, you MUST use indicate a value from the list of values otherwise it's a freetext"",
          ""example"": ""true""
        }
      }
    },
    ""BeezUP.Common.ParameterIn"": {
      ""type"": ""string"",
      ""description"": ""* path: if the parameter must be pass in the path uri\n* header: if the parameter must be passed in http header\n* query: if the parameter must be passed in querystring\n* body: if the paramter must be passed in the body\n"",
      ""example"": ""path"",
      ""enum"": [
        ""path"",
        ""header"",
        ""query"",
        ""body""
      ]
    },
    ""BeezUP.Common.ParameterType"": {
      ""description"": ""The value type of the parameter"",
      ""type"": ""string"",
      ""enum"": [
        ""string"",
        ""integer"",
        ""number"",
        ""boolean"",
        ""object"",
        ""array"",
        ""date"",
        ""date-time""
      ],
      ""default"": ""string"",
      ""example"": ""string""
    },
    ""BeezUP.Common.LOVLink3"": {
      ""description"": ""Describe the way you have to follow to get access to the LOV"",
      ""type"": ""object"",
      ""required"": [
        ""href""
      ],
      ""properties"": {
        ""href"": {
          ""type"": ""string"",
          ""format"": ""uri"",
          ""example"": ""/v2/public/Go2CultureName"",
          ""description"": ""Indicate the uri to the list of value""
        },
        ""method"": {
          ""$ref"": ""#/definitions/BeezUP.Common.HttpMethod""
        }
      }
    },
    ""BeezUP.Common.LinkParameterProperty3"": {
      ""type"": ""object"",
      ""required"": [
        ""type""
      ],
      ""properties"": {
        ""label"": {
          ""type"": ""string"",
          ""description"": ""The label corresponding to the link parameter property. This label is automatically translated based on the Accept-Language http header."",
          ""example"": ""The translated label""
        },
        ""value"": {
          ""type"": ""object"",
          ""description"": ""The value of the parameter. It can be an integer a string or an object."",
          ""example"": ""1234""
        },
        ""required"": {
          ""type"": ""boolean"",
          ""example"": ""true""
        },
        ""type"": {
          ""$ref"": ""#/definitions/BeezUP.Common.ParameterType""
        },
        ""lovLink"": {
          ""$ref"": ""#/definitions/BeezUP.Common.LOVLink3""
        },
        ""lovRequired"": {
          ""type"": ""boolean"",
          ""description"": ""If true, you MUST use indicate a value from the list of values otherwise it's a freetext"",
          ""example"": ""true""
        },
        ""description"": {
          ""type"": ""string"",
          ""description"": ""description of the parameter"",
          ""example"": ""the store identifier""
        },
        ""schema"": {
          ""type"": ""string"",
          ""description"": ""schema of the parameter"",
          ""example"": ""orderListRequest""
        }
      }
    },
    ""BeezUP.Common.LOVLink2"": {
      ""description"": ""Describe the way you have to follow to get access to the LOV"",
      ""x-deprecated"": ""true"",
      ""type"": ""object"",
      ""required"": [
        ""listName""
      ],
      ""properties"": {
        ""rel"": {
          ""type"": ""string"",
          ""description"": ""Indicate the relation name related to the link"",
          ""example"": ""LOV_Go2CultureName""
        },
        ""href"": {
          ""type"": ""string"",
          ""default"": ""/v2/user/{listName}"",
          ""example"": ""/v2/public/{listName}"",
          ""description"": ""Indicate the relative uri pattern to the list of value""
        },
        ""listName"": {
          ""type"": ""string"",
          ""description"": ""The name of the list of value"",
          ""example"": ""Go2CultureName""
        }
      }
    },
    ""BeezUP.Common.UserErrorMessage"": {
      ""type"": ""object"",
      ""required"": [
        ""code"",
        ""message""
      ],
      ""properties"": {
        ""docUrl"": {
          ""$ref"": ""#/definitions/BeezUP.Common.DocUrl""
        },
        ""code"": {
          ""type"": ""string"",
          ""description"": ""the error code. The error code can be a pattern containing the argument's name"",
          ""example"": ""CatalogImportationAlreadyInProgress(ExecutionId currentCatalogImportationId)""
        },
        ""message"": {
          ""type"": ""string"",
          ""description"": ""The error message"",
          ""example"": ""There is already an importation in progress: b24d0dd8-a561-478a-9b26-34f573f03527\n""
        },
        ""cultureName"": {
          ""type"": ""string"",
          ""description"": ""If the error is translated, the culture name will be indicated"",
          ""example"": ""en"",
          ""x-lov"": ""Go2CultureName""
        },
        ""arguments"": {
          ""type"": ""array"",
          ""description"": ""a dictionary string/object"",
          ""items"": {
            ""type"": ""object"",
            ""required"": [
              ""name"",
              ""value""
            ],
            ""properties"": {
              ""name"": {
                ""type"": ""string"",
                ""description"": ""The key of the parameter"",
                ""example"": ""currentCatalogImportationId""
              },
              ""value"": {
                ""type"": ""object"",
                ""description"": ""The value of the parameter. Depending to the type."",
                ""example"": ""b24d0dd8-a561-478a-9b26-34f573f03527""
              }
            }
          }
        }
      }
    },
    ""BeezUP.Common.SuccessSummary"": {
      ""type"": ""object"",
      ""properties"": {
        ""successCode"": {
          ""type"": ""string""
        },
        ""successMessage"": {
          ""type"": ""string""
        },
        ""successArguments"": {
          ""type"": ""object"",
          ""additionalProperties"": {
            ""type"": ""string""
          }
        },
        ""propertyName"": {
          ""type"": ""string""
        },
        ""propertyValue"": {
          ""type"": ""string""
        },
        ""objectName"": {
          ""type"": ""string""
        }
      }
    },
    ""BeezUP.Common.ErrorSummary"": {
      ""type"": ""object"",
      ""properties"": {
        ""utcDate"": {
          ""format"": ""date-time"",
          ""type"": ""string""
        },
        ""errorGuid"": {
          ""format"": ""uuid"",
          ""type"": ""string""
        },
        ""errorCode"": {
          ""type"": ""string""
        },
        ""errorMessage"": {
          ""type"": ""string""
        },
        ""technicalErrorMessage"": {
          ""type"": ""string""
        },
        ""exceptionDetail"": {
          ""$ref"": ""#/definitions/BeezUP.Common.ExceptionDetail""
        },
        ""errorArguments"": {
          ""type"": ""object"",
          ""additionalProperties"": {
            ""type"": ""string""
          }
        },
        ""propertyName"": {
          ""type"": ""string""
        },
        ""propertyValue"": {
          ""type"": ""string""
        },
        ""objectName"": {
          ""type"": ""string""
        },
        ""source"": {
          ""type"": ""string""
        }
      }
    },
    ""BeezUP.Common.WarningSummary"": {
      ""type"": ""object"",
      ""properties"": {
        ""technicalErrorMessage"": {
          ""type"": ""string""
        },
        ""warningMessage"": {
          ""type"": ""string""
        },
        ""warningCode"": {
          ""type"": ""string""
        },
        ""warningArguments"": {
          ""type"": ""object"",
          ""additionalProperties"": {
            ""type"": ""string""
          }
        }
      }
    },
    ""BeezUP.Common.InfoSummary"": {
      ""type"": ""object"",
      ""properties"": {
        ""informationCode"": {
          ""type"": ""string""
        },
        ""informationMessage"": {
          ""type"": ""string""
        },
        ""informationArguments"": {
          ""type"": ""object"",
          ""additionalProperties"": {
            ""type"": ""string""
          }
        },
        ""propertyName"": {
          ""type"": ""string""
        },
        ""propertyValue"": {
          ""type"": ""string""
        },
        ""objectName"": {
          ""type"": ""string""
        }
      }
    },
    ""BeezUP.Common.ExceptionDetail"": {
      ""type"": ""object"",
      ""properties"": {
        ""helpLink"": {
          ""type"": ""string""
        },
        ""message"": {
          ""type"": ""string""
        },
        ""stackTrace"": {
          ""type"": ""string""
        },
        ""type"": {
          ""type"": ""string""
        }
      }
    },
    ""BeezUP.Common.ApiCredential"": {
      ""description"": ""Your api credential"",
      ""type"": ""object"",
      ""properties"": {
        ""productName"": {
          ""description"": ""The product name related to this credential"",
          ""type"": ""string"",
          ""example"": ""UserAPI""
        },
        ""primaryToken"": {
          ""description"": ""The primary token to be used in the next call in the user scope API"",
          ""type"": ""string"",
          ""example"": ""3b22980d8d1143c6ba7adf4e55b9a153""
        },
        ""secondaryToken"": {
          ""description"": ""The secondary token. Could be usefull if you want to share your access with someone else."",
          ""type"": ""string"",
          ""example"": ""162ae17fd52044c38cce3388ea5b0c91""
        }
      }
    },
    ""processingStatus"": {
      ""x-exclude"": true,
      ""description"": ""The processing status of an operation"",
      ""type"": ""string"",
      ""enum"": [
        ""none"",
        ""inProgress"",
        ""done"",
        ""failed"",
        ""aborted""
      ]
    },
    ""feedType"": {
      ""type"": ""string"",
      ""enum"": [
        ""Product"",
        ""Offer""
      ]
    },
    ""feedEvent"": {
      ""type"": ""object"",
      ""discriminator"": ""eventType"",
      ""properties"": {
        ""sku"": {
          ""$ref"": ""#/definitions/sku""
        },
        ""eventType"": {
          ""type"": ""string""
        }
      }
    },
    ""sku"": {
      ""type"": ""string"",
      ""description"": ""The Stock Keeping Unit (SKU), i.e. a merchant-specific identifier for a product or service, or the product to which the offer refers. http://schema.org/sku"",
      ""maxLength"": ""50""
    },
    ""feedMetadata"": {
      ""type"": ""object"",
      ""properties"": {
        ""key"": {
          ""type"": ""string"",
          ""description"": ""The marketplace column name""
        },
        ""value"": {
          ""type"": ""string""
        }
      }
    },
    ""feedPublicationStatus"": {
      ""type"": ""string"",
      ""enum"": [
        ""NotStarted"",
        ""InProgress"",
        ""Success"",
        ""Failed""
      ],
      ""default"": ""NotStarted""
    },
    ""feedPublicationError"": {
      ""type"": ""object"",
      ""required"": [
        ""code"",
        ""count"",
        ""skuList""
      ],
      ""properties"": {
        ""code"": {
          ""type"": ""string""
        },
        ""message"": {
          ""type"": ""string""
        },
        ""skuList"": {
          ""type"": ""array"",
          ""minItems"": ""1"",
          ""items"": {
            ""$ref"": ""#/definitions/sku""
          }
        }
      }
    },
    ""errorResponseMessage"": {
      ""x-exclude"": true,
      ""type"": ""object"",
      ""required"": [
        ""errors""
      ],
      ""properties"": {
        ""errors"": {
          ""type"": ""array"",
          ""uniqueItems"": ""false"",
          ""items"": {
            ""$ref"": ""#/definitions/userErrorMessage""
          }
        }
      }
    },
    ""userErrorMessage"": {
      ""x-exclude"": true,
      ""type"": ""object"",
      ""required"": [
        ""code"",
        ""message""
      ],
      ""properties"": {
        ""docUrl"": {
          ""$ref"": ""#/definitions/docUrl""
        },
        ""code"": {
          ""$ref"": ""#/definitions/errorCode""
        },
        ""message"": {
          ""type"": ""string"",
          ""description"": ""The error message"",
          ""example"": ""There is already an importation in progress: b24d0dd8-a561-478a-9b26-34f573f03527\n""
        },
        ""cultureName"": {
          ""$ref"": ""#/definitions/cultureName""
        },
        ""arguments"": {
          ""$ref"": ""#/definitions/userErrorMessageArguments""
        }
      }
    },
    ""pageSize"": {
      ""x-exclude"": true,
      ""description"": ""Indicate the item count per page"",
      ""type"": ""integer"",
      ""format"": ""int32"",
      ""default"": ""100"",
      ""minimum"": ""25"",
      ""x-minimum"": ""25"",
      ""maximum"": ""100"",
      ""x-maximum"": ""100"",
      ""example"": ""100""
    },
    ""pageNumber"": {
      ""x-exclude"": true,
      ""description"": ""Indicates the page number"",
      ""format"": ""int32"",
      ""type"": ""integer"",
      ""example"": ""1"",
      ""minimum"": ""1"",
      ""x-minimum"": ""1"",
      ""default"": ""1""
    },
    ""paginationResultLinks"": {
      ""x-exclude"": true,
      ""description"": ""The navigation links 'first', 'last', 'next', 'previous'"",
      ""type"": ""object"",
      ""required"": [
        ""first"",
        ""last""
      ],
      ""properties"": {
        ""first"": {
          ""$ref"": ""#/definitions/link3""
        },
        ""last"": {
          ""$ref"": ""#/definitions/link3""
        },
        ""previous"": {
          ""$ref"": ""#/definitions/link3""
        },
        ""next"": {
          ""$ref"": ""#/definitions/link3""
        }
      }
    },
    ""link3"": {
      ""x-exclude"": true,
      ""type"": ""object"",
      ""required"": [
        ""href""
      ],
      ""properties"": {
        ""label"": {
          ""type"": ""string"",
          ""description"": ""The label corresponding to the link. This label is automatically translated based on the Accept-Language http header."",
          ""example"": ""The translated label""
        },
        ""docUrl"": {
          ""$ref"": ""#/definitions/docUrl""
        },
        ""description"": {
          ""type"": ""string"",
          ""description"": ""The description of the link"",
          ""example"": ""This is a description link""
        },
        ""href"": {
          ""$ref"": ""#/definitions/href""
        },
        ""operationId"": {
          ""$ref"": ""#/definitions/operationId""
        },
        ""method"": {
          ""$ref"": ""#/definitions/httpMethod""
        },
        ""parameters"": {
          ""$ref"": ""#/definitions/linkParameter3Types""
        },
        ""urlTemplated"": {
          ""type"": ""boolean"",
          ""description"": ""indicates whether the href is templated or not""
        },
        ""allRequiredParamsProvided"": {
          ""type"": ""boolean"",
          ""description"": ""indicates whether all required params have been provided""
        },
        ""allOptionalParamsProvided"": {
          ""type"": ""boolean"",
          ""description"": ""indicates whether all optionals params have been provided""
        },
        ""info"": {
          ""$ref"": ""#/definitions/infoSummaries""
        }
      }
    },
    ""docUrl"": {
      ""x-exclude"": true,
      ""type"": ""string"",
      ""format"": ""uri"",
      ""description"": ""The documentation related to this operation."",
      ""example"": ""https://api-docs.imn.io/#operation/EnableChannelCatalog""
    },
    ""href"": {
      ""x-exclude"": true,
      ""type"": ""string"",
      ""example"": ""/merchant/orders/v1/{marketplaceCode}/{IMNOrderId}"",
      ""description"": ""Indicate the relative uri for this link""
    },
    ""operationId"": {
      ""x-exclude"": true,
      ""type"": ""string"",
      ""description"": ""The operationId to call."",
      ""example"": ""GetOrder""
    },
    ""httpMethod"": {
      ""x-exclude"": true,
      ""type"": ""string"",
      ""enum"": [
        ""GET"",
        ""POST"",
        ""PATCH"",
        ""DELETE"",
        ""PUT"",
        ""HEAD""
      ],
      ""default"": ""GET"",
      ""example"": ""GET"",
      ""description"": ""Indicate the http method to use on this link""
    },
    ""linkParameter3Types"": {
      ""x-exclude"": true,
      ""type"": ""object"",
      ""additionalProperties"": {
        ""$ref"": ""#/definitions/linkParameter3""
      }
    },
    ""infoSummaries"": {
      ""type"": ""object"",
      ""properties"": {
        ""successes"": {
          ""type"": ""array"",
          ""items"": {
            ""$ref"": ""#/definitions/successSummary""
          }
        },
        ""errors"": {
          ""type"": ""array"",
          ""items"": {
            ""$ref"": ""#/definitions/errorSummary""
          }
        },
        ""warnings"": {
          ""type"": ""array"",
          ""items"": {
            ""$ref"": ""#/definitions/warningSummary""
          }
        },
        ""informations"": {
          ""type"": ""array"",
          ""items"": {
            ""$ref"": ""#/definitions/infoSummary""
          }
        }
      }
    },
    ""linkParameter3"": {
      ""x-exclude"": true,
      ""type"": ""object"",
      ""required"": [
        ""in""
      ],
      ""properties"": {
        ""label"": {
          ""type"": ""string"",
          ""description"": ""The label corresponding to the link parameter. This label is automatically translated based on the Accept-Language http header."",
          ""example"": ""The translated label""
        },
        ""value"": {
          ""type"": ""object"",
          ""description"": ""The value of the parameter. It can be an integer a string or an object."",
          ""example"": ""1234""
        },
        ""required"": {
          ""type"": ""boolean"",
          ""default"": ""false"",
          ""example"": ""true""
        },
        ""in"": {
          ""$ref"": ""#/definitions/parameterIn""
        },
        ""lovLink"": {
          ""$ref"": ""#/definitions/LOVLink3""
        },
        ""lovRequired"": {
          ""type"": ""boolean"",
          ""description"": ""If true, you MUST indicate a value from the list of values otherwise it's a freetext"",
          ""example"": ""true""
        },
        ""description"": {
          ""type"": ""string"",
          ""description"": ""description of the parameter"",
          ""example"": ""the store identifier""
        },
        ""schema"": {
          ""type"": ""string"",
          ""description"": ""schema of the parameter"",
          ""example"": ""orderListRequest""
        },
        ""pattern"": {
          ""$ref"": ""#/definitions/validationPattern""
        },
        ""properties"": {
          ""description"": ""If the parameter is an object with flexible properties (additionProperties/dictionary), we will describe the properties of the object."",
          ""additionalProperties"": {
            ""$ref"": ""#/definitions/linkParameterProperty3""
          },
          ""example"": {
            ""shipOrder"": {
              ""type"": ""​string""
            }
          }
        }
      }
    },
    ""parameterIn"": {
      ""x-exclude"": true,
      ""type"": ""string"",
      ""description"": ""* path: if the parameter must be pass in the path uri\n* header: if the parameter must be passed in http header\n* query: if the parameter must be passed in querystring\n* body: if the parameter must be passed in the body\n* file: if the parameter must be passed in a multipart/form-data (https://swagger.io/docs/specification/2-0/file-upload/)\n"",
      ""example"": ""path"",
      ""enum"": [
        ""path"",
        ""header"",
        ""query"",
        ""body"",
        ""file""
      ]
    },
    ""LOVLink3"": {
      ""x-exclude"": true,
      ""description"": ""Describe the way you have to follow to get access to the LOV"",
      ""type"": ""object"",
      ""required"": [
        ""href""
      ],
      ""properties"": {
        ""href"": {
          ""type"": ""string"",
          ""format"": ""uri"",
          ""example"": ""/merchant/lov/v1/Go2CultureName"",
          ""description"": ""Indicate the uri to the list of value""
        },
        ""method"": {
          ""$ref"": ""#/definitions/httpMethod""
        }
      }
    },
    ""validationPattern"": {
      ""type"": ""string"",
      ""description"": ""The regular expression to validate the value"",
      ""example"": ""*.-api$""
    },
    ""linkParameterProperty3"": {
      ""x-exclude"": true,
      ""type"": ""object"",
      ""required"": [
        ""type""
      ],
      ""properties"": {
        ""label"": {
          ""type"": ""string"",
          ""description"": ""The label corresponding to the link parameter property. This label is automatically translated based on the Accept-Language http header."",
          ""example"": ""The translated label""
        },
        ""value"": {
          ""type"": ""object"",
          ""description"": ""The value of the parameter. It can be an integer a string or an object."",
          ""example"": ""1234""
        },
        ""required"": {
          ""type"": ""boolean"",
          ""example"": ""true""
        },
        ""type"": {
          ""$ref"": ""#/definitions/parameterType""
        },
        ""lovLink"": {
          ""$ref"": ""#/definitions/LOVLink3""
        },
        ""lovRequired"": {
          ""type"": ""boolean"",
          ""description"": ""If true, you MUST use indicate a value from the list of values otherwise it's a freetext"",
          ""example"": ""true""
        },
        ""description"": {
          ""type"": ""string"",
          ""description"": ""description of the parameter"",
          ""example"": ""the store identifier""
        },
        ""schema"": {
          ""type"": ""string"",
          ""description"": ""schema of the parameter"",
          ""example"": ""orderListRequest""
        },
        ""pattern"": {
          ""$ref"": ""#/definitions/validationPattern""
        }
      }
    },
    ""parameterType"": {
      ""x-exclude"": true,
      ""description"": ""The value type of the parameter"",
      ""type"": ""string"",
      ""enum"": [
        ""string"",
        ""integer"",
        ""number"",
        ""boolean"",
        ""object"",
        ""array"",
        ""date"",
        ""date-time"",
        ""file""
      ],
      ""default"": ""string"",
      ""example"": ""string""
    },
    ""errorCode"": {
      ""x-exclude"": true,
      ""type"": ""string"",
      ""description"": ""the error code. The error code can be a pattern containing the argument's name"",
      ""example"": ""CatalogImportationAlreadyInProgress(ExecutionId currentCatalogImportationId)""
    },
    ""cultureName"": {
      ""x-exclude"": true,
      ""type"": ""string"",
      ""description"": ""If the error is translated, the culture name will be indicated"",
      ""example"": ""en"",
      ""x-lov"": ""Go2CultureName""
    },
    ""userErrorMessageArguments"": {
      ""x-exclude"": true,
      ""type"": ""object"",
      ""description"": ""a dictionary string/object"",
      ""additionalProperties"": {
        ""type"": ""object""
      }
    },
    ""successSummary"": {
      ""type"": ""object"",
      ""properties"": {
        ""successCode"": {
          ""type"": ""string""
        },
        ""successMessage"": {
          ""type"": ""string""
        },
        ""successArguments"": {
          ""type"": ""object"",
          ""additionalProperties"": {
            ""type"": ""string""
          }
        },
        ""propertyName"": {
          ""type"": ""string""
        },
        ""propertyValue"": {
          ""type"": ""string""
        },
        ""objectName"": {
          ""type"": ""string""
        }
      }
    },
    ""errorSummary"": {
      ""type"": ""object"",
      ""properties"": {
        ""utcDate"": {
          ""format"": ""date-time"",
          ""type"": ""string""
        },
        ""errorGuid"": {
          ""format"": ""uuid"",
          ""type"": ""string""
        },
        ""errorCode"": {
          ""type"": ""string""
        },
        ""errorMessage"": {
          ""type"": ""string""
        },
        ""technicalErrorMessage"": {
          ""type"": ""string""
        },
        ""exceptionDetail"": {
          ""$ref"": ""#/definitions/exceptionDetail""
        },
        ""errorArguments"": {
          ""type"": ""object"",
          ""additionalProperties"": {
            ""type"": ""string""
          }
        },
        ""propertyName"": {
          ""type"": ""string""
        },
        ""propertyValue"": {
          ""type"": ""string""
        },
        ""objectName"": {
          ""type"": ""string""
        },
        ""source"": {
          ""type"": ""string""
        }
      }
    },
    ""warningSummary"": {
      ""type"": ""object"",
      ""properties"": {
        ""technicalErrorMessage"": {
          ""type"": ""string""
        },
        ""warningMessage"": {
          ""type"": ""string""
        },
        ""warningCode"": {
          ""type"": ""string""
        },
        ""warningArguments"": {
          ""type"": ""object"",
          ""additionalProperties"": {
            ""type"": ""string""
          }
        }
      }
    },
    ""infoSummary"": {
      ""type"": ""object"",
      ""properties"": {
        ""informationCode"": {
          ""type"": ""string""
        },
        ""informationMessage"": {
          ""type"": ""string""
        },
        ""informationArguments"": {
          ""type"": ""object"",
          ""additionalProperties"": {
            ""type"": ""string""
          }
        },
        ""propertyName"": {
          ""type"": ""string""
        },
        ""propertyValue"": {
          ""type"": ""string""
        },
        ""objectName"": {
          ""type"": ""string""
        }
      }
    },
    ""exceptionDetail"": {
      ""type"": ""object"",
      ""properties"": {
        ""helpLink"": {
          ""type"": ""string""
        },
        ""message"": {
          ""type"": ""string""
        },
        ""stackTrace"": {
          ""type"": ""string""
        },
        ""type"": {
          ""type"": ""string""
        }
      }
    },
    ""merchantCode"": {
      ""type"": ""string"",
      ""description"": ""The merchant code identifier in IMN"",
      ""pattern"": ""^([A-Z|0-9]){5}$"",
      ""x-pattern"": ""^([A-Z|0-9]){5}$"",
      ""example"": ""MLT01""
    },
    ""marketplaceCode"": {
      ""type"": ""string"",
      ""description"": ""The marketplace code identifier in IMN.\nFor now we have:\n- C1 for Cdiscount\n- E1 for ePrice\n- R1 for Real.DE\n- B1 for BOL\n- E2 for eMAG\n"",
      ""pattern"": ""^([A-Z]|[0-9]){2}$"",
      ""x-pattern"": ""^([A-Z]|[0-9]){2}$"",
      ""example"": ""C1""
    },
    ""healthCheckStatus"": {
      ""type"": ""string"",
      ""description"": ""Possible status values of a health check result.\n* degraded:  The check is degraded, failing but not critical\n* healthy: The check is healthy\n* ignored: The check was ignored\n* unhealthy:  The check is unhealthy\n"",
      ""enum"": [
        ""degraded"",
        ""healthy"",
        ""ignored"",
        ""unhealthy""
      ]
    },
    ""messageId"": {
      ""x-exclude"": true,
      ""description"": ""The message identifier. It's a guid."",
      ""format"": ""MessageId"",
      ""type"": ""string"",
      ""example"": ""b0d3faea-f881-439f-ba92-02b1168511ea""
    },
    ""correlationId"": {
      ""x-exclude"": true,
      ""description"": ""The correlation identifier. It's a guid."",
      ""format"": ""CorrelationId"",
      ""type"": ""string"",
      ""example"": ""b0d3faea-f881-439f-ba92-02b1168511ea""
    },
    ""causeId"": {
      ""x-exclude"": true,
      ""description"": ""The cause identifier. It's a guid."",
      ""format"": ""CauseId"",
      ""type"": ""string"",
      ""example"": ""b0d3faea-f881-439f-ba92-02b1168511ea""
    },
    ""messageBase"": {
      ""x-exclude"": true,
      ""type"": ""object"",
      ""required"": [
        ""messageId"",
        ""correlationId""
      ],
      ""properties"": {
        ""messageId"": {
          ""allOf"": [
            {
              ""x-excludeProperty"": ""true""
            },
            {
              ""$ref"": ""#/definitions/messageId""
            }
          ]
        },
        ""correlationId"": {
          ""$ref"": ""#/definitions/correlationId""
        },
        ""causeId"": {
          ""$ref"": ""#/definitions/causeId""
        }
      }
    },
    ""eventType"": {
      ""x-exclude"": true,
      ""description"": ""The event type"",
      ""type"": ""string""
    }
  },
  ""x-azure-api-id"": ""MarketplaceGatewayFeedsV1"",
  ""responses"": {
    ""GeneralError"": {
      ""description"": ""Occurs when something goes wrong"",
      ""schema"": {
        ""$ref"": ""#/definitions/BeezUP.Common.ErrorResponseMessage""
      }
    }
  }
}
";
    }
#endif
}
