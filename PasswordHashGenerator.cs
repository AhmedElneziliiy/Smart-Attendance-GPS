// Temporary Password Hash Generator
// Copy this into a simple console app or run it in LINQPad

using Microsoft.AspNetCore.Identity;

public class Program
{
    public static void Main()
    {
        // Create a password hasher
        var hasher = new PasswordHasher<object>();

        // Generate hash for your new password
        string newPassword = "Admin@123";  // Change this to your desired password
        string hash = hasher.HashPassword(null, newPassword);

        Console.WriteLine("Password Hash for: " + newPassword);
        Console.WriteLine(hash);
        Console.WriteLine("\nUse this hash in the SQL UPDATE statement below:");
        Console.WriteLine($"UPDATE AspNetUsers SET PasswordHash = '{hash}' WHERE Email = 'admin@example.com';");
    }
}

// To run this:
// 1. Create a new console app: dotnet new console -n PasswordHasher
// 2. Add Identity package: dotnet add package Microsoft.AspNetCore.Identity
// 3. Copy this code to Program.cs
// 4. Run: dotnet run
