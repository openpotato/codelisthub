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
    public class IdentifierResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IdentifierResponse"/> class.
        /// </summary>
        /// <param name="identifier">The data from the database for assigning</param>
        public IdentifierResponse(DataLayer.Identifier identifier)
        {
            Source = string.IsNullOrEmpty(identifier?.Source?.ShortName) ? null : new IdentifierSourceResponse(identifier.Source);
            Value = identifier.Value;
        }

        /// <summary>
        /// The source of the identifier.
        /// </summary>
        [JsonPropertyOrder(1)]
        public IdentifierSourceResponse Source { get; set; }

        /// <summary>
        /// The identifier value.
        /// </summary>
        [Required]
        [JsonPropertyOrder(2)]
        public string Value { get; set; }
    }
}
