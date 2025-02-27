#region CodeListHub - Copyright (C) STÜBER SYSTEMS GmbH
/*    
 *    CodeListHub 
 *    
 *    Copyright (C) STÜBER SYSTEMS GmbH
 *
 *    This program is free software: you can redistribute it and/or modify
 *    it under the terms of the GNU Affero General Public License, version 3,
 *    as published by the Free Software Foundation.
 *
 *    This program is distributed in the hope that it will be useful,
 *    but WITHOUT ANY WARRANTY; without even the implied warranty of
 *    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 *    GNU Affero General Public License for more details.
 *
 *    You should have received a copy of the GNU Affero General Public License
 *    along with this program. If not, see <http://www.gnu.org/licenses/>.
 *
 */
#endregion

using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CodeListHub
{
    /// <summary>
    /// Customize the Swagger (OpenAPI) documentation for each operation
    /// </summary>
    public class AppOperationFilter : IOperationFilter
    {
        /// <summary>
        /// Apply customization
        /// </summary>
        /// <param name="operation">Represents the OpenAPI operation metadata (e.g., HTTP method, parameters, responses).</param>
        /// <param name="context">Provides access to the API description and the reflection context.</param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var apiDescription = context.ApiDescription;
            if (apiDescription == null) return;

            if (apiDescription.ActionDescriptor is ControllerActionDescriptor actionDescriptor)
            {
                if (actionDescriptor.ActionName == "GetAlternativeFormat")
                {
                    operation.Responses[StatusCodes.Status200OK.ToString()].Content[MediaTypeNames.Text.Csv].Example = GetAlternativeFormatExample();
                }
                else if (actionDescriptor.ActionName == "GetDocument")
                {
                    operation.Responses[StatusCodes.Status200OK.ToString()].Content[MediaTypeNames.Application.Json].Example = GetDocumentExample();
                    operation.Responses[StatusCodes.Status200OK.ToString()].Content[MediaTypeNames.Text.Json].Example = GetDocumentExample();
                    operation.Responses[StatusCodes.Status200OK.ToString()].Content[MediaTypeNames.Text.Plain].Example = GetDocumentExample();
                }
                else if (actionDescriptor.ActionName == "GetDocumentIndex")
                {
                    operation.Responses[StatusCodes.Status200OK.ToString()].Content[MediaTypeNames.Application.Json].Example = GetDocumentIndexExample();
                    operation.Responses[StatusCodes.Status200OK.ToString()].Content[MediaTypeNames.Text.Json].Example = GetDocumentIndexExample();
                    operation.Responses[StatusCodes.Status200OK.ToString()].Content[MediaTypeNames.Text.Plain].Example = GetDocumentIndexExample();
                }
                else if (actionDescriptor.ActionName == "GetDocumentLanguages")
                {
                    operation.Responses[StatusCodes.Status200OK.ToString()].Content[MediaTypeNames.Application.Json].Example = GetLanguagesExample();
                    operation.Responses[StatusCodes.Status200OK.ToString()].Content[MediaTypeNames.Text.Json].Example = GetLanguagesExample();
                    operation.Responses[StatusCodes.Status200OK.ToString()].Content[MediaTypeNames.Text.Plain].Example = GetLanguagesExample();
                }
                else if (actionDescriptor.ActionName == "GetTags")
                {
                    operation.Responses[StatusCodes.Status200OK.ToString()].Content[MediaTypeNames.Application.Json].Example = GetTagsExample();
                    operation.Responses[StatusCodes.Status200OK.ToString()].Content[MediaTypeNames.Text.Json].Example = GetTagsExample();
                    operation.Responses[StatusCodes.Status200OK.ToString()].Content[MediaTypeNames.Text.Plain].Example = GetTagsExample();
                }
                else if (actionDescriptor.ActionName == "GetPublishers")
                {
                    operation.Responses[StatusCodes.Status200OK.ToString()].Content[MediaTypeNames.Application.Json].Example = GetPublisherExample();
                    operation.Responses[StatusCodes.Status200OK.ToString()].Content[MediaTypeNames.Text.Json].Example = GetPublisherExample();
                    operation.Responses[StatusCodes.Status200OK.ToString()].Content[MediaTypeNames.Text.Plain].Example = GetPublisherExample();
                }
            }
        }

        private static OpenApiString GetAlternativeFormatExample()
        {
            return new OpenApiString(
                """
                code,name
                code1,Name 1
                code2,Name 2
                """
            );
        }

        private static OpenApiObject GetDocumentExample()
        {
            return new OpenApiObject
            {
                ["$opencodelist"] = new OpenApiString("0.2.0"),
                ["codeList"] = new OpenApiObject
                {
                    ["identification"] = new OpenApiObject
                    {
                        ["language"] = new OpenApiString("en"),
                        ["shortName"] = new OpenApiString("MyCodeList"),
                        ["longName"] = new OpenApiString("My Code List"),
                        ["canonicalUri"] = new OpenApiString("urn:anycodelist:mycodelist"),
                        ["canonicalVersionUri"] = new OpenApiString("urn:anycodelist:mycodelist:v1"),
                        ["publishedAt"] = new OpenApiString("2025-02-12T11:00:00+00:00"),
                        ["publisher"] = GetPublisherExample(),
                        ["tags"] = GetTagsExample()
                    },
                    ["columnSet"] = new OpenApiObject
                    {
                        ["columns"] = new OpenApiArray
                            {
                                new OpenApiObject
                                {
                                    ["id"] = new OpenApiString("code"),
                                    ["name"] = new OpenApiString("Code"),
                                    ["type"] = new OpenApiString("string")
                                },
                                new OpenApiObject
                                {
                                    ["id"] = new OpenApiString("name"),
                                    ["name"] = new OpenApiString("Name"),
                                    ["type"] = new OpenApiString("string")
                                }
                            }
                    },
                    ["dataSet"] = new OpenApiObject
                    {
                        ["rows"] = new OpenApiArray
                            {
                                new OpenApiObject
                                {
                                    ["code"] = new OpenApiString("code1"),
                                    ["name"] = new OpenApiString("Name 1")
                                },
                                new OpenApiObject
                                {
                                    ["code"] = new OpenApiString("code2"),
                                    ["name"] = new OpenApiString("Name 2")
                                }
                            }
                    }
                }
            };
        }

        private static OpenApiObject GetDocumentIndexExample()
        {
            return new OpenApiObject
            {
                ["language"] = new OpenApiString("en"),
                ["shortName"] = new OpenApiString("MyCodeList"),
                ["longName"] = new OpenApiString("My Code List"),
                ["documentType"] = new OpenApiString("CodeList"),
                ["canonicalUri"] = new OpenApiString("urn:anycodelist:mycodelist"),
                ["canonicalVersionUri"] = new OpenApiString("urn:anycodelist:mycodelist:v1"),
                ["publishedAt"] = new OpenApiString("2025-02-12T11:00:00+00:00"),
                ["publisher"] = GetPublisherExample(),
                ["tags"] = GetTagsExample()
            };
        }
        private static OpenApiArray GetLanguagesExample()
        {
            return
            [
                new OpenApiString("de"),
                new OpenApiString("en")
            ];
        }

        private static OpenApiObject GetPublisherExample()
        {
            return new OpenApiObject
            {
                ["shortName"] = new OpenApiString("AnyPublisher"),
                ["longName"] = new OpenApiString("Any Publisher"),
                ["url"] = new OpenApiString("https://example.com/anypublisher/")
            };
        }
        private static OpenApiArray GetTagsExample()
        {
            return
            [
                new OpenApiString("tag1"),
                new OpenApiString("tag2")
            ];
        }
    }
}
