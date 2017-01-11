using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using System.Security.Cryptography;

namespace TestWriting
{
    public class Database : IDisposable
    {

        public OracleConnection Connection; 

        public Database()
        {
            Connection = new OracleConnection(ConfigurationManager.ConnectionStrings["DataFormConnection"].ConnectionString);
        }


        public OracleConnection Connect()
        {
            Connection.Open();
            return Connection;
        }

        public static OracleConnection ConnectAndStartTransaction(IsolationLevel isoLevel)
        {
            Database db = new Database();
            OracleConnection connection = db.Connect();
            connection.BeginTransaction(isoLevel);

            return connection;
        }

        public void TestConnection()
        {
            Connection.Open();
            Connection.Close();
        }

        public static T? GetValue<T>(DataRow row, string columnName) where T : struct
        {
            if (row.IsNull(columnName))
                return null;

            return row[columnName] as T?;
        }

        public static string GetStringFromReader(IDataReader reader, string columnName)
        {
            if (reader[columnName] == null)
                return string.Empty;

            return reader[columnName] as string ?? string.Empty;
        }





        public static string CalculateMD5Hash(string input)
        {

            MD5 md5 = System.Security.Cryptography.MD5.Create();

            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);

            byte[] hash = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)

            {

                sb.Append(hash[i].ToString("X2"));

            }

            return sb.ToString();

        }


        public void Dispose()
        {
            if (Connection.State != ConnectionState.Closed )
            {
                Connection.Close();
            }
        }
    }
}
