using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace resQbackEnd
{
    internal class App
    {
        internal App()
        {
            Launch();
        }
        private static void Launch()
        {
            switch (AppHandle._sessionValue)//Refer to comments in AppHandle class for _sessionvalue
            {
                case 1111: // debugging
                    DebugControls.d2();
                    break;
                case 0000: // Security Pin State

                    break;
                case 0001: // Null App Process

                    break;

            }
        }
    }
    public static class AppHandle
    {
        public static int _sessionValue = 1111;
        /* Used for pages and overall app state
         * Session Values:
         * 1--- = Already initiated, app launching, awaiting internal/external action
         * 0--- = App opened for the first time
         * --00 = Security Pin
         * --1- = Front Page(0,1)
         * --2- = Setup(0,1,2,3,4)
         * --3- = Security Backup(0,1)
         * --01 = App Process(while active)
         * --02 = Main Page
         * --03 = Background Run
         * -0-- = Pending/Awaiting OTP(0,1)
         */
        public static void frontPage() { }
        public static void setup(int mode) { }
        public static void mainPage(int state) { }
        public static void secBackup(int mode)
        {
            if (mode == 0)
            {

            }
        }
        public static void voidBridge(int typing) { }
    }
    public static class FrontHandler
    {
        public static void displayBridge() { }
    }
    public static class ApiHandler { }
    public static class DataHandler
    {
        private const string _dataFileName = "userData.db";
        private const string connectionString = $"Data Source={_dataFileName};Version=3;";

        public static void createDatabase()
        {
            try
            {
                if (!File.Exists(_dataFileName))
                {
                    SQLiteConnection.CreateFile(_dataFileName);
                    Console.Write("Database Created!");
                }
                else
                {
                    Console.Write("Database already exists.");
                }
            }
            catch (Exception ex)
            {
                Console.Write("Error: " + ex);
            }
        }
        public static bool checkDatabase()
        {
            SQLiteConnection conn = new SQLiteConnection(connectionString);
            try
            {
                if (File.Exists(_dataFileName))
                {
                    using (conn)
                    {
                        conn.Open();
                        if (tableExists(conn, "Contacts"))
                        {
                            conn.Close();
                            return true;
                        }
                        else
                        {
                            conn.Close();
                            return false;
                        }
                    }
                }
                else return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nAn error occurred: " + ex.Message);

                return false;
            }
        }

        private static bool tableExists(SQLiteConnection connection, string tableName)
        {
            string sql = "SELECT name FROM sqlite_master WHERE type='table' AND name=@tableName";

            using (var command = new SQLiteCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@tableName", tableName);
                object result = command.ExecuteScalar();
                return result != null;
            }
        }

        public static void createTable()
        {
            SQLiteConnection conn = new SQLiteConnection(connectionString);
            conn.Open();
            string sql = @"
                CREATE TABLE Contacts (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT,
                    Number BIGINT NOT NULL,
                    ContactState INTEGER NOT NULL
                );";

            using (var command = new SQLiteCommand(sql, conn))
            {
                command.ExecuteNonQuery();
            }
            Console.WriteLine("'Contacts' table created!");
            conn.Close();
        }

        // Contacts
        public static void createContact(Contact contact)
        {
            SQLiteConnection conn = new SQLiteConnection(connectionString);
            conn.Open();
            string sql = @"
                INSERT INTO Contacts (Name, Number, ContactState)
                VALUES (@name,@number,@cstate);";

            using (var command = new SQLiteCommand(sql, conn))
            {
                command.Parameters.AddWithValue("@name", contact.Name());
                command.Parameters.AddWithValue("@number", contact.Number());
                command.Parameters.AddWithValue("@cstate", contact.ContactState());
                Console.Write(
                    @$"
                        Contact's Name: {contact.Name()}
                        Contact's Number: {contact.Number()}
                        Contact's State: {contact.ContactState()}
                    "
                );
                Console.ReadLine();
                command.ExecuteNonQuery();
            }
            Console.WriteLine("Contact Saved!");
            conn.Close();
        }
        public static void deleteContact(Contact contact)
        {
            SQLiteConnection conn = new SQLiteConnection(connectionString);
            conn.Open();
            string sql = @"
                DELETE FROM Contacts WHERE Number=@number";

            using (var command = new SQLiteCommand(sql, conn))
            {
                command.Parameters.AddWithValue("@number", contact.Number());
                command.ExecuteNonQuery();
            }
            Console.WriteLine("Contact Deleted!");
            conn.Close();
        }
        public static string generateOTP()
        {
            Random rnd = new Random(); //New Random class object
            string _strOTP = Convert.ToString(rnd.Next(1, 999)); //Genetares 0-999, 3 digit Number and converts it to string
            if (_strOTP.Length < 3) //Code inside runs if the number generated is 1-2 digits 
            {
                string _zeroAdd = string.Empty; //String container for storing the 0s needed for the code to become 3 digit
                for (int i = _strOTP.Length; i < 3; i++) //For loop to add the 0s depending on how many digit there is
                {
                    _zeroAdd += "0"; // Adds the 0s into the container String
                }
                return _zeroAdd + _strOTP; //Returns the value as a 3 digit string
            }
            else return _strOTP; // If the numbers are 3 digits already, returns the full value instead
        }
        public static void updateContact(Contact oldContact, Contact newContact) { }
    }


    public class Contact
    {
        private string _name;

        /*
         * For null-value Names, add '%%'
         */

        private long _number;
        private int _contactState;

        /*
         * contactState:
         * 0 - Unconfirmed Contact Number
         * 1 - OTP-Confirmed Contact Number
         * 2 - Personal Number
         * 3 - Hotline
         */

        public Contact(string name, long number, int contactState)
        {
            _name = name;
            _number = number;
            if (contactState < 0 || contactState > 3) _contactState = 0;
            else _contactState = contactState;

        }

        public string Name() { return _name; }
        public long Number() { return _number; }
        public int ContactState() { return _contactState; }
    }

    public static class DebugControls
    {


        public static void d3() //Deleting contacts
        {
            
        }
        public static void d2() //Generates 100 OTP codes, can't send anything to contact numbers yet
        {

            for (int i = 0; i < 100; i++)
            {

                Console.WriteLine(DataHandler.generateOTP());
            }
        }
        public static void d1() //Initial debug, creates database and table, then asks for the initial contact number and saves it to the database, Contacts table
        {
            Console.WriteLine("Database and Table exists: " + DataHandler.checkDatabase());
            if (!DataHandler.checkDatabase())
            {
                Console.WriteLine("Creating Database...");
                DataHandler.createDatabase();
                Console.WriteLine("Creating Table...");
                DataHandler.createTable();
            }
            else
            {
                Console.WriteLine("Input a Name(Press [Enter] to skip): ");
                string _inputName = Console.ReadLine();

                Console.WriteLine("Input Contact's Number(11 digits, all decimals): ");
                bool _correctNumberFormat = false;

                long _inputNumber = 0; // True Contact Number, loop below checks correct format for temporary contact number
                while (!_correctNumberFormat)
                {
                    string _tempInputNumber = string.Empty;
                    try
                    {
                        _tempInputNumber = Console.ReadLine();
                        Console.Write(_tempInputNumber);

                        _inputNumber = Convert.ToInt64(_tempInputNumber); //Failed string-to-int conversion is thrown to catch block to retry input again
                        if (Convert.ToString(_inputNumber).Length != 10 || Convert.ToString(_inputNumber)[9] != '9') int.Parse("wrong format"); //Checks if the 10-digit format with number 9 starting from the left

                        _correctNumberFormat = true;
                    }
                    catch
                    {
                        Console.WriteLine("WRONG FORMAT");
                        Thread.Sleep(800);
                        _correctNumberFormat = false;
                    }
                }

                var _contact = new Contact(_inputName, _inputNumber, 000);
                DataHandler.createContact(_contact);
            }
        }
    }
}
