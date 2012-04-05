using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestClient.RHIT.Private;

namespace TestClient
{
    class Program
    {
        private static string token;
        private static IWebService service = new WebServiceClient();

        static void Main(string[] args)
        {
            Console.Write("Username: ");
            string username = Console.ReadLine();
            Console.Write("Password: ");
            string password = ReadPassword();

            var auth = service.Login(username, password);

            if (auth == null)
            {
                Console.WriteLine("Unable to login");
                Environment.Exit(0);
            }
            token = auth.Token;

            int option;
            do
            {
                Console.WriteLine("1. Search Users");
                Console.WriteLine("2. Lookup User");
                Console.WriteLine("9. Quit");
                Console.Write("> ");
                option = int.Parse(Console.ReadLine());

                switch (option)
                {
                    case 1:
                        SearchUser();
                        break;
                    case 2:
                        LookupUser();
                        break;
                }

            } while (option != 9);
        }

        static void SearchUser()
        {
            Console.Write("Search: ");
            string search = Console.ReadLine();
            var users = service.SearchUsers(token, search);

            foreach (var user in users)
                PrintUser(user);
        }

        static void LookupUser()
        {
            Console.Write("Username: ");
            string username = Console.ReadLine();
            var user = service.GetUser(token, username);
            if (user == null)
            {
                Console.WriteLine("User not found");
                return;
            }

            PrintUser(user);
        }

        static void PrintUser(User user)
        {
            Console.WriteLine("Username:    {0}", user.Username);
            Console.WriteLine("Alias:       {0}", user.Alias);
            Console.WriteLine("Mailbox:     {0}", user.Mailbox);
            Console.WriteLine("Major:       {0}", user.Major);
            Console.WriteLine("Class:       {0}", user.Class);
            Console.WriteLine("Year:        {0}", user.Year);
            Console.WriteLine("Name:        {0} {1} {2}", user.FirstName, user.MiddleName, user.LastName);
            Console.WriteLine("Deptartment: {0}", user.Department);
            Console.WriteLine("Phone:       {0}", user.Phone);
            Console.WriteLine("Room:        {0}", user.Room);
            Console.WriteLine();
        }

        static string ReadPassword()
        {
            string password = "";
            ConsoleKeyInfo info = Console.ReadKey(true);
            while (info.Key != ConsoleKey.Enter)
            {
                if (info.Key != ConsoleKey.Backspace)
                {
                    password += info.KeyChar;
                    info = Console.ReadKey(true);
                }
                else if (info.Key == ConsoleKey.Backspace)
                {
                    if (!string.IsNullOrEmpty(password))
                    {
                        password = password.Substring
                        (0, password.Length - 1);
                    }
                    info = Console.ReadKey(true);
                }
            }
            //for (int i = 0; i < password.Length; i++)
            //    Console.Write("*");
            Console.WriteLine();
            return password;
        }

    }
}
