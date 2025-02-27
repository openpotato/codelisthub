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

using System.Globalization;

namespace CodeListHub
{
    /// <summary>
    /// Represents a weighted value (or quality value) from an http header e.g. gzip=0.9; deflate; x-gzip=0.5;
    /// </summary>
    /// <remarks>
    /// See RFC 9110, Quality Values: https://www.rfc-editor.org/rfc/rfc9110.html#name-quality-values
    /// Adapted from https://github.com/fatso83/QValues
    /// </remarks>
    /// <example>
    /// Accept:          text/json,application/json,application/xhtml+xml,text/html;q=0.9,text/plain;q=0.8,image/png,*/*;q=0.5
    /// Accept-Encoding: gzip,deflate
    /// Accept-Charset:  ISO-8859-1,utf-8;q=0.7,*;q=0.7
    /// Accept-Language: de,en-gb,en;q=0.5
    /// </example>
    public struct QualityValue : IComparable<QualityValue>
    {
        private const float defaultWeight = 1;
        private static readonly char[] delimiters = [';','='];
        private string _name;
        private int _ordinal;
        private float _weight;

        /// <summary>
        /// Creates a new <see cref="QualityValue"/> by parsing a given value 
        /// for name and weight
        /// </summary>
        /// <param name="value">The value to be parsed e.g. gzip=0.3</param>
        public QualityValue(string value)
            : this(value, 0)
        { }

        /// <summary>
        /// Creates a new <see cref="QualityValue"/> by parsing a given value 
        /// for name and weight and assigns the given ordinal
        /// </summary>
        /// <param name="value">The value to be parsed e.g. gzip=0.3</param>
        /// <param name="ordinal">The ordinal/index where the item found in the original list.</param>
        public QualityValue(string value, int ordinal)
        {
            _name = null;
            _weight = 0;
            _ordinal = ordinal;
            ParseInternal(value);
        }

        /// <summary>
        /// Whether the value can be accepted (i.e. it's weight is greater than zero)
        /// </summary>
        public readonly bool CanAccept
        {
            get 
            { 
                return _weight > 0; 
            }
        }

        /// <summary>
        /// Whether the value is empty (i.e. has no name)
        /// </summary>
        public readonly bool IsEmpty
        {
            get 
            { 
                return string.IsNullOrEmpty(_name); 
            }
        }

        /// <summary>
        /// The name of the value part
        /// </summary>
        public string Name
        {
            get 
            { 
                return _name; 
            }
            internal set 
            {
                _name = value;
            }
        }

        /// <summary>
        /// The weighting (or qvalue, quality value) of the encoding
        /// </summary>
        public float Weight
        {
            get
            {
                return _weight;
            }
            internal set
            {
                _weight = value;
            }
        }

        /// <summary>
        /// Compares two <see cref="QualityValue"/> instances in ascending order.
        /// </summary>
        /// <param name="first">The first <see cref="QualityValue"/> instance</param>
        /// <param name="second">The second <see cref="QualityValue"/> instance</param>
        /// <returns>An integer that indicates whether the first instance precedes, follows, or appears in the same position 
        /// in the sort order as the second instance.</returns>
        public static int CompareByWeightAsc(QualityValue first, QualityValue second)
        {
            return first.CompareTo(second);
        }

        /// <summary>
        /// Compares two <see cref="QualityValue"/> instances in descending order.
        /// </summary>
        /// <param name="first">The first <see cref="QualityValue"/> instance</param>
        /// <param name="second">The second <see cref="QualityValue"/> instance</param>
        /// <returns>An integer that indicates whether the first instance precedes, follows, or appears in the same position 
        /// in the sort order as the second instance.</returns>
        public static int CompareByWeightDesc(QualityValue first, QualityValue second)
        {
            return -first.CompareTo(second);
        }

        /// <summary>
        /// Parses the given string for name and weigth (qvalue) and
        /// gives back a new <see cref="QualityValue"/> instance.
        /// </summary>
        /// <param name="value">The string to parse</param>
        /// <returns>A new <see cref="QualityValue"/> instance</returns>
        public static QualityValue Parse(string value)
        {
            return new QualityValue(value);
        }

        /// <summary>
        /// Parses the given string for name and weigth (qvalue) and
        /// gives back a new <see cref="QualityValue"/> instance.
        /// </summary>
        /// <param name="value">The string to parse</param>
        /// <param name="ordinal">The order of the <see cref="QualityValue"/> instance in sequence</param>
        /// <returns>A new <see cref="QualityValue"/> instance</returns>
        public static QualityValue Parse(string value, int ordinal)
        {
            return new QualityValue(value, ordinal);
        }

        /// <summary>
        /// Compares this instance to another <see cref="QualityValue"/> instance by
        /// comparing first weights, then ordinals.
        /// </summary>
        /// <param name="other">The <see cref="QualityValue"/> instance to compare</param>
        /// <returns>An integer that indicates whether this instance precedes, follows, or appears in the same position 
        /// in the sort order as the other parameter.</returns>
        public int CompareTo(QualityValue other)
        {
            int value = _weight.CompareTo(other._weight);
            if (value == 0)
            {
                int ord = -_ordinal;
                value = ord.CompareTo(-other._ordinal);
            }
            return value;
        }

        /// <summary>
        /// Parses the given string for name and weigth assigns those values to the current instance.
        /// </summary>
        /// <param name="value">The string to parse</param>
        private void ParseInternal(string value)
        {
            var parts = value.Split(delimiters, 3);
            
            if (parts.Length > 0)
            {
                _name = parts[0].Trim();
                _weight = defaultWeight;
            }

            if (parts.Length == 3)
            {
                if (float.TryParse(parts[2], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture.NumberFormat, out var weight))
                {
                    _weight = weight;
                }
            }
        }
    }
}