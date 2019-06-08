using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using SomeOneSpyingOnYou.Models;

namespace SomeOneSpyingOnYou.Connection
{
    public class DataConnection
    {
        public static List<T> Get<T>(string sqlSuffix = "")
        {
            //Create GenericClassConverter object.
            //Its hold all information about model class.
            GenericClassConverter gcc = new GenericClassConverter();
            gcc.ConvertClass<T>();
            if (gcc == null || gcc.PropNamesAndTypes.Count == 0 || string.IsNullOrWhiteSpace(gcc.TableName))
                throw new Exception("GenericClassConverter object is null. Please control your model object.");


            //Get bin folder for database path. If db doesnt exist create.
            using (var conn = new SQLiteConnection("Data Source=" + DataConnectionHelpers.ReturnDatabaseNamePathAndCreateIfDoesntExists() + ";Version=3;"))
            {
                try
                {
                    conn.Open();

                    //try test command for table. If table doesnt exist create.
                    DataConnectionHelpers.TestTableAndCreateIfDoesntExists(conn, gcc);
                    
                    //Create sql command for model. Also you can give suffix like where clause.
                    string sql = DataConnectionHelpers.CreateSqlSelectString(gcc) + sqlSuffix;
                    DataSet ds = new DataSet();
                    var da = new SQLiteDataAdapter(sql, conn);
                    da.Fill(ds);
                    
                    //Convert DataTable to model.
                    return DataConnectionHelpers.ConvertDataTableToGenericList<T>(ds.Tables[0]);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public static void Insert<T>(T model)
        {
            GenericClassConverter gcc = new GenericClassConverter();
            gcc.ConvertClass<T>();
            if (gcc == null || gcc.PropNamesAndTypes.Count == 0 || string.IsNullOrWhiteSpace(gcc.TableName))
                throw new Exception("GenericClassConverter object is null. Please control your model object.");


            using (var conn = new SQLiteConnection("Data Source=" + DataConnectionHelpers.ReturnDatabaseNamePathAndCreateIfDoesntExists() + ";Version=3;"))
            {
                conn.Open();

                DataConnectionHelpers.TestTableAndCreateIfDoesntExists(conn, gcc);
                
                SQLiteCommand insertSQL = new SQLiteCommand(DataConnectionHelpers.GetInsertSql(gcc), conn);
                

                foreach (KeyValuePair<string, string> pr in gcc.PropNamesAndTypes)
                {
                    PropertyInfo pi = typeof(T).GetProperty(pr.Key);
                    object value = pi.GetValue(model, null);
                    
                    insertSQL.Parameters.AddWithValue("@" + pr.Key, value);
                }

                try
                {
                    insertSQL.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }
    }
}
