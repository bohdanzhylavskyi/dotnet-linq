using System;
using System.Collections.Generic;
using System.Linq;
using Task1.DoNotChange;

namespace Task1
{
    public static class LinqTask
    {
        public static IEnumerable<Customer> Linq1(IEnumerable<Customer> customers, decimal limit)
        {
            return customers.Where(c => c.Orders.Sum(o => o.Total) > limit);
        }

        public static IEnumerable<(Customer customer, IEnumerable<Supplier> suppliers)> Linq2(
            IEnumerable<Customer> customers,
            IEnumerable<Supplier> suppliers
        )
        {
            var supplierLookup = suppliers
                                .GroupBy(s => (s.Country, s.City))
                                .ToDictionary(g => g.Key, g => g.AsEnumerable());

            return customers.Select(c =>
            {
                supplierLookup.TryGetValue((c.Country, c.City), out var matchingSuppliers);

                return (c, matchingSuppliers ?? Enumerable.Empty<Supplier>());
            });
        }

        public static IEnumerable<(Customer customer, IEnumerable<Supplier> suppliers)> Linq2UsingGroup(
            IEnumerable<Customer> customers,
            IEnumerable<Supplier> suppliers
        )
        {
            return customers.Select((c) =>
            {
                return (c, suppliers.Where(s => s.Country == c.Country && s.City == c.City));
            });
        }

        public static IEnumerable<Customer> Linq3(IEnumerable<Customer> customers, decimal limit)
        {
            return customers.Where(c =>
            {
                var total = c.Orders.Sum(o => o.Total);

                return total > limit && total != 0;
            });
        }

        public static IEnumerable<(Customer customer, DateTime dateOfEntry)> Linq4(
            IEnumerable<Customer> customers
        )
        {
            return customers.Where(c => c.Orders.Count() > 0)
                    .Select(c => (c, c.Orders.FirstOrDefault().OrderDate));
        }

        public static IEnumerable<(Customer customer, DateTime dateOfEntry)> Linq5(
            IEnumerable<Customer> customers
        )
        {
            return customers.Where(c => c.Orders.Count() > 0)
                    .Select(c => (c, c.Orders.FirstOrDefault().OrderDate))
                    .OrderBy(v => v.OrderDate.Year)
                    .ThenBy(v => v.OrderDate.Month)
                    .ThenByDescending(v => v.c.Orders.Sum(o => o.Total))
                    .ThenBy(v => v.c.CompanyName);
        }

        public static IEnumerable<Customer> Linq6(IEnumerable<Customer> customers)
        {
            return customers.Where((c) =>
            {
                return c.PostalCode.Any((c) => !char.IsDigit(c))
                    || c.Region == null
                    || (!c.Phone.Contains("(") || !c.Phone.Contains(")"));
            });
        }

        public static IEnumerable<Linq7CategoryGroup> Linq7(IEnumerable<Product> products)
        {
            /* example of Linq7result

             category - Beverages
	            UnitsInStock - 39
		            price - 18.0000
		            price - 19.0000
	            UnitsInStock - 17
		            price - 18.0000
		            price - 19.0000
             */

            return products.GroupBy(p => p.Category)
                    .Select(g =>
                    {
                        return new Linq7CategoryGroup()
                        {
                            Category = g.Key,
                            UnitsInStockGroup = g.GroupBy(g => g.UnitsInStock)
                                                 .Select((g) =>
                                                    {
                                                        return new Linq7UnitsInStockGroup()
                                                        {
                                                            UnitsInStock = g.Key,
                                                            Prices = g.Select(p => p.UnitPrice).OrderBy(price => price)
                                                        };
                                                    })
                        };
                    });


            throw new NotImplementedException();
        }

        public static IEnumerable<(decimal category, IEnumerable<Product> products)> Linq8(
            IEnumerable<Product> products,
            decimal cheap,
            decimal middle,
            decimal expensive
        )
        {
            return products.GroupBy((p) =>
            {
                if (p.UnitPrice <= cheap)
                {
                    return cheap;
                }

                if (p.UnitPrice <= middle)
                {
                    return middle;
                }

                return expensive;
            }).Select(g => (g.Key, g.AsEnumerable()));
        }

        public static IEnumerable<(string city, int averageIncome, int averageIntensity)> Linq9(
            IEnumerable<Customer> customers
        )
        {
            return customers.GroupBy(c => c.City)
                    .Select(g =>
                        (
                            g.Key,
                            (int)Math.Round(g.Average(c => c.Orders.Sum(o => o.Total))),
                            (int)g.Average(c => c.Orders.Count())
                        )
            );
        }

        public static string Linq10(IEnumerable<Supplier> suppliers)
        {
            return suppliers.Select(s => s.Country)
                    .Distinct()
                    .OrderBy(c => c.Length)
                    .ThenBy(c => c)
                    .Aggregate("", (acc, c) => acc + c);
        }
    }
}