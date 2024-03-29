﻿using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using SomeBasicDapperApp.Core.Entities;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SomeBasicDapperApp.Core
{

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

        public Task<IEnumerable<Customer>> GetCustomersWithFirstname(string v) =>
            _conn.QueryAsync<Customer>("SELECT c.* FROM Customers c WHERE c.FirstName = @FirstName",
                new { FirstName = v });

        public void Close() => _conn.Close();

        public Task<IEnumerable<Product>> GetOrderProducts(int v) =>
            _conn.QueryAsync<Product>(
                "SELECT p.* FROM OrdersToProducts otp JOIN Products p ON otp.Product_id WHERE otp.Order_Id = @OrderId",
                new { OrderId = v });

        public IDbTransaction BeginTransaction() =>
            _conn.BeginTransaction();

        public Task<Customer> GetCustomerForOrder(int v) =>
            _conn.QueryFirstOrDefaultAsync<Customer>(
                    "SELECT c.* FROM Orders o JOIN Customers c ON o.Customer_Id = c.Id WHERE o.Id = @Id",
                    new { Id = v });

        public Task<Product> GetProduct(int productId) =>
            _conn.QueryFirstOrDefaultAsync<Product>("SELECT p.* FROM Products p WHERE p.Id = @id", new { id = productId });

        public Task<Order> GetOrder(int orderId) =>
            _conn.QueryFirstOrDefaultAsync<Order>("SELECT o.* FROM Orders o WHERE p.Id = @id", new { id = orderId });

        public Task<Customer> GetCustomer(int v) =>
            _conn.QueryFirstOrDefaultAsync<Customer>("SELECT c.* FROM Customers c WHERE c.Id = @id", new { id = v });

        public Task CreateCustomer(int cId, string cFirstname, string cLastname, int cVersion) =>
            _conn.ExecuteAsync(
                @"insert into Customers(id, firstname, lastname, version) values (@id, @firstname, @lastname, @version)",
                new { id = cId, firstname = cFirstname, lastname = cLastname, version = cVersion }
            );

        public Task CreateOrder(int oId, DateTime oOrderDate, int? customerId, int oVersion) => _conn.ExecuteAsync(
            @"insert into Orders(id, orderDate, Customer_id, version) values (@id, @orderDate, @customerId, @version)",
            new { id = oId, orderDate = oOrderDate, customerId = customerId, version = oVersion }
        );

        public Task CreateProduct(int pId, string pName, float pCost, int pVersion) => _conn.ExecuteAsync(
            @"insert into Products(id, name, cost, version) values (@id, @name, @cost, @version)",
            new { id = pId, name = pName, cost = pCost, version = pVersion }
        );

        public Task AddProductToOrder(int productId, int orderId) => _conn.ExecuteAsync(
            @"insert into OrdersToProducts(Order_Id, Product_id) values (@orderId, @productId)",
            new[] { new { orderId, productId } }
        );
    }


}