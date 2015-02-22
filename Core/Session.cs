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
		private IMapPath mapPath;

		public Db(IMapPath mapPath)
		{
			this.mapPath = mapPath;
		}

		public Factory CreateTestSessionFactory(string file)
		{
			return new Factory(mapPath, file);
		}

		public class Session : IDisposable
		{
			private IDbConnection conn;

			public Session(IDbConnection conn)
			{
				this.conn = conn;
				this.conn.Open();
			}

			public void Dispose()
			{
				this.conn.Dispose();
			}

			public IEnumerable<Customer> GetCustomersWithFirstname(string v)
			{
				return this.conn.Query<Customer>("SELECT c.* FROM Customers c WHERE c.FirstName = @FirstName", new { FirstName = v });
			}

			public void Close()
			{
				this.conn.Close();
			}

			public IEnumerable<Product> GetOrderProducts(int v)
			{
				return this.conn.Query<Product>("SELECT p.* FROM OrdersToProducts otp JOIN Products p ON otp.Product_id WHERE otp.Order_Id = @OrderId", new { OrderId = v });
			}

			public IDbTransaction BeginTransaction()
			{
				return this.conn.BeginTransaction();
			}

			public Customer GetCustomerForOrder(int v)
			{
				return this.conn.Query<Customer>("SELECT c.* FROM Orders o JOIN Customers c ON o.Customer_Id = c.Id WHERE o.Id = @Id", new { Id = v }).SingleOrDefault();
			}

			public Product GetProduct(int productId)
			{
				return this.conn.Query<Product>("SELECT p.* FROM Products p WHERE p.Id = @id", new { id = productId }).SingleOrDefault();
			}

			public Order GetOrder(int orderId)
			{
				return this.conn.Query<Order>("SELECT o.* FROM Orders o WHERE p.Id = @id", new { id = orderId }).SingleOrDefault();
			}

			public Customer GetCustomer(int v)
			{
				return this.conn.Query<Customer>("SELECT c.* FROM Customers c WHERE c.Id = @id", new { id = v }).SingleOrDefault();
			}
		}

		public class Factory : IDisposable
		{
			private string file;
			private IMapPath mapPath;

			public Factory(IMapPath mapPath, string file)
			{
				this.mapPath = mapPath;
				this.file = file;
			}
			public void Dispose()
			{
			}

			public Session OpenSession()
			{
				var conn = new SQLiteConnection();
				conn.ConnectionString = "Data Source=" + Path.Combine(Directory.GetCurrentDirectory(), file) + ";Version=3;";
				return new Session(conn);
			}
		}
	}
}
