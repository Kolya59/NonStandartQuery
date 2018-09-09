namespace NonStandartQuery.Classes
{
    using System.Collections.Generic;

    public class OrderFieldComparer : IComparer<OrderedField>
    {
        public int Compare(OrderedField x, OrderedField y)
        {
            if (x.OrderIndex > y.OrderIndex)
            {
                return 1;
            }

            if (x.OrderIndex < y.OrderIndex)
            {
                return -1;
            }

            return 0;
        }
    }
}
