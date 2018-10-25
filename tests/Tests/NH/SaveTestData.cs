using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using SomeBasicDapperApp.Core;

namespace SomeBasicDapperApp.Tests.NH
{
    class SaveTestData
    {
        private readonly Db.Factory _db;

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

        public SaveTestData(Db.Factory db)
        {
            _db = db;
        }

        public void Save()
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
                    import.Parse(new[] {typeof(Customer), typeof(Order), typeof(Product)},
                        (type, obj) =>
                        {
                            switch (obj)
                            {
                                case Customer c:
                                    session.CreateCustomer(c.Id, c.Firstname, c.Lastname, c.Version);
                                    break;
                                case Order o:
                                    session.CreateOrder(o.Id, o.OrderDate,
                                        orderCustomer.TryGetValue(o.Id, out var v) ? v : (int?) null,
                                        o.Version);
                                    break;
                                case Product p:
                                    session.CreateProduct(p.Id, p.Name, p.Cost, p.Version);
                                    break;
                                default: throw new Exception(obj.GetType().FullName);
                            }
                        },
                        onIgnore: (type, property) =>
                        {
                            Console.WriteLine("Not mapped: " + type.Name + " " + property.Name);
                        });
                    tnx.Commit();
                }

                using (var session = _db.OpenSession())
                using (var tnx = session.BeginTransaction())
                {
                    import.ParseConnections("OrderProduct", "Product", "Order",
                        (productId, orderId) => { session.AddProductToOrder(productId, orderId); });


                    tnx.Commit();
                }
            }
        }
    }
}