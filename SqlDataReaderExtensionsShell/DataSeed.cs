using Bogus;
using Microsoft.Data.SqlClient;
using System.Data;

namespace SqlDataReaderExtensionsShell;

public static class DataSeed
{
    public async static Task Seed(string connectionString, int count)
    {
        var users = GetRandomUsers(count);

        await InserUsers(users, connectionString);
    }

    private static async Task InserUsers(IList<User> users, string connectionString)
    {
        using (var connection = new SqlConnection(connectionString))
        {
            await connection.OpenAsync();

            foreach (var user in users)
            {
                await InserUser(user, connection);
            }
        }
    }

    private static async Task InserUser(User user, SqlConnection connection)
    {

        SqlCommand command = connection.CreateCommand();
        command.CommandType = CommandType.Text;

        command.Parameters.AddWithValue("@FirstName", user.FirstName);
        command.Parameters.AddWithValue("@LastName", user.LastName);
        command.Parameters.AddWithValue("@Bio", user.Bio != null ? user.Bio : DBNull.Value);
        command.Parameters.AddWithValue("@AccountCreatedAt", user.AccountCreatedAt);
        command.Parameters.AddWithValue("@DateOfBirth", user.DateOfBirth.HasValue ? user.DateOfBirth.Value : DBNull.Value);
        command.Parameters.AddWithValue("@Active", user.Active);
        command.Parameters.AddWithValue("@Email", user.Email);
        command.Parameters.AddWithValue("@Followers", user.Followers);
        command.Parameters.AddWithValue("@Following", user.Following);
        command.Parameters.AddWithValue("@Balance", user.Balance);


        command.CommandText = "INSERT INTO [dbo].[User]([FirstName],[LastName],[Bio],[AccountCreatedAt],[DateOfBirth],[Active],[Email],[Followers],[Following],[Balance])  "
                            + " VALUES(@FirstName, @LastName, @Bio, @AccountCreatedAt, @DateOfBirth, @Active, @Email, @Followers, @Following, @Balance) ";

        await command.ExecuteNonQueryAsync();

    }

    private static List<User> GetRandomUsers(int count)
    {
        var userFaker = new Faker<User>()
                           .RuleFor(f => f.Bio, f => null)
                           .RuleFor(f => f.AccountCreatedAt, f => f.Date.Between(new DateTime(1975, 1, 1), new DateTime(2023, 1, 1)))
                           .RuleFor(f => f.DateOfBirth, f => f.Date.Between(new DateTime(1975, 1, 1), new DateTime(2023, 1, 1)))
                           .RuleFor(f => f.FirstName, f => f.Random.String2(1, 50))
                           .RuleFor(f => f.LastName, f => f.Random.String2(1, 50))
                           .RuleFor(f => f.Email, f => f.Random.String2(1, 30))
                           .RuleFor(f => f.Followers, f => f.Random.Int(1, 5000))
                           .RuleFor(f => f.Following, f => f.Random.Int(1, 5000))
                           .RuleFor(f => f.Balance, f => f.Random.Decimal(1, 600))
                           .RuleFor(f => f.Active, f => f.Random.Bool());

        return userFaker.Generate(count);
    }
}
