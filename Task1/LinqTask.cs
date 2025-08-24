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
            return from customer in customers
                   select (
                    customer,
                    from supplier in suppliers
                    where (supplier.Country, supplier.City) == (customer.Country, customer.City)
                    select supplier
                   );
        }

        public static IEnumerable<(Customer customer, IEnumerable<Supplier> suppliers)> Linq2UsingGroup(
            IEnumerable<Customer> customers,
            IEnumerable<Supplier> suppliers
        )
        {
            return customers.GroupJoin(
                suppliers,
                (c) => (c.Country, c.City),
                (s) => (s.Country, s.City),
                (c, s) => (c, s)
            );
        }

        public static IEnumerable<Customer> Linq3(IEnumerable<Customer> customers, decimal limit)
        {
            return from customer in customers
                   let totalOrdersSum = customer.Orders.Sum(o => o.Total)
                   where totalOrdersSum > limit && totalOrdersSum != 0
                   select customer;
        }

        public static IEnumerable<(Customer customer, DateTime dateOfEntry)> Linq4(
            IEnumerable<Customer> customers
        )
        {
            return customers
                    .Where(c => c.Orders.Count() > 0)
                    .Select(c => (c, c.Orders.First().OrderDate));
        }

        public static IEnumerable<(Customer customer, DateTime dateOfEntry)> Linq5(
            IEnumerable<Customer> customers
        )
        {
            return from customer in customers
                   where customer.Orders.Count() > 0
                   let firstOrder = customer.Orders.First()
                   orderby firstOrder.OrderDate.Year,
                           firstOrder.OrderDate.Month,
                           customer.Orders.Sum(o => o.Total) descending,
                           customer.CompanyName
                   select (customer, firstOrder.OrderDate);
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
            return from product in products
                   group product by product.Category into productsByCategoryGroup
                   select new Linq7CategoryGroup()
                   {
                       Category = productsByCategoryGroup.Key,
                       UnitsInStockGroup
                        = from product in productsByCategoryGroup
                          group product by product.UnitsInStock into productsByUnitsInStockGroup
                          select new Linq7UnitsInStockGroup()
                          {
                              UnitsInStock = productsByUnitsInStockGroup.Key,
                              Prices = from product in productsByUnitsInStockGroup
                                       orderby product.UnitPrice
                                       select product.UnitPrice
                          }
                   };
        }

        public static IEnumerable<(decimal category, IEnumerable<Product> products)> Linq8(
            IEnumerable<Product> products,
            decimal cheap,
            decimal middle,
            decimal expensive
        )
        {
            return products
                .GroupBy((p) =>
                    {
                        if (p.UnitPrice <= cheap) return cheap;
                        if (p.UnitPrice <= middle) return middle;

                        return expensive;
                    })
                .Select(g => (g.Key, g.AsEnumerable()));
        }

        public static IEnumerable<(string city, int averageIncome, int averageIntensity)> Linq9(
            IEnumerable<Customer> customers
        )
        {
            return from customer in customers
                   group customer by customer.City into customerGroup
                   select
                    (
                        customerGroup.Key,
                        (int)Math.Round(customerGroup.Average(c => c.Orders.Sum(o => o.Total))),
                        (int)customerGroup.Average(c => c.Orders.Count())
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