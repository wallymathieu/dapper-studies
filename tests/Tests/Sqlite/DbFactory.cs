using System;
using System.Data.SQLite;
using System.IO;
using SomeBasicDapperApp.Core;

namespace SomeBasicDapperApp.Tests.Sqlite;

public class DbFactory : IDisposable
{
    private readonly string file;

    public DbFactory(string file)
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