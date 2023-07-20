using Dapper;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace DataLibrary.Config;

public static class SqlDataAccess
{
    public static string GetConnectionString(string name)
    {
        return ConfigurationManager.ConnectionStrings[name].ConnectionString;
    }

    public static List<T> LoadData<T>(string sql)
    {
        using (IDbConnection cnn = new SqlConnection("Server=.;Database=SaulGoodmanDB;Trusted_Connection=True;"))
        {
            return cnn.Query<T>(sql).ToList();
        }
    }

    public static int SaveData<T>(string sql, T data)
    {
        using (IDbConnection cnn = new SqlConnection("Server=.;Database=SaulGoodmanDB;Trusted_Connection=True;"))
        {
            return cnn.Execute(sql, data);
        }
    }
}
