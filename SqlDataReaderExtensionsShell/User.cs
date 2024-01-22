using MapDataReader;

namespace SqlDataReaderExtensionsShell;

public record User(int Id, 
                   string FirstName,
                   string LastName,
                   string? Bio,
                   DateTime AccountCreatedAt, 
                   DateTime? DateOfBirth,
                   bool Active, 
                   string Email, 
                   int Followers, 
                   int Following,
                   decimal Balance);
