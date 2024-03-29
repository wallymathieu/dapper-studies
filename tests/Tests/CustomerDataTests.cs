﻿using System;
using System.IO;
using SomeBasicDapperApp.Core;
using System.Linq;
using SomeBasicDapperApp.Tests.NH;
using Xunit;
using System.Threading.Tasks;
using SomeBasicDapperApp.Tests.Sqlite;

namespace SomeBasicDapperApp.Tests
{
    public class CustomerDataTests
    {
        private static Lazy<DbFactory> _sessionFactory = new Lazy<DbFactory>(CreateFactory);

        [Fact]
        public async Task CanGetCustomerByIdAsync()
        {
            using (var session = _sessionFactory.Value.OpenSession())
                Assert.NotNull(await session.GetCustomer(1));
        }

        [Fact]
        public async Task CanGetProductByIdAsync()
        {
            using (var session = _sessionFactory.Value.OpenSession())
                Assert.NotNull(await session.GetProduct(1));
        }

        [Fact]
        public async Task CanGetCustomerByFirstnameAsync()
        {
            using (var session = _sessionFactory.Value.OpenSession())
            {
                var customers = await session.GetCustomersWithFirstname("Steve");
                Assert.Equal(3, customers.Count());
            }
        }

        [Fact]
        public async Task OrderContainsProductAsync()
        {
            using (var session = _sessionFactory.Value.OpenSession())
                Assert.True((await session.GetOrderProducts(1)).Any(p => p.Id == 1));
        }

        [Fact]
        public async Task OrderHasACustomerAsync()
        {
            using (var session = _sessionFactory.Value.OpenSession())
                Assert.NotNull(await session.GetCustomerForOrder(1));
        }

        private static DbFactory CreateFactory()
        {
            if (File.Exists("CustomerDataTests.db"))
                File.Delete("CustomerDataTests.db");

            var factory = new DbFactory("CustomerDataTests.db");
            new Migrator("CustomerDataTests.db").Migrate();
            new SaveTestData(factory).Save().GetAwaiter().GetResult();
            return factory;
        }
    }
}