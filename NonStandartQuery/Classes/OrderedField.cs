namespace NonStandartQuery.Classes
{
    public class OrderedField : Field
    {
        public OrderedField(string name, string displayedName, string type, string tableName, string categoryName, string order)
            : base(name, displayedName, type, tableName, categoryName)
        {
            Order = order;
        }

        public OrderedField(Field field, string order, int index)
            : base(field.Name, field.DisplayedName, field.Type, field.TableName, field.CategoryName)
        {
            Order = order;
            OrderIndex = index;
        }

        public string Order { get; set; }

        public int OrderIndex { get; set; }

        public Field ToField() => new Field(Name, DisplayedName, Type, TableName, CategoryName);
    }
}