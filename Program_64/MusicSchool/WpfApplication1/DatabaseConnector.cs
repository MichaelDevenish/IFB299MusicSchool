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

        public List<string>[] getTimetableInfo(int id)
        {
            List<string>[] list = new List<string>[5];
            list[0] = new List<string>();
            list[1] = new List<string>();
            list[2] = new List<string>();
            list[3] = new List<string>();
            list[4] = new List<string>();

            if (this.OpenConnection())
            {
                String query = "SELECT student_id,teacher_id,lesson_date,comments,attended FROM lessons WHERE student_id = @STUDENTID;";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@STUDENTID", id);
                MySqlDataReader dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    list[0].Add(dataReader["student_id"] + "");
                    list[1].Add(dataReader["teacher_id"] + "");
                    list[2].Add(dataReader["lesson_date"] + "");
                    list[3].Add(dataReader["comments"] + "");
                    list[4].Add(dataReader["attended"] + "");
                }

                dataReader.Close();
                this.CloseConnection();
            }

            return list;
        }


        /// <summary>
        /// Simplifies opening a connection, reading from or writing to the database, and closing the connection
        /// </summary>
        /// <param name="write">Boolean that is determines if it is a write operation for true, or a read operation for false</param>
        /// <param name="query">Mysql statement to execute</param>
        /// <param name="parameters">Dictionary of parameters where the keys are the variables in the MySql statement, and the values are the values to be binded to them </param>
        /// <returns></returns>
        public List<string>[] simpleConnection(bool write, string query, Dictionary<string, string> parameters)
        {
            if (this.OpenConnection())
            {

                MySqlCommand cmd = new MySqlCommand(query, this.connection);
                //Get the values from the parameter dictionary and use it to bind variables in the query statement
                if (parameters != null)
                {
                    foreach (KeyValuePair<string, string> par in parameters)
                    {
                        cmd.Parameters.AddWithValue(par.Key, par.Value);
                    }
                }


                //-------WRITE OPERATION------
                if (write)
                {
                    cmd.ExecuteNonQuery();
                    CloseConnection();
                    return null;
                }
                //---------READ OPERATION--------
                else
                {
                    MySqlDataReader dataReader = cmd.ExecuteReader();
                    //now that we know the field length, set up the list object
                    List<string>[] list = new List<string>[dataReader.FieldCount];
                    for (int i = 0; i < dataReader.FieldCount; i++)
                    {
                        list[i] = new List<string>();
                    }
                    while (dataReader.Read())
                    {
                        for (int i = 0; i < dataReader.FieldCount; i++)
                        {
                            list[i].Add(dataReader[i].ToString());
                        }
                    }
                    dataReader.Close();
                    CloseConnection();
                    return list;
                }

            }
            //connection wasn't opened
            return null;
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
                cmd.Parameters.AddWithValue("@userID", username);
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
    }
}
