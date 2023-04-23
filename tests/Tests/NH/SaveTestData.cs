using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using SomeBasicDapperApp.Core;
using SomeBasicDapperApp.Tests.Sqlite;

namespace SomeBasicDapperApp.Tests.NH
{
    class SaveTestData
    {
        private readonly DbFactory _db;

        private class Customer
        {
            public virtual int Id { get; set; }

            public virtual string Firstname { get; set; }

            public virtual string Lastname { get; set; }

            public virtual int Version { get; set; }
        }

        private class Order
        {
            public virtual int CustomerId { get; set; }

            public virtual DateTime OrderDate { get; set; }

            public virtual int Id { get; set; }

            public virtual int Version { get; set; }
        }

        private class Product
        {
            public virtual float Cost { get; set; }

            public virtual string Name { get; set; }

            public virtual int Id { get; set; }

            public virtual int Version { get; set; }
        }

        public SaveTestData(DbFactory db)
        {
            _db = db;
        }

        public async Task Save()
        {
            var doc = XDocument.Load(Path.Combine("TestData", "TestData.xml"));
            var import = new XmlImport(doc, "http://tempuri.org/Database.xsd");
            {
                using (var session = _db.OpenSession())
                using (var tnx = session.BeginTransaction())
                {
                    var orderCustomer = new Dictionary<int, int>();
                    import.ParseIntProperty("Order", "Customer",
                        (orderId, customerId) => { orderCustomer.Add(orderId, customerId); });
                    foreach (var (type,obj) in import.Parse(new[] {typeof(Customer), typeof(Order), typeof(Product)},
                        onIgnore: (type, property) =>
                        {
                            Console.WriteLine("Not mapped: " + type.Name + " " + property.Name);
                        }))
                    {
                        switch (obj)
                        {
                            case Customer c:
                                await session.CreateCustomer(c.Id, c.Firstname, c.Lastname, c.Version);
                                break;
                            case Order o:
                                await session.CreateOrder(o.Id, o.OrderDate,
                                    orderCustomer.TryGetValue(o.Id, out var v) ? v : (int?) null,
                                    o.Version);
                                break;
                            case Product p:
                                await session.CreateProduct(p.Id, p.Name, p.Cost, p.Version);
                                break;
                            default: throw new Exception(obj.GetType().FullName);
                        }
                    }
                    tnx.Commit();
                }

                using (var session = _db.OpenSession())
                using (var tnx = session.BeginTransaction())
                {
                    foreach (var (productId, orderId) in import.ParseConnections("OrderProduct", "Product", "Order"))
                    {
                        await session.AddProductToOrder(productId, orderId);
                    }
                    tnx.Commit();
                }
            }
        }
    }
}