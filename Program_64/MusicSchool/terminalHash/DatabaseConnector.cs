using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseConnector
{
    /// <summary>
    /// Author: Michael Devenish
    /// </summary>
    public class DatabaseConnector
    {
        private MySqlConnection connection;
        private string server;
        private string database;
        private string uid;
        private string password;

        public DatabaseConnector()
        {
            Initialize();
        }

        /// <summary>
        /// This function initalises the state of the database connection, it 
        /// does not connect yet but sets up the values that are used to connect
        /// </summary>
        private void Initialize()
        {
            server = "db4free.net";
            database = "musicschool";
            uid = "ifb299admin";
            password = "Cpq7BWXZsVP35nZwsUzL";
            string connectionString = "SERVER=" + server + ";" + "DATABASE=" +
            database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";

            connection = new MySqlConnection(connectionString);
        }

        /// <summary>
        /// This function handles the oepning of the database connection, call this in 
        /// an if statment and put your query inside the if statment 
        /// </summary>
        /// <returns>true if opened successfully</returns>
        private bool OpenConnection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (MySqlException e)
            {
                return false;
            }
        }

        /// <summary>
        /// This function handles the closing of the database connection, Call this when
        /// You have finished your query
        /// </summary>
        /// <returns>true if closed successfully</returns>
        private bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException e)
            {
                return false;
            }
        }



        /// <summary>
        /// This is an example class that shows how execute a query to get data
        /// </summary>
        /// <returns>result of query</returns>
        public List<string>[] ExampleReadDatabaseClass(int id)
        {
            String query = "SELECT first_name,last_name,dob FROM users WHERE user_id = @userID;";

            //each sublist represents a column in the database
            List<string>[] list = new List<string>[3];
            list[0] = new List<string>();
            list[1] = new List<string>();
            list[2] = new List<string>();

            if (OpenConnection())
            {
                //Create Command, bind value, Create a data reader and Execute the command
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@userID", id);
                MySqlDataReader dataReader = cmd.ExecuteReader();

                //Read the data and store it
                while (dataReader.Read())
                {
                    list[0].Add(dataReader["first_name"] + "");
                    list[1].Add(dataReader["last_name"] + "");
                    list[2].Add(dataReader["dob"] + "");
                }

                //close everything
                dataReader.Close();
                CloseConnection();

                //return result
                return list;
            }
            else return null; //if cant connect return null

        }

        /// <summary>
        /// This is an example class that shows how execute a query to get data
        /// </summary>
        /// <returns>result of query</returns>
        public bool CheckUsername(string username)
        {
            String query = "SELECT username FROM users WHERE username = @username;";
            bool result = false;

            if (OpenConnection())
            {
                //Create Command, bind value, Create a data reader and Execute the command
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@username", username);
                MySqlDataReader dataReader = cmd.ExecuteReader();

                //check if exists
                while (dataReader.Read())
                    if ((string)dataReader["username"] == username)
                        result = true;

                //close everything
                dataReader.Close();
                CloseConnection();
            }
            return result; //if cant connect return null

        }

        /// <summary>
        /// This is an example class that shows how to execute a query to set data
        /// </summary>
        public void ExampleWriteDatabaseClass(string firstName, string lastName, DateTime dob, int role, byte[] password_hash, string salt, string username)
        {

            String query = "INSERT INTO users (first_name, last_name, dob, role, password_hash, salt,username) VALUES (@first_name, @last_name, @dob, @role, @password_hash, @salt,@username);";

            if (OpenConnection())
            {

                //Create Command bind values and execute
                MySqlCommand cmd = new MySqlCommand(query, connection);

                cmd.Parameters.AddWithValue("@first_name", firstName);
                cmd.Parameters.AddWithValue("@last_name", lastName);
                cmd.Parameters.AddWithValue("@dob", dob.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@role", role);
                cmd.Parameters.AddWithValue("@password_hash", password_hash);
                cmd.Parameters.AddWithValue("@salt", salt);
                cmd.Parameters.AddWithValue("@username", username);

                cmd.ExecuteNonQuery();

                //close everything
                CloseConnection();
            }


        }

        /// <summary>
        /// This is an example class that outlines data insertion into the instruments table
        /// </summary>
        public void ExampleWriteInstrumentDatabaseClass(string InstrumentQuality, string instrumentName, string instrumentType)
        {

            String query = "INSERT INTO instruments (quality, instrument_name, instrument_type) VALUES (@quality, @instrument_name, @instrument_type);";

            if (OpenConnection())
            {

                //Create Command bind values and execute
                MySqlCommand cmd = new MySqlCommand(query, connection);

                cmd.Parameters.AddWithValue("@quality", InstrumentQuality);
                cmd.Parameters.AddWithValue("@instrument_name", instrumentName);
                cmd.Parameters.AddWithValue("@instrument_type", instrumentType);
             
                cmd.ExecuteNonQuery();

                //close everything
                CloseConnection();
            }


        }


    }
}
