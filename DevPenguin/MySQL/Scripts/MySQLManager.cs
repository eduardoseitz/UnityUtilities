using UnityEngine;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using TMPro;

namespace DevPenguin.Utilities
{
    public class MySQLManager : MonoBehaviour
    {
        #region Declarations
        public static MySQLManager instance;

        [SerializeField] string _server = "sql9.freesqldatabase.com";
        [SerializeField] string _database = "sql9652545";
        [SerializeField] string _user = "sql9652545";
        [SerializeField] string _password = "Qs8qM3yVUR";
        [SerializeField] string _charset = "utf8";
        [SerializeField] TextMeshProUGUI debugText;

        //[SerializeField] Text textCanvas;
        //[SerializeField] Text nameText;
        //[SerializeField] Text surnameText;
        //[SerializeField] Text ageText;

        private string connectionString;
        private MySqlConnection MS_Connection;
        private MySqlCommand MS_Command;
        private MySqlDataReader MS_Reader;
        #endregion

        #region Unity Methods
        private void Awake()
        {
            // Singleton pattern.
            if (instance == null)
                instance = this;
            else
                Destroy(this.gameObject);

            DontDestroyOnLoad(this.gameObject);
        }
        #endregion

        #region HelperMethods
        public void WriteIntoSQLDatabase(string query)
        {
            ConnectIntoSQLDatabase();

            try
            {
                // query = "insert into Douglas(Nom, Cognom, Edat) values( '" + name.text + "' , '" + Surnames.text + "','" + age.text + "');";
                //query = $"insert into WalrusVsJujitsu(DidWalrusWin) values({didWalrusWin});";
                MS_Command = new MySqlCommand(query, MS_Connection);
                MS_Command.ExecuteNonQuery();
                MS_Connection.Close();
            }
            catch(MySqlException ex)
            {
                Debug.LogError("Error when WritoIntoSQLDatabase: " + ex.Message);
                debugText.text = "Error when WritoIntoSQLDatabase: " + ex.Message;
            }
        }

        public List<string> ReadFromSQLDatabase(string query)
        {
            List<string> _queryResult = new List<string>();
            ConnectIntoSQLDatabase();

            try
            {
                //query = "SELECT * FROM WalrusVsJujitsu";
                MS_Command = new MySqlCommand(query, MS_Connection);
                MS_Reader = MS_Command.ExecuteReader();
                while (MS_Reader.Read())
                {
                    for (int i = 0; i < MS_Reader.FieldCount; i++)
                    {
                        _queryResult.Add(MS_Reader[i] as string);
                    }
                    //textCanvas.text += "\n              " + MS_Reader[0] + "                            " + MS_Reader[1] + "                     " + MS_Reader[2] + "                    " + MS_Reader[3];
                }
                MS_Reader.Close();
            }
            catch(MySqlException ex)
            {
                Debug.LogError("Error when ReadFromSQLDatabase: " + ex.Message);
                debugText.text = "Error when ReadFromSQLDatabase: " + ex.Message;
            }

            return _queryResult;
        }

        private void ConnectIntoSQLDatabase()
        {
            try
            {
                connectionString = $"Server = {_server} ; Database = {_database} ; User = {_user}; Password = {_password}; Charset = {_charset};";
                MS_Connection = new MySqlConnection(connectionString);
                MS_Connection.Open();
            }
            catch(MySqlException ex)
            {
                Debug.LogError("Error when ConnectIntoSQLDatabase: " + ex.Message);
                debugText.text = "Error when ConnectIntoSQLDatabase: " + ex.Message;
            }
        }
        #endregion
    }
}
