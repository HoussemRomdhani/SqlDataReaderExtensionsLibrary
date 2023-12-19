using BenchmarkDotNet.Running;

namespace SqlDataReaderExtensionsShell
{
    internal class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<DataRead>();
        }
    }

    //internal class Program
    //{
    //    const string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=UsersDB;Trusted_Connection=True;";
    //    static async Task Main(string[] args)
    //    {
    //        var x = await new DataRead().ReadWithSourceGenerator();
    //    }
    //}
}
