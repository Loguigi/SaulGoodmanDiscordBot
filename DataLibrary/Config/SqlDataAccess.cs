using Dapper;
using System.Data;
using System.Data.SqlClient;

namespace DataLibrary.Config;

public static class SqlDataAccess
{
    public static string ConnectionString { get; } = "Server=.;Database=SaulGoodmanDB;Trusted_Connection=True;";
    // public static string GetConnectionString(string name)
    // {
    //     return ConfigurationManager.ConnectionStrings[name].ConnectionString;
    // }

    public static List<T> LoadData<T>(string sql)
    {
        try {
            using (IDbConnection cnn = new SqlConnection(ConnectionString))
            {
                return cnn.Query<T>(sql).ToList();
            }
        } catch (Exception e) {
            Console.WriteLine(e.Message);
            return new List<T>();
        }
    }

    public static int SaveData<T>(string sql, T data)
    {
        try {
            using (IDbConnection cnn = new SqlConnection(ConnectionString))
            {
                return cnn.Execute(sql, data);
            }
        } catch (Exception e) {
            Console.WriteLine(e.Message);
            return 0;
        }
    }
}
