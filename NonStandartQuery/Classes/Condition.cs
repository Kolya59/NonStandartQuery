using System;
using System.Collections.Generic;

namespace NonStandartQuery.Classes
{
    internal class Condition : IEquatable<Condition>
    {
        public Condition(Field field, Criterion criterion, object expression, string bunch)
        {
            Field = field;
            Criterion = criterion;
            Expression = expression;
            Bunch = bunch;
        }

        public Condition(Field field, object expression, string bunch)
        {
            Field = field;
            Criterion = new Criterion(null);
            Expression = expression;
            Bunch = bunch;
        }

        public Field Field { get; }

        public Criterion Criterion { get; }

        public object Expression { get; }

        public string Bunch { get; }

        public static bool operator ==(Condition condition1, Condition condition2)
        {
            return EqualityComparer<Condition>.Default.Equals(condition1, condition2);
        }

        public static bool operator !=(Condition condition1, Condition condition2)
        {
            return !(condition1 == condition2);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Condition);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Field != null ? Field.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Criterion != null ? Criterion.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Expression != null ? Expression.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Bunch != null ? Bunch.GetHashCode() : 0);
                return hashCode;
            }
        }

        public bool Equals(Condition other)
        {
            return other != null &&
                   EqualityComparer<Field>.Default.Equals(Field, other.Field) &&
                   EqualityComparer<Criterion>.Default.Equals(Criterion, other.Criterion) &&
                   EqualityComparer<object>.Default.Equals(Expression, other.Expression) &&
                   Bunch == other.Bunch;
        }
    }
}
