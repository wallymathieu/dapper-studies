using System;
using System.IO;
using SomeBasicDapperApp.Core;
using System.Linq;
using SomeBasicDapperApp.Tests.NH;
using Xunit;

namespace SomeBasicDapperApp.Tests
{
    public class CustomerDataTests
    {
        private static Lazy<Db.Factory> _sessionFactory = new Lazy<Db.Factory>(CreateFactory);

        [Fact]
        public void CanGetCustomerById()
        {
            using (var session = _sessionFactory.Value.OpenSession())
                Assert.NotNull(session.GetCustomer(1));
        }

        [Fact]
        public void CanGetProductById()
        {
            using (var session = _sessionFactory.Value.OpenSession())
                Assert.NotNull(session.GetProduct(1));
        }

        [Fact]
        public void CanGetCustomerByFirstname()
        {
            using (var session = _sessionFactory.Value.OpenSession())
            {
                var customers = session.GetCustomersWithFirstname("Steve");
                Assert.Equal(3, customers.Count());
            }
        }

        [Fact]
        public void OrderContainsProduct()
        {
            using (var session = _sessionFactory.Value.OpenSession())
                Assert.True(session.GetOrderProducts(1).Any(p => p.Id == 1));
        }

        [Fact]
        public void OrderHasACustomer()
        {
            using (var session = _sessionFactory.Value.OpenSession())
                Assert.NotNull(session.GetCustomerForOrder(1));
        }

        private static Db.Factory CreateFactory()
        {
            if (File.Exists("CustomerDataTests.db"))
            {
                File.Delete("CustomerDataTests.db");
            }

            new Migrator("CustomerDataTests.db").Migrate();
            new SaveTestData().Save();
            return new Db(new ConsoleMapPath()).CreateTestSessionFactory("CustomerDataTests.db");
        }
    }
}