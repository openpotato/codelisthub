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

using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CodeListHub.Dto
{
    /// <summary>
    /// The publisher responsible for publishing and/or maintaining the codes
    /// </summary>
    [SwaggerSchema(ReadOnly = true)]
    public class PublisherResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PublisherResponse"/> class.
        /// </summary>
        /// <param name="publisher">The data from the database for assigning</param>
        public PublisherResponse(DataLayer.Publisher publisher)
        {
            Identifier = string.IsNullOrEmpty(publisher?.Identifier?.Value) ? null : new IdentifierResponse(publisher.Identifier);
            ShortName = publisher.ShortName;
            LongName = publisher.LongName;
            Url = new Uri(publisher.Url);
        }

        /// <summary>
        /// Identifier of the publisher
        /// </summary>
        [JsonPropertyOrder(1)]
        public IdentifierResponse Identifier { get; set; }

        /// <summary>
        /// Human-readable name for the publisher
        /// </summary>
        [JsonPropertyOrder(3)]
        public string LongName { get; set; }

        /// <summary>
        /// Short name of the publisher
        /// </summary>
        [Required]
        [JsonPropertyOrder(2)]
        public string ShortName { get; set; }

        /// <summary>
        /// Url with further information.
        /// </summary>
        [JsonPropertyOrder(4)]
        public Uri Url { get; set; }
    }
}
