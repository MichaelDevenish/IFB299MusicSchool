using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApplication1;

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
            catch (MySqlException)
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
            catch (MySqlException)
            {
                return false;
            }
        }

        /// <summary>
        /// Simplifies opening a connection, reading from or writing to the database, and closing the connection
        /// </summary>
        /// <param name="write">Boolean that is determines if it is a write operation for true, or a read operation for false</param>
        /// <param name="query">Mysql statement to execute</param>
        /// <param name="parameters">Dictionary of parameters where the keys are the variables in the MySql statement, and the values are the values to be binded to them </param>
        /// <returns></returns>
        public List<string[]> simpleConnection(bool write, string query, Dictionary<string, object> parameters)
        {
            if (this.OpenConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, this.connection);
                //Get the values from the parameter dictionary and use it to bind variables in the query statement
                if (parameters != null)
                    foreach (KeyValuePair<string, object> par in parameters)
                        cmd.Parameters.AddWithValue(par.Key, par.Value);

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
                    List<string[]> list = new List<string[]>();

                    while (dataReader.Read())
                    {
                        string[] tempArray = new string[dataReader.FieldCount];
                        for (int i = 0; i < dataReader.FieldCount; i++)
                            tempArray[i] = dataReader[i].ToString();

                        list.Add(tempArray);
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
        /// Searches the database for each lesson on a specified date that the user has booked and returns it
        /// </summary>
        /// <param name="id">the student id</param>
        /// <param name="date"> the requested date</param>
        /// <returns>an array containing the first and last name of the teacher, the lesson date and the
        /// lesson length (0 = 30min, 1 - 60min)</returns>
        public List<string[]> ReadUserLessons(int id)
        {
            String query = "SELECT users.first_name, users.last_name, lessons.lesson_date, lessons.lesson_length" +
                          " FROM lessons LEFT JOIN users ON lessons.teacher_id = users.user_id" +
                          " WHERE student_id = @userID;";
            List<string[]> list = new List<string[]>();

            if (OpenConnection())
            {
                //Create Command, bind value, Create a data reader and Execute the command
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@userID", id);
                MySqlDataReader dataReader = cmd.ExecuteReader();

                //Read the data and store it
                while (dataReader.Read())
                {
                    string[] tempArray = { dataReader["first_name"] + "",
                        dataReader["last_name"] + "", dataReader["lesson_date"] + "",dataReader["lesson_length"] + "" };
                    list.Add(tempArray);
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
        /// Searches the database for each lesson on a specified date that does not have 
        /// a student assigned and returns it
        /// </summary>
        /// <returns>an array containing the first and last name of the teacher, the lesson date and the
        /// lesson length (0 = 30min, 1 - 60min)</returns>
        public List<string[]> ReadEmptyLessons()
        {
            String query = "SELECT users.first_name, users.last_name, lessons.lesson_date, lessons.lesson_length" +
                          " FROM lessons LEFT JOIN users ON lessons.teacher_id = users.user_id" +
                          " WHERE student_id IS NULL;";


            List<string[]> list = new List<string[]>();

            if (OpenConnection())
            {
                //Create Command, bind value, Create a data reader and Execute the command
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader dataReader = cmd.ExecuteReader();

                //Read the data and store it
                while (dataReader.Read())
                    list.Add(new string[]{ dataReader["first_name"] + "", dataReader["last_name"] + "",
                                        dataReader["lesson_date"] + "",dataReader["lesson_length"] + ""  });

                //close everything
                dataReader.Close();
                CloseConnection();

                //return result
                return list;
            }
            else return null; //if cant connect return null
        }

        /// <summary>
        /// Checks if the users login credentials are correct and returns the permission status and id of the user
        /// if correct and all values set to false or -1 respectively if wrong
        /// </summary>
        /// <param name="username">the username of the user</param>
        /// <param name="password">the users password</param>
        /// <returns>[0] = admin bool, [1] = teacher bool, [2] = userID</returns>
        public List<string[]> ReadLoginCheck(string username, string password)
        {
            String query = "SELECT user_id, role, password_hash, salt FROM users WHERE username = @username";
            List<string[]> list = new List<string[]>();
            PasswordManagment pass = new PasswordManagment();

            if (OpenConnection())
            {
                //Create Command, bind value, Create a data reader and Execute the command
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@username", username);
                MySqlDataReader dataReader = cmd.ExecuteReader();

                //Read the data do some comparisons and insert results into an array
                while (dataReader.Read())
                {
                    //checks the users roles
                    bool teacher = false;
                    bool admin = false;
                    int role = Convert.ToInt32(dataReader["role"]);
                    if (role == 1) teacher = true;
                    if (role == 0) admin = true;

                    string[] tempArray;

                    if (compareByteArrays((byte[])dataReader["password_hash"], pass.GenerateHash(password, dataReader["salt"].ToString())))
                        tempArray = new[] { true.ToString(), admin.ToString(), teacher.ToString(), dataReader["user_id"].ToString() };
                    else tempArray = new[] { false.ToString(), false.ToString(), false.ToString(), "-1" };

                    list.Add(tempArray);
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
        /// Compares two byte arrays together and returns true if they match and false if they dont
        /// </summary>
        /// <param name="byte1"></param>
        /// <param name="byte2"></param>
        /// <returns></returns>
        private static bool compareByteArrays(byte[] byte1, byte[] byte2)
        {
            bool hashComparison = true;
            if (byte1.Length != byte2.Length)
                hashComparison = false;
            for (int i = 0; i < byte1.Length; i++)
                if (byte1[i] != byte2[i])
                    hashComparison = false;
            return hashComparison;
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


        public List<string[]> ReadTeacherInfo()
        {
            String query = "SELECT user_id, first_name, last_name FROM users WHERE role=1";


            List<string[]> list = new List<string[]>();

            if (OpenConnection())
            {
                //Create Command, bind value, Create a data reader and Execute the command
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader dataReader = cmd.ExecuteReader();

                //Read the data and store it
                while (dataReader.Read())
                    list.Add(new string[] { dataReader["user_id"] + "", dataReader["first_name"] + "", dataReader["last_name"] + "" });

                //close everything
                dataReader.Close();
                CloseConnection();

                //return result
                return list;
            }
            else return null; //if cant connect return null
        }


        public List<string[]> ReadInstrumentInfo()
        {
            String query = "SELECT instrument_id, instrument_name, instrument_type, quality FROM instruments;";


            List<string[]> list = new List<string[]>();

            if (OpenConnection())
            {
                //Create Command, bind value, Create a data reader and Execute the command
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader dataReader = cmd.ExecuteReader();

                //Read the data and store it
                while (dataReader.Read())
                    list.Add(new string[]{ dataReader["instrument_id"] + "", dataReader["instrument_name"] + "",
                        dataReader["instrument_type"] + "", dataReader["quality"] + "" });

                //close everything
                dataReader.Close();
                CloseConnection();

                //return result
                return list;
            }
            else return null; //if cant connect return null
        }


        /// <summary>
        /// Inserts a provided user into the database
        /// </summary>
        /// <param name="firstName">the first name of the user</param>
        /// <param name="lastName">the last name of the user</param>
        /// <param name="dob">the dob of the user</param>
        /// <param name="role">the role of the user (0 = admin, 1 = teacher, 2 = student)</param>
        /// <param name="password_hash">a hashed version of the users password</param>
        /// <param name="salt">the salt that the users password was hashed with</param>
        /// <param name="username">the users username</param>
        public void InsertUser(string firstName, string lastName, DateTime dob, int role, byte[] password_hash, string salt, string username)
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
        /// Inserts a lesson into the database
        /// </summary>
        /// <param name="teacherID">the teacher holding the lesson</param>
        /// <param name="lessonDate">the date of the lesson in half hour increments from 9am to 5pm</param>
        /// <param name="length">the length of the lesson (false = 30 minutes, true = 1 hour)</param>
        public void InsertLesson(int teacherID, DateTime lessonDate, bool length)
        {

            String query = "INSERT INTO lessons (teacher_id,lesson_length,lesson_date) VALUES (@id, @length, @date);";

            if (OpenConnection())
            {
                //Create Command bind values and execute
                MySqlCommand cmd = new MySqlCommand(query, connection);

                cmd.Parameters.AddWithValue("@id", teacherID);
                cmd.Parameters.AddWithValue("@length", length);
                cmd.Parameters.AddWithValue("@date", lessonDate.ToString("yyyy-MM-dd HH:mm:ss"));


                cmd.ExecuteNonQuery();

                //close everything
                CloseConnection();
            }
        }
    }
}
