using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace MariosPizzaApp
{
    public partial class LoginForm : Form
    {
        /**
         * Create global private variables. Can be used in all functions 
         * in this partial class.
         **/
        private MySqlConnection conn;
        private string host;
        private string db;
        private string uid;
        private string pass;
        private string tbl;
        bool connSuccess;
        int attempts = 3;

        string employeeId;

        public LoginForm()
        {
            InitializeComponent();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            // MySQL database credentials
            host = "localhost";
            db = "backend_db";
            uid = "root";
            pass = "";

            // create variable for table, as it will be used in all queries
            tbl = "tbl_appLogin";

            // create connection string used for the initial database connection - debug
            string connString;
            connString = "SERVER=" + host + ";" + "DATABASE=" + db + ";" + "UID=" + uid + ";" + "PASSWORD=" + pass + ";";
            conn = new MySqlConnection(connString);

            connSuccess = OpenConnection();

            // check whether database connection was successful. 
            if (connSuccess == true)
            {
                Console.WriteLine("Connection is successfully established");
                /**
                 * close connection if connection opened successfully.
                 * if database did not connect successfully, there is no
                 * connection to close, hence no CloseConnection method 
                 * if connSuccess = false.
                 **/
                CloseConnection();
            }
            else
            {
                MessageBox.Show("Database connecton not established. Contact an Administrator \nClosing application");
                Environment.Exit(0);
            }
        }

        private bool OpenConnection()
        {
            try
            {
                // attempt to open connection in try loop to ensure no crash
                conn.Open();

                // return true to have connSuccess set to true
                return true;
            }
            catch (MySqlException ex)
            {
                // return MySQL exception, with the string stored in 'ex'
                switch (ex.Number)
                {
                    // return number and compare against most common errors
                    case 0:
                        MessageBox.Show("Cannot connection to database server. Contact administrator");
                        break;
                    case 1045:
                        MessageBox.Show("invalid credentials for database. Contact administrator");
                        break;
                }
                // return false to have connSuccess set to false
                return false;
            }
        }

        private bool CloseConnection()
        {
            // attempt to close connection in try loop to remove crash
            try
            {
                // close connection using conn MySQLCommand
                conn.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                // return MySQL except and display error in message box
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            // create match bool to set to false if a match is found
            bool match = false;

            // mysql query string, collects useranme from text box and uses no % as they MUST be exact.
            string query = "SELECT * FROM " + tbl + " WHERE username LIKE '" + txtLoginUser.Text + "'";

            // open connection
            OpenConnection();

            // create mysql command object, then use this object to execute query 
            MySqlCommand cmd = new MySqlCommand(query, conn);
            MySqlDataReader response = cmd.ExecuteReader();

            // incase of errors, do not crash app but give error in console
            try
            {
                // continue until response has cycled through rows, 
                while (response.Read() && match == false)
                {
                    if (Convert.ToString(response["password"]) == txtLoginPass.Text)
                    {
                        employeeId = Convert.ToString(response["employee_key"]);
                        
                        // match found, set match to true
                        match = true;
                    }
                    else
                    {
                        // no match found, set match to false and continue loop
                        match = false;
                    }
                }

                response.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }

            CloseConnection();

            if (match == true)
            {
                Console.WriteLine("Login completed");
                MessageBox.Show("Login successful! Welcome " + txtLoginUser.Text);
                Form orderForm = new OrderForm(employeeId);
                orderForm.Show();
                this.Hide();
            }
            else
            {
                Console.WriteLine("Login credentials incorrect");
                attempts--;
                MessageBox.Show(string.Format("Invalid credentials. Please try again. \nYou have {0} attempts remaining!", attempts.ToString()));
            }

            if (attempts <= 0)
            {
                Environment.Exit(0);
            }
        }
    }
}
