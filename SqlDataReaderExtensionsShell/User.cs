using MapDataReader;

namespace SqlDataReaderExtensionsShell;

[GenerateDataReaderMapper]
public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? Bio { get; set; }
    public DateTime AccountCreatedAt { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public bool Active { get; set; }
    public string Email { get; set; }
    public int Followers { get; set; }
    public int Following { get; set; }
    public decimal Balance { get; set; }
}
