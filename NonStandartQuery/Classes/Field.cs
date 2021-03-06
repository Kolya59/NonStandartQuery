﻿namespace NonStandartQuery.Classes
{
    using System;

    public class Field : IEquatable<Field>
    {
        public Field(string name, string displayedName, string type, string tableName, string categoryName)
        {
            Name = name;
            DisplayedName = displayedName;
            Type = type;
            TableName = tableName;
            CategoryName = categoryName;
        }

        public string DisplayedName { get; }

        public string Type { get; }

        public string TableName { get; }

        public string CategoryName { get; }

        public string Name { get; }

        public static bool operator ==(Field left, Field right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Field left, Field right)
        {
            return !Equals(left, right);
        }

        public string GetFullName() => @"[" + TableName + "].[" + Name + "]";

        public override bool Equals(object field)
        {
            try
            {
                return Name == ((Field)field).Name && DisplayedName == ((Field)field).DisplayedName && Type == ((Field)field).Type &&
                       TableName == ((Field)field).TableName;
            }
            catch (InvalidCastException)
            {
                return DisplayedName == (string)field;
            }
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Name != null ? Name.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ (DisplayedName != null ? DisplayedName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Type != null ? Type.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (TableName != null ? TableName.GetHashCode() : 0);
                return hashCode;
            }
        }

        public bool Equals(Field other) => string.Equals(Name, other.Name) &&
                                               string.Equals(DisplayedName, other.DisplayedName) &&
                                               Type == other.Type && string.Equals(TableName, other.TableName);

        public override string ToString() => DisplayedName;
    }
}
