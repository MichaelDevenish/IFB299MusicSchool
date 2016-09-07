using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace terminalHash
{
    /// <summary>
    /// This function is created to help with database input for the users table
    /// </summary>
    class Program
    {
        /// <summary>
        /// The constructor, handels everything
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            bool loop = true;

            while (loop)
            {
                DatabaseConnector.DatabaseConnector database = new DatabaseConnector.DatabaseConnector();
                PasswordManagment.PasswordManagment password = new PasswordManagment.PasswordManagment();

                string first = RequestData("Please enter the users first name:");
                string last = RequestData("Please enter the users last name:");
                string result = RequestData("Please enter the users password:");

                string salt = password.GenerateSalt();
                byte[] hash = password.GenerateHash(result, salt);

                int role = EnterRole();
                DateTime dob = EnterDOB();

                if (RequestBinaryCondition("Is the data entered correctly (y/n)?", 'y', 'n'))
                    database.ExampleWriteDatabaseClass(first, last, dob, role, hash, salt);
                Console.WriteLine();

                loop = RequestBinaryCondition("Do you wish to do another password (y/n)?", 'y', 'n');
                Console.WriteLine();
            }
            Console.Write("Press any key to exit");
            Console.ReadKey();
        }

        /// <summary>
        /// Requests a binary condition from the user and Deals with the error handeling 
        /// </summary>
        /// <param name="message">the starting message</param>
        /// <param name="cond1">the yes condition</param>
        /// <param name="cond2">the no condition</param>
        /// <returns></returns>
        private static bool RequestBinaryCondition(string message, char cond1, char cond2)
        {
            Console.Write(message);
            while (true)
            {
                char result = Console.ReadKey().KeyChar;
                if (result == cond1) return true;
                else if (result == cond2) return false;
                else Console.Write(result + " is an invalid Input, please try again:");
            }
        }

        /// <summary>
        /// Prompts the user to give data
        /// </summary>
        /// <param name="message">The starting message</param>
        /// <returns>the result</returns>
        private static string RequestData(string message)
        {
            Console.Write(message);
            string result = Console.ReadLine();
            return result;
        }

        /// <summary>
        /// Prompts the user to enter their date of birth and then returns their input as a 
        /// DateTime if it is correct, else it loops
        /// </summary>
        /// <returns>the users DOB represented as a DateTime</returns>
        private static DateTime EnterDOB()
        {
            DateTime dob = new DateTime();
            bool loop = false;

            Console.Write("What is the users date of birth (Formatted as dd/MM/yyyy):");
            while (!loop)
            {
                loop = DateTime.TryParseExact(Console.ReadLine(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dob);
                if (!loop) Console.Write("The date was not entered correctly, please try again:");
            }
            return dob;
        }

        /// <summary>
        /// Prompts the user to enter their role and then returns their input if it is correct,
        /// else it loops
        /// </summary>
        /// <returns>the user role represented by a 0,1 or 2</returns>
        private static int EnterRole()
        {
            int role = 3;
            bool roleLoop = true;
            Console.Write("What is the Role of the user (0 = admin, 1 = teacher, 2 = student):");
            while (roleLoop)
            {
                roleLoop = false;
                string rolestring = Console.ReadLine();
                if (rolestring == "0")
                    role = 0;
                else if (rolestring == "1")
                    role = 1;
                else if (rolestring == "2")
                    role = 2;
                else {
                    Console.Write(rolestring + " is not a valid role, please try again:");
                    roleLoop = true;
                }
            }

            return role;
        }
    }
}
