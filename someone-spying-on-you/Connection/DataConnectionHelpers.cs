using SomeOneSpyingOnYou.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SomeOneSpyingOnYou.Connection
{
    public class DataConnectionHelpers
    {
        public static void TestTableAndCreateIfDoesntExists(SQLiteConnection conn, GenericClassConverter gcc)
        {
            SQLiteCommand dbTestCommand = new SQLiteCommand(conn);
            dbTestCommand.CommandText = testTableString(gcc);
            var test = dbTestCommand.ExecuteScalar();

            if (test == null)
            {
                dbTestCommand.CommandText = createTableString(gcc);
                dbTestCommand.ExecuteNonQuery();
            }
        }

        public static string ReturnDatabaseNamePathAndCreateIfDoesntExists()
        {
            var pathBin = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
            var dbName = (pathBin + @"\sosoyDb.sqlite").Replace(@"file:\", "");

            if (!File.Exists(dbName))
                SQLiteConnection.CreateFile(dbName);

            return dbName;
        }

        public static List<T> ConvertDataTableToGenericList<T>(DataTable dt)
        {
            PropertyInfo[] props = typeof(T).GetProperties();

            List<T> lst = new List<T>();
            foreach (DataRow dr in dt.Rows)
            {
                T ob = Activator.CreateInstance<T>();
                foreach (var propertyInfo in props)
                {
                    foreach (DataColumn dc in dt.Columns)
                    {
                        if (propertyInfo.Name == dc.ColumnName)
                        {
                            object value = dr[dc.ColumnName];
                            propertyInfo.SetValue(ob, value);
                            break;
                        }
                    }
                }

                lst.Add(ob);
            }

            return lst;
        }

        public static string GetInsertSql(GenericClassConverter gcc)
        {
            string columnNames = gcc.PropNamesAndTypes.Keys.Aggregate((f, s) => f + "," + s);
            string parameterNames = gcc.PropNamesAndTypes.Keys.Aggregate((f, s) => "@" + f + "," + "@" + s);

            return "INSERT INTO " + gcc.TableName + " (" + columnNames + ") VALUES (" + parameterNames + ")";
        }

        private static string testTableString(GenericClassConverter gcc)
        {
            return "SELECT name FROM sqlite_master WHERE type='table' AND name='" + gcc.TableName + "';";
        }

        private static string createTableString(GenericClassConverter gcc)
        {
            string retval = "CREATE TABLE " + gcc.TableName + " (";

            foreach (KeyValuePair<string, string> nat in gcc.PropNamesAndTypes)
            {
                retval += nat.Key + " " + getSqliteNameFromTypeName(nat.Value) + ",";
            }

            retval = retval.TrimEnd(',') + ")";
            return retval;
        }

        private static string getSqliteNameFromTypeName(string typeName)
        {
            //https://www.sqlite.org/datatype3.html
            //todo: make all converts.

            if (typeName.ToLower() == "int32" || typeName.ToLower() == "int" || typeName.ToLower() == "integer")
                return "INT";
            if (typeName.ToLower() == "string")
                return "TEXT";//VARCHAR(X)
            return "";
        }

        public static string CreateSqlSelectString(GenericClassConverter gcc)
        {
            return "SELECT * FROM " + gcc.TableName;
        }
    }
}
