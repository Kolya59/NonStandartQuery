namespace NonStandartQuery.Classes
{
    public class Criterion
    {
        private readonly string value;

        public Criterion(string value)
        {
            this.value = value;
        }

        public static string[] GetAllowedOperations(Field field)
        {
            var type = new SqlType(field).GetCSharpType();
            if (type == typeof(string))
            {
                return new[] { "=", "LIKE", "<>" };
            }

            return type == typeof(bool) ? new[] { "=", "<>" } : new[] { "<", ">", "=", "<>", ">=", "<=" };
        }

        public string GetValue() => value;
    }
}
