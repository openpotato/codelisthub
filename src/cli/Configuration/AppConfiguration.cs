﻿#region CodeListHub - Copyright (C) STÜBER SYSTEMS GmbH
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

namespace CodeListHub.CLI
{
    /// <summary>
    /// CLI configuration 
    /// </summary>
    public class AppConfiguration
    {
        /// <summary>
        /// Database configuration
        /// </summary>
        public DatabaseConfiguration Database { get; set; } = new DatabaseConfiguration();

        /// <summary>
        /// File depot
        /// </summary>
        public FileDepotConfiguration FileDepot { get; set; } = new FileDepotConfiguration();
    }
}
