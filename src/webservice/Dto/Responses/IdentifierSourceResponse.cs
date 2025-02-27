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
    /// A general identifier.
    /// </summary>
    [SwaggerSchema(ReadOnly = true)]
    public class IdentifierSourceResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IdentifierSourceResponse"/> class.
        /// </summary>
        /// <param name="source">The data from the database for assigning</param>
        public IdentifierSourceResponse(DataLayer.IdentifierSource source)
        {
            ShortName= source.ShortName;
            LongName = source.LongName;
            Url = source.Url;
        }

        /// <summary>
        /// Human-readable name of the source.
        /// </summary>
        [JsonPropertyOrder(2)]
        public string LongName { get; set; }

        /// <summary>
        /// Short name of the source.
        /// </summary>
        [Required]
        [JsonPropertyOrder(1)]
        public string ShortName { get; set; }

        /// <summary>
        /// Url with further information.
        /// </summary>
        [JsonPropertyOrder(3)]
        public string Url { get; set; }
    }
}
