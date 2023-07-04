﻿namespace LapkaBackend.Infrastructure.Database;

public class ConnectionStringBuilder
{
    private const string ServerName = "LAPTOP-B2ON02BS\\SQLEXPRESS";
    private const string DatabaseName = "main";
    private const string Username = "admin";
    private const string Password = "admin";

    public static string Build()
    {
        return $"Server={ServerName};Database={DatabaseName};User Id={Username};Password={Password};Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true";
    }
}