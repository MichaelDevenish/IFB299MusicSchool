using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace terminalHash
{
    class Program
    {
        /// <summary>
        /// DO NOT USE TIS YET DUE TO THE GETSTRING FUNCTION IGNORES SOME PARTS OF THE BYTE ARRAY, ILL HAVE TO CREATE A FUNCTION THAT DIRECTLY INTERFACES WITH THE DATABASE
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            bool loop = true;

            while (loop)
            {
                DatabaseConnector.DatabaseConnector database = new DatabaseConnector.DatabaseConnector();
                PasswordManagment.PasswordManagment password = new PasswordManagment.PasswordManagment();

                Console.Write("Please enter the users first name:");
                string first = Console.ReadLine();

                Console.Write("Please enter the users last name:");
                string last = Console.ReadLine();

                Console.Write("Please enter the users password:");
                string result = Console.ReadLine();

                string salt = password.GenerateSalt();
                byte[] hash = password.GenerateHash(result, salt);

                int role = 2;
                Console.Write("What is the Role of the user (0 = admin, 1 = teacher, 2 = student):");
                string rolestring = Console.ReadLine();
                if (rolestring == "0")
                    role = 0;
                if (rolestring == "1")
                    role = 1;
                if (rolestring == "2")
                    role = 2;

                Console.Write("What is the users date of birth (Formatted as dd/M/yyyy):");
                DateTime dob = DateTime.ParseExact(Console.ReadLine(), "dd/M/yyyy", CultureInfo.InvariantCulture);

                Console.Write("Is the data entered correctly (y/n)?");
                if (Console.ReadKey().KeyChar == 'y')
                    database.ExampleWriteDatabaseClass(first, last, dob, role, hash, salt);

                Console.Write("Do you wish to do another password (y/n)?");
                if (Console.ReadKey().KeyChar == 'n')
                    loop = false;

                Console.WriteLine();
            }
            Console.Write("Press any key to exit");
            Console.ReadKey();
        }
    }
}
