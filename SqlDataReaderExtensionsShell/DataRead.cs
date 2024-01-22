using BenchmarkDotNet.Attributes;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace SqlDataReaderExtensionsShell;

[MemoryDiagnoser]
public class DataRead
{
    public const string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=UsersDB;Trusted_Connection=True;";
    public const string sql = "SELECT TOP 100 [Id],[FirstName],[LastName],[Bio],[AccountCreatedAt],[DateOfBirth] ,[Active] ,[Email],[Followers] ,[Following],[Balance] FROM [UsersDB].[dbo].[User]";
    public const string sqlSingle = "SELECT [Id],[FirstName],[LastName],[Bio],[AccountCreatedAt],[DateOfBirth] ,[Active] ,[Email],[Followers] ,[Following],[Balance] FROM [UsersDB].[dbo].[User] WHERE Id = @Id";

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
            result = (await connection.QueryAsync<User>(sql)).AsList();
        }

        return result;
    }


    [Benchmark]
    public async Task<User?> ReadSingleUsingClassicMethod()
    {
        User? result = null;

        using (var connection = new SqlConnection(connectionString))
        {
            await connection.OpenAsync();

            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = sqlSingle;

            command.Parameters.AddWithValue("@Id", -1);

            SqlDataReader reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                result = DataReaderToObject(reader);
            }
        }

        return result;
    }



    [Benchmark]
    public async Task<User?> ReadSignleUsingDapper()
    {
        User? result;

        using (var connection = new SqlConnection(connectionString))
        {
            await connection.OpenAsync();

            result = await connection.QueryFirstOrDefaultAsync<User>(sqlSingle, new { Id = -1 });
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
        return new User(Id, FirstName, LastName, Bio, AccountCreatedAt, DateOfBirth, Active, Email, Followers, Following, Balance);
    }
}
