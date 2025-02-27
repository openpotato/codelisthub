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

using Microsoft.AspNetCore.WebUtilities;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CodeListHub
{
    /// <summary>
    /// Customize the Swagger (OpenAPI) documentation for problem details responses
    /// </summary>
    public class ProblemDetailsOperationFilter : IOperationFilter
    {
        private static readonly OpenApiObject _status400ProblemDetails = new()
        {
            ["type"] = new OpenApiString("https://tools.ietf.org/html/rfc7231#section-6.5.1"),
            ["title"] = new OpenApiString(ReasonPhrases.GetReasonPhrase(StatusCodes.Status400BadRequest)),
            ["status"] = new OpenApiInteger(StatusCodes.Status400BadRequest),
            ["traceId"] = new OpenApiString("00-982607166a542147b435be3a847ddd71-fc75498eb9f09d48-00"),
            ["errors"] = new OpenApiObject
            {
                ["property1"] = new OpenApiArray
                {
                    new OpenApiString("The property field is required"),
                }
            }
        };

        private static readonly OpenApiObject _status401ProblemDetails = new()
        {
            ["type"] = new OpenApiString("https://tools.ietf.org/html/rfc7235#section-3.1"),
            ["title"] = new OpenApiString(ReasonPhrases.GetReasonPhrase(StatusCodes.Status401Unauthorized)),
            ["status"] = new OpenApiInteger(StatusCodes.Status401Unauthorized),
            ["traceId"] = new OpenApiString("00-982607166a542147b435be3a847ddd71-fc75498eb9f09d48-00"),
        };

        private static readonly OpenApiObject _status403ProblemDetails = new()
        {
            ["type"] = new OpenApiString("https://tools.ietf.org/html/rfc7231#section-6.5.3"),
            ["title"] = new OpenApiString(ReasonPhrases.GetReasonPhrase(StatusCodes.Status403Forbidden)),
            ["status"] = new OpenApiInteger(StatusCodes.Status403Forbidden),
            ["traceId"] = new OpenApiString("00-982607166a542147b435be3a847ddd71-fc75498eb9f09d48-00"),
        };

        private static readonly OpenApiObject _status404ProblemDetails = new()
        {
            ["type"] = new OpenApiString("https://tools.ietf.org/html/rfc7231#section-6.5.4"),
            ["title"] = new OpenApiString(ReasonPhrases.GetReasonPhrase(StatusCodes.Status404NotFound)),
            ["status"] = new OpenApiInteger(StatusCodes.Status404NotFound),
            ["traceId"] = new OpenApiString("00-982607166a542147b435be3a847ddd71-fc75498eb9f09d48-00"),
        };

        private static readonly OpenApiObject _status406ProblemDetails = new()
        {
            ["type"] = new OpenApiString("https://tools.ietf.org/html/rfc7231#section-6.5.6"),
            ["title"] = new OpenApiString(ReasonPhrases.GetReasonPhrase(StatusCodes.Status406NotAcceptable)),
            ["status"] = new OpenApiInteger(StatusCodes.Status406NotAcceptable),
            ["traceId"] = new OpenApiString("00-982607166a542147b435be3a847ddd71-fc75498eb9f09d48-00"),
        };

        private static readonly OpenApiObject _status409ProblemDetails = new()
        {
            ["type"] = new OpenApiString("https://tools.ietf.org/html/rfc7231#section-6.5.8"),
            ["title"] = new OpenApiString(ReasonPhrases.GetReasonPhrase(StatusCodes.Status409Conflict)),
            ["status"] = new OpenApiInteger(StatusCodes.Status409Conflict),
            ["traceId"] = new OpenApiString("00-982607166a542147b435be3a847ddd71-fc75498eb9f09d48-00"),
        };

        private static readonly OpenApiObject _status415ProblemDetails = new()
        {
            ["type"] = new OpenApiString("https://tools.ietf.org/html/rfc7231#section-6.5.13"),
            ["title"] = new OpenApiString(ReasonPhrases.GetReasonPhrase(StatusCodes.Status415UnsupportedMediaType)),
            ["status"] = new OpenApiInteger(StatusCodes.Status415UnsupportedMediaType),
            ["traceId"] = new OpenApiString("00-982607166a542147b435be3a847ddd71-fc75498eb9f09d48-00"),
        };

        private static readonly OpenApiObject _status422ProblemDetails = new()
        {
            ["type"] = new OpenApiString("https://tools.ietf.org/html/rfc4918#section-11.2"),
            ["title"] = new OpenApiString(ReasonPhrases.GetReasonPhrase(StatusCodes.Status422UnprocessableEntity)),
            ["status"] = new OpenApiInteger(StatusCodes.Status422UnprocessableEntity),
            ["traceId"] = new OpenApiString("00-982607166a542147b435be3a847ddd71-fc75498eb9f09d48-00"),
        };

        private static readonly OpenApiObject _status500ProblemDetails = new()
        {
            ["type"] = new OpenApiString("https://tools.ietf.org/html/rfc7231#section-6.6.1"),
            ["title"] = new OpenApiString(ReasonPhrases.GetReasonPhrase(StatusCodes.Status500InternalServerError)),
            ["status"] = new OpenApiInteger(StatusCodes.Status500InternalServerError),
            ["traceId"] = new OpenApiString("00-982607166a542147b435be3a847ddd71-fc75498eb9f09d48-00"),
        };

        /// <summary>
        /// Apply customization
        /// </summary>
        /// <param name="operation">Represents the OpenAPI operation metadata (e.g., HTTP method, parameters, responses).</param>
        /// <param name="context">Provides access to the API description and the reflection context.</param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var problemDetails = new Dictionary<string, IOpenApiAny>
            {
                { StatusCodes.Status400BadRequest.ToString(), _status400ProblemDetails },
                { StatusCodes.Status401Unauthorized.ToString(), _status401ProblemDetails },
                { StatusCodes.Status403Forbidden.ToString(), _status403ProblemDetails },
                { StatusCodes.Status404NotFound.ToString(), _status404ProblemDetails },
                { StatusCodes.Status406NotAcceptable.ToString(), _status406ProblemDetails },
                { StatusCodes.Status409Conflict.ToString(), _status409ProblemDetails },
                { StatusCodes.Status415UnsupportedMediaType.ToString(), _status415ProblemDetails },
                { StatusCodes.Status422UnprocessableEntity.ToString(), _status422ProblemDetails },
                { StatusCodes.Status500InternalServerError.ToString(), _status500ProblemDetails },
            };

            foreach (var operationResponse in operation.Responses)
            {
                if (problemDetails.TryGetValue(operationResponse.Key, out var problemDetail))
                {
                    operationResponse.Value.Content[MediaTypeNames.Application.ProblemDetails].Example = problemDetail;
                }
            }
        }
    }
}
