using System.IO;
using System.Xml.Linq;
using NUnit.Framework;
using SomeBasicDapperApp.Core;
using System.Linq;
using System;
using SomeBasicDapperApp.Core.Entities;
using SomeBasicDapperApp.Tests.NH;

namespace SomeBasicDapperApp.Tests
{
	[TestFixture]
	public class CustomerDataTests
	{
		private Db.Factory _sessionFactory;
		private Db.Session _session;

		[Test]
		public void CanGetCustomerById()
		{
			Assert.IsNotNull(_session.GetCustomer(1));
		}

		[Test]
		public void CanGetProductById()
		{
			Assert.IsNotNull(_session.GetProduct(1));
		}

		[Test]
		public void CanGetCustomerByFirstname()
		{
			var customers = _session.GetCustomersWithFirstname("Steve");
			Assert.AreEqual(3, customers.Count());
		}

		[Test]
		public void OrderContainsProduct()
		{
			Assert.True(_session.GetOrderProducts(1).Any(p => p.Id == 1));
		}
		[Test]
		public void OrderHasACustomer()
		{
			Assert.IsNotNull(_session.GetCustomerForOrder(1));
		}

		[SetUp]
		public void Setup()
		{
			_session = _sessionFactory.OpenSession();
		}


		[TearDown]
		public void TearDown()
		{
			if (_session != null)
			{
				_session.Close();
				_session.Dispose();
			}
		}

		[TestFixtureSetUp]
		public void TestFixtureSetup()
		{
			if (File.Exists("CustomerDataTests.db")) { File.Delete("CustomerDataTests.db"); }

			new Migrator("CustomerDataTests.db").Migrate();
			new SaveTestData().Save();
			_sessionFactory = new Db(new ConsoleMapPath()).CreateTestSessionFactory("CustomerDataTests.db");
		}

		[TestFixtureTearDown]
		public void TestFixtureTearDown()
		{
			_sessionFactory.Dispose();
		}
	}
}
