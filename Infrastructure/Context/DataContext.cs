using System;
using Npgsql;

namespace Infrastructure.Context;

public class DataContext
{
    private const string connectionString = @"
    Host=localhost;
    Port=5432;
    Username=postgres;
    Database=workspace_db;
    Password=Ismoil10";

    public NpgsqlConnection GetConnection()
    {
        return new NpgsqlConnection(connectionString);
    }
}
