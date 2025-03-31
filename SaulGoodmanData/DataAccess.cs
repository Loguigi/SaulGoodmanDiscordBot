using System.Data;
using System.Reflection;
using Dapper;
using Microsoft.Data.SqlClient;

namespace SaulGoodmanData;

public abstract class DataAccess
{
    /// <summary>
    /// Abstract database function to call GetData() and refresh data
    /// </summary>
    public abstract void Load();
    
    #region Protected DB Methods
    /// <summary>
    /// Gets data from SQL Server database
    /// </summary>
    /// <param name="sp">Stored procedure to call from StoredProcedures struct</param>
    /// <param name="param">List of any parameters</param>
    /// <typeparam name="T">Model to get a list of</typeparam>
    /// <returns>List of DB data</returns>
    /// <exception cref="Exception"></exception>
    protected async Task<List<T>> GetData<T>(string sp, DynamicParameters param) 
    {
        try 
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new Exception("Connection string is null or empty");
            }

            await using SqlConnection cnn = new(_connectionString);
            param.Add("Status", null, DbType.Int32, ParameterDirection.Output);
            param.Add("Message", null, DbType.String, ParameterDirection.Output, 500);
            var data = await cnn.QueryAsync<T>(sp, param, commandType: CommandType.StoredProcedure);
			
            ResultArgs result = new(param.Get<int>("Status"), param.Get<string>("Message"));
            if (result.Status != StatusCodes.SUCCESS) throw new Exception(result.Message);
			
            return data.ToList();
        } 
        catch (Exception ex) 
        {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    protected async Task SaveData(string sp, DynamicParameters param)
    {
        try 
        {
            if (string.IsNullOrEmpty(_connectionString)) 
            {
                throw new Exception("Connection string is null or empty");
            }
			
            await using SqlConnection cnn = new(_connectionString);
            param.Add("Status", null, DbType.Int32, ParameterDirection.Output);
            param.Add("Message", null, DbType.String, ParameterDirection.Output, 500);
            await cnn.ExecuteAsync(sp, param, commandType: CommandType.StoredProcedure);

            ResultArgs result = new(param.Get<int>("Status"), param.Get<string>("Message"));
            if (result.Status != StatusCodes.SUCCESS) throw new Exception(result.Message);
        } 
        catch (Exception ex) 
        {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }
    #endregion Public DB Methods
    
    #region Protected Classes
    protected struct StoredProcedures
    {
        #region ServerMembers

        public const string MEMBER_LOAD_ONE = "ServerMembers_LoadOne";
        public const string MEMBER_LOAD_ALL = "ServerMembers_LoadAll";
        public const string MEMBER_ACTIVATE = "ServerMembers_ActivateMember";
        public const string MEMBER_DEACTIVATE = "ServerMembers_DeactivateMember";
        public const string MEMBER_SAVE = "ServerMembers_SaveMember";
        #endregion ServerMembers
        
        #region Santa
        public const string SANTA_GET_CONFIG = "Santa_GetConfig";
        public const string SANTA_SAVE_CONFIG = "Santa_SaveConfig";
        public const string SANTA_GET_PARTICIPANTS = "Santa_GetParticipants";
        public const string SANTA_SAVE_PARTICIPANT = "Santa_SaveParticipant";
        public const string SANTA_END_EVENT = "Santa_EndEvent";
        #endregion Santa
        
        #region Wheels
        public const string GET_WHEEL_DATA = "Wheels_GetData";
        public const string PROCESS_WHEEL_DATA = "Wheels_Process";
        #endregion Wheels
        
        #region Config
        public const string GET_CONFIG_DATA = "Config_GetData";
        public const string PROCESS_CONFIG_DATA = "Config_Process";
        #endregion Config
        
        #region Roles
        public const string GET_ROLE_DATA = "Roles_GetData";
        public const string PROCESS_ROLE_DATA = "Roles_Process";
        public const string REMOVE_ROLE_DATA = "Roles_Remove";
        #endregion Roles
    }
    
    protected class ResultArgs(int status, string message)
    {
        public readonly StatusCodes Status = (StatusCodes)status;
        public string Message => message;
    }

    protected enum StatusCodes 
    {
        SUCCESS = 0,
        ERROR = 1,
        NEEDS_SETUP = 2,
        UNKNOWN = 99
    }
    #endregion Protected Classes

    private readonly string? _connectionString = Environment.GetEnvironmentVariable("SAULCNN");
}