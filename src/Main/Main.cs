// using System;
// using BCrypt.Net;

// class Program
// {
//     static void Main()
//     {
//         string password = "Secure123!";
//         string hash = BCrypt.Net.BCrypt.HashPassword(password);

//         Console.WriteLine($"Password Hash: {hash}");

//         bool isMatch = BCrypt.Net.BCrypt.Verify(password, hash);
//         Console.WriteLine($"Password Verified: {isMatch}");
//     }
// }
// // 