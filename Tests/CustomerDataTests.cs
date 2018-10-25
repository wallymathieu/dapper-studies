using System;
using System.IO;
using SomeBasicDapperApp.Core;
using System.Linq;
using SomeBasicDapperApp.Tests.NH;
using Xunit;

namespace SomeBasicDapperApp.Tests
{
	public class CustomerDataTests:IDisposable
	{
		private static Db.Factory _sessionFactory;
		private Db.Session _session;

		[Fact]
		public void CanGetCustomerById()
		{
			Assert.NotNull(_session.GetCustomer(1));
		}

		[Fact]
		public void CanGetProductById()
		{
			Assert.NotNull(_session.GetProduct(1));
		}

		[Fact]
		public void CanGetCustomerByFirstname()
		{
			var customers = _session.GetCustomersWithFirstname("Steve");
			Assert.Equal(3, customers.Count());
		}

		[Fact]
		public void OrderContainsProduct()
		{
			Assert.True(_session.GetOrderProducts(1).Any(p => p.Id == 1));
		}
		[Fact]
		public void OrderHasACustomer()
		{
			Assert.NotNull(_session.GetCustomerForOrder(1));
		}

		public CustomerDataTests()
		{
			_session = _sessionFactory.OpenSession();
		}


		public void Dispose()
		{
			if (_session != null)
			{
				_session.Close();
				_session.Dispose();
			}
		}

		static CustomerDataTests()
		{
			if (File.Exists("CustomerDataTests.db")) { File.Delete("CustomerDataTests.db"); }

			new Migrator("CustomerDataTests.db").Migrate();
			new SaveTestData().Save();
			_sessionFactory = new Db(new ConsoleMapPath()).CreateTestSessionFactory("CustomerDataTests.db");
		}

	}
}
