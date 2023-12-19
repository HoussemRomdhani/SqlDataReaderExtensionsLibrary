using BenchmarkDotNet.Attributes;
using Dapper;
using Microsoft.Data.SqlClient;
using SqlDataReaderExtensions;
using System.Data;
using MapDataReader;

namespace SqlDataReaderExtensionsShell;

[MemoryDiagnoser]
public class DataRead
{
    public const string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=UsersDB;Trusted_Connection=True;";
    public const string sql = "SELECT TOP 100 [Id],[FirstName],[LastName],[Bio],[AccountCreatedAt],[DateOfBirth] ,[Active] ,[Email],[Followers] ,[Following],[Balance] FROM [UsersDB].[dbo].[User]";

    [Benchmark]
    public async Task<List<User>> ReadUsingClassicMethod()
    {
        List<User> result = new();

        using (var connection = new SqlConnection(connectionString))
        {
            await connection.OpenAsync();

            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = sql;

            SqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.Add(DataReaderToObject(reader));
            }
        }

        return result;
    }


    [Benchmark]
    public async Task<List<User>> ReadUsingDapper()
    {

        List<User> result = new();

        using (var connection = new SqlConnection(connectionString))
        {
            await connection.OpenAsync();
            var enumerable = await connection.QueryAsync<User>(sql);
            result = enumerable.AsList();
        }

        return result;
    }

    [Benchmark]
    public async Task<List<User>> ReadUsingSourceGenerator()
    {
        List<User> result = new();

        using (var connection = new SqlConnection(connectionString))
        {
            await connection.OpenAsync();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = sql;

            var reader = await command.ExecuteReaderAsync();

            result = reader.ToUser();
        }

        return result;
    }

    [Benchmark]
    public async Task<List<User>> ReadUsingReflection()
    {
        List<User> result = new();

        using (var connection = new SqlConnection(connectionString))
        {
            await connection.OpenAsync();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = sql;

            var reader = await command.ExecuteReaderAsync();

            result = reader.ReadListReflectionAsync<User>();
        }

        return result;
    }

    private static User DataReaderToObject(IDataReader reader)
    {
        int Id = reader["Id"] != DBNull.Value ? (int)reader["Id"] : 0;
        string FirstName = reader["FirstName"] != DBNull.Value ? (string)reader["FirstName"] : "";
        string LastName = reader["LastName"] != DBNull.Value ? (string)reader["LastName"] : "";
        string? Bio = reader["Bio"] != DBNull.Value ? (string)reader["Bio"] : null;
        DateTime AccountCreatedAt = reader["AccountCreatedAt"] != DBNull.Value ? (DateTime)reader["AccountCreatedAt"] : DateTime.Now;
        DateTime? DateOfBirth = reader["DateOfBirth"] != DBNull.Value ? (DateTime)reader["DateOfBirth"] : null;
        bool Active = reader["Active"] != DBNull.Value ? (bool)reader["Active"] : false;
        string Email = reader["Email"] != DBNull.Value ? (string)reader["Email"] : "";
        int Followers = reader["Followers"] != DBNull.Value ? (int)reader["Followers"] : default;
        int Following = reader["Following"] != DBNull.Value ? (int)reader["Following"] : default;
        decimal Balance = reader["Balance"] != DBNull.Value ? (decimal)reader["Balance"] : default;
        return new User
        {
            Id = Id,
            FirstName = FirstName,
            LastName = LastName,
            Bio = Bio,
            AccountCreatedAt = AccountCreatedAt,
            DateOfBirth = DateOfBirth,
            Active = Active,
            Email = Email,
            Followers = Followers,
            Following = Following,
            Balance = Balance
        };
    }
}
