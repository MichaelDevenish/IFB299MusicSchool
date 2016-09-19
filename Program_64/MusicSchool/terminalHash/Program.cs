using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApplication1;

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
            PasswordManagment passTest = new PasswordManagment(2);
            byte[] output = passTest.GenerateHash("password", passTest.GenerateSalt());
            foreach (var item in output)
            {
                Debug.Write(item.ToString() + ",");
            }
            bool loop = true;
            Console.WriteLine("Welcome");

            while (loop)
            {


                Console.WriteLine("What would you like to do? ");
                Console.WriteLine("1 = add user ");
                Console.WriteLine("2 = add instrument");
                Console.WriteLine("3 = search users ");
                Console.WriteLine("4 = search instruments");
                Console.WriteLine("5 = delete user ");
                Console.WriteLine("6 = delete instrument ");
                string task = Console.ReadLine();
                int performTask = Int32.Parse(task);

                DatabaseConnector.DatabaseConnector database = new DatabaseConnector.DatabaseConnector();
                PasswordManagment password = new PasswordManagment();

                if (performTask == 1)
                {
                    addUser(loop, database, password);
                }
                else if (performTask == 2)
                {
                    AddInstrument(loop, database);
                }
                else if (performTask == 3)
                {
                    SearchUser(loop, database);
                }
                else if (performTask == 4)
                {
                    SearchInstrument(loop, database);
                }
                else if (performTask == 5)
                {
                    deleteUser(loop, database);
                }
                else if (performTask == 6)
                {
                    deleteInstrument(loop, database);
                }

            }
            Console.Write("Press any key to exit");
            Console.ReadKey();
        }

        //#####################################//

        private static Boolean AddInstrument(Boolean loop, DatabaseConnector.DatabaseConnector database)
        {
            string instrumentName = RequestData("Please enter the instruments name: ");
            string instrumentType = RequestData("Please enter the instruments type, eg; string: ");
            string instrumentQuality = RequestData("Please briefly describe the instruments condition: ");

            if (RequestBinaryCondition("Is the data entered correctly (y/n)?", 'y', 'n'))
                database.InsertInstrument(instrumentQuality, instrumentName, instrumentType);
            Console.WriteLine();

            Console.ReadKey();
            loop = RequestBinaryCondition("Do you wish to do another intstrument (y/n)?", 'y', 'n');
            Console.WriteLine();

            return loop;
        }

        private static Boolean SearchInstrument(Boolean loop, DatabaseConnector.DatabaseConnector database)
        {
            int id = Int32.Parse(RequestData("Please enter the instrument's ID: "));
            string instrumentName = RequestData("Please enter the instrument's name: ");
            string instrumentType = RequestData("Please enter the insturment's type: ");

            DataTable dt = database.getInstrument(id, instrumentName, instrumentType);

            string quality = "";
            foreach (DataRow row in dt.Rows)
            {
                id = row.Field<int>(0);
                instrumentName = row.Field<string>(1);
                quality = row.Field<string>(2);
                instrumentType = row.Field<string>(3);

                Console.WriteLine(id + ", " + instrumentName + ", " + quality + ", " + instrumentType);
            }

            return loop;
        }

        private static Boolean addUser(Boolean loop, DatabaseConnector.DatabaseConnector database, PasswordManagment password)
        {
            string first = RequestData("Please enter the users first name:");
            string last = RequestData("Please enter the users last name:");
            string result = RequestData("Please enter the users password:");

            string username = RequestUsername(database);

            string salt = password.GenerateSalt();
            byte[] hash = password.GenerateHash(result, salt);

            int role = EnterRole();
            DateTime dob = EnterDOB();

            if (RequestBinaryCondition("Is the data entered correctly (y/n)?", 'y', 'n'))
                database.InsertUser(first, last, dob, role, hash, salt, username);
            Console.WriteLine();

            Console.ReadKey();
            loop = RequestBinaryCondition("Do you wish to do another user (y/n)?", 'y', 'n');
            Console.WriteLine();

            return loop;
        }

        private static Boolean SearchUser(Boolean loop, DatabaseConnector.DatabaseConnector database)
        {
            int id = Int32.Parse(RequestData("Please enter the users ID: "));
            string username = RequestUsername(database);
            string firstName = RequestData("Please enter the user's name: ");
            string lastName = RequestData("Please enter the user's last name: ");

            DataTable dt = database.getUser(id, username, firstName, lastName);

            byte[] password = null;
            DateTime dob = new DateTime();
            int role = 0;

            foreach (DataRow row in dt.Rows)
            {
                id = row.Field<int>(0);
                firstName = row.Field<string>(1);
                lastName = row.Field<string>(2);
                dob = row.Field<DateTime>(3);
                role = row.Field<int>(4);
                password = row.Field<byte[]>(5);
                username = row.Field<string>(7);

                Console.WriteLine(id + ", " + firstName + ", " + lastName + ", " + dob + ", " + role + ", " + password + ", " + username);
            }

            return loop;
        }

        private static Boolean deleteUser(Boolean loop, DatabaseConnector.DatabaseConnector database)
        {
            Boolean sure = false;
            int id = Int32.Parse(RequestData("Please enter the users ID: "));
            Console.ReadKey();
            sure = RequestBinaryCondition("Are you sure you wish to delete this user (y/n)?", 'y', 'n');
            if (sure)
            {
                database.DeleteUser(id);
            }
            else { }
            return loop;
        }

        private static Boolean deleteInstrument(Boolean loop, DatabaseConnector.DatabaseConnector database)
        {
            Boolean sure = false;
            int id = Int32.Parse(RequestData("Please enter the instrument ID: "));
            Console.ReadKey();
            sure = RequestBinaryCondition("Are you sure you wish to delete this instrument (y/n)?", 'y', 'n');
            if (sure)
            {
                database.DeleteInstrument(id);
            }
            else { }
            return loop;
        }


        /// <summary>
        /// under construction
        /// </summary>
        /// <param name="loop"></param>
        /// <param name="database"></param>
        /// <returns></returns>
        public Boolean editUser(Boolean loop, DatabaseConnector.DatabaseConnector database)
        {

            String firstName = "";
            String lastName = "";
            String username = "";
            int id = Int32.Parse(RequestData("Please enter the User ID of the User you wish to edit: "));
            DataTable dt = database.getUser(id, username, firstName, lastName);

            return loop;
        }

        //######################################//

        private static string RequestUsername(DatabaseConnector.DatabaseConnector database)
        {
            string result = RequestData("what is the users username:");
            while (database.CheckUsername(result))
                result = RequestData("username already exists, try again:");
            return result;
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
