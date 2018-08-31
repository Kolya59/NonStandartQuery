using System;
using System.Collections.Generic;
using System.Linq;

namespace NonStandartQuery.Classes
{
    internal class SqlType : IEquatable<SqlType>
    {
        private string Name { get; }

        private Type AnalogType { get; }

        public SqlType(string name)
        {
            Name = name;
            AnalogType = GetCSharpType();
        }

        public SqlType(Field field)
        {
            Name = field.Type;
            AnalogType = GetCSharpType();
        }

        private static readonly Dictionary<string, Type> CollectionOfTypesMatching = new Dictionary<string, Type>
                                                                                         {
                                                                                             {"bigint", typeof(long)},
                                                                                             {"numeric", typeof(decimal)},
                                                                                             {"bit", typeof(bool)},
                                                                                             {"smallint", typeof(short)},
                                                                                             {"decimal", typeof(decimal)},
                                                                                             {"smallmoney", typeof(decimal)},
                                                                                             {"int", typeof(int)},
                                                                                             {"tinyint", typeof(byte)},
                                                                                             {"money", typeof(decimal)},
                                                                                             {"float", typeof(double)},
                                                                                             {"real", typeof(float)},
                                                                                             {"date", typeof(DateTime)},
                                                                                             {"datetimeoffset", typeof(DateTimeOffset)},
                                                                                             {"datetime2", typeof(DateTime)},
                                                                                             {"smalldatetime", typeof(DateTime)},
                                                                                             {"datetime", typeof(DateTime)},
                                                                                             {"time", typeof(TimeSpan)},
                                                                                             {"char", typeof(string)},
                                                                                             {"varchar", typeof(string)},
                                                                                             {"text", typeof(string)},
                                                                                             {"nchar", typeof(string)},
                                                                                             {"nvarchar", typeof(string)},
                                                                                             {"ntext", typeof(string)},
                                                                                             {"binary", typeof(byte[])},
                                                                                             {"varbinary", typeof(byte[])},
                                                                                             {"image", typeof(byte[])}
                                                                                         };

        public static bool operator ==(SqlType type1, SqlType type2)
        {
            return EqualityComparer<SqlType>.Default.Equals(type1, type2);
        }

        public static bool operator !=(SqlType type1, SqlType type2)
        {
            return !(type1 == type2);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ (AnalogType != null ? AnalogType.GetHashCode() : 0);
            }
        }

        public override string ToString() => Name;

        public Type GetCSharpType() => CollectionOfTypesMatching.FirstOrDefault(t => t.Key == Name).Value;

        public string GetSqlType() => Name;

        public bool Equals(SqlType other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return string.Equals(Name, other.Name) && AnalogType == other.AnalogType;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as SqlType);
        }
    }
}
