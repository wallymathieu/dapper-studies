using SomeBasicDapperApp.Tests.NH.Entities;
using System;
using System.IO;
using System.Xml.Linq;

namespace SomeBasicDapperApp.Tests.NH
{
	class SaveTestData
	{
		public void Save()
		{
			var doc = XDocument.Load(Path.Combine("TestData", "TestData.xml"));
			var import = new XmlImport(doc, "http://tempuri.org/Database.xsd");
			using (var hibernateSessionFactory = new HibernateSession().CreateTestSessionFactory("CustomerDataTests.db"))
			{
				using (var session = hibernateSessionFactory.OpenSession())
				using (var tnx = session.BeginTransaction())
				{
					import.Parse(new[] { typeof(Customer), typeof(Order), typeof(Product) },
									(type, obj) =>
									{
										session.Save(obj);
									}, onIgnore: (type, property) =>
									{
										Console.WriteLine("Not mapped: " + type.Name + " " + property.Name);
									});
					tnx.Commit();
				}
				using (var session = hibernateSessionFactory.OpenSession())
				using (var tnx = session.BeginTransaction())
				{
					import.ParseConnections("OrderProduct", "Product", "Order", (productId, orderId) =>
					{
						var product = session.Get<Product>(productId);
						var order = session.Get<Order>(orderId);
						order.Products.Add(product);
					});
					import.ParseIntProperty("Order", "Customer", (orderId, customerId) =>
					{
						session.Get<Order>(orderId).Customer = session.Get<Customer>(customerId);
					});

					tnx.Commit();
				}
			}

		}
	}
}
