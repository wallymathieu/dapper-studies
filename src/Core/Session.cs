using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using Dapper;
using SomeBasicDapperApp.Core.Entities;
using System.IO;
using System.Linq;

namespace SomeBasicDapperApp.Core
{
    public class Db
    {
        public Factory CreateTestSessionFactory(string file)
        {
            return new Factory(file);
        }

        public class Session : IDisposable
        {
            private readonly IDbConnection _conn;

            public Session(IDbConnection conn)
            {
                _conn = conn;
                _conn.Open();
            }

            public void Dispose()
            {
                _conn.Dispose();
            }

            public IEnumerable<Customer> GetCustomersWithFirstname(string v) =>
                _conn.Query<Customer>("SELECT c.* FROM Customers c WHERE c.FirstName = @FirstName",
                    new {FirstName = v});

            public void Close() => _conn.Close();

            public IEnumerable<Product> GetOrderProducts(int v) =>
                _conn.Query<Product>(
                    "SELECT p.* FROM OrdersToProducts otp JOIN Products p ON otp.Product_id WHERE otp.Order_Id = @OrderId",
                    new {OrderId = v});

            public IDbTransaction BeginTransaction() =>
                _conn.BeginTransaction();

            public Customer GetCustomerForOrder(int v) =>
                _conn.Query<Customer>(
                        "SELECT c.* FROM Orders o JOIN Customers c ON o.Customer_Id = c.Id WHERE o.Id = @Id",
                        new {Id = v})
                    .SingleOrDefault();

            public Product GetProduct(int productId) =>
                _conn.Query<Product>("SELECT p.* FROM Products p WHERE p.Id = @id", new {id = productId})
                    .SingleOrDefault();

            public Order GetOrder(int orderId) =>
                _conn.Query<Order>("SELECT o.* FROM Orders o WHERE p.Id = @id", new {id = orderId}).SingleOrDefault();

            public Customer GetCustomer(int v) =>
                _conn.Query<Customer>("SELECT c.* FROM Customers c WHERE c.Id = @id", new {id = v}).SingleOrDefault();

            public void CreateCustomer(int cId, string cFirstname, string cLastname, int cVersion) =>
                _conn.Execute(
                    @"insert into Customers(id, firstname, lastname, version) values (@id, @firstname, @lastname, @version)",
                    new {id = cId, firstname = cFirstname, lastname = cLastname, version = cVersion}
                );

            public void CreateOrder(int oId, DateTime oOrderDate, int? customerId, int oVersion) => _conn.Execute(
                @"insert into Orders(id, orderDate, Customer_id, version) values (@id, @orderDate, @customerId, @version)",
                new {id = oId, orderDate = oOrderDate, customerId = customerId, version = oVersion}
            );

            public void CreateProduct(int pId, string pName, float pCost, int pVersion) => _conn.Execute(
                @"insert into Products(id, name, cost, version) values (@id, @name, @cost, @version)",
                new {id = pId, name = pName, cost = pCost, version = pVersion}
            );

            public void AddProductToOrder(int productId, int orderId) => _conn.Execute(
                @"insert into OrdersToProducts(Order_Id, Product_id) values (@orderId, @productId)",
                new[] {new {orderId, productId}}
            );
        }

        public class Factory : IDisposable
        {
            private string file;

            public Factory(string file)
            {
                this.file = file;
            }

            public void Dispose()
            {
            }

            public Session OpenSession()
            {
                var conn = new SQLiteConnection();
                conn.ConnectionString =
                    "Data Source=" + Path.Combine(Directory.GetCurrentDirectory(), file) + ";Version=3;";
                return new Session(conn);
            }
        }
    }
}