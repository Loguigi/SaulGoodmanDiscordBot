using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using Dapper;
using DSharpPlus;
using DSharpPlus.Entities;
using SaulGoodmanBot.Config;

namespace SaulGoodmanBot.Data;

public abstract class DataAccess
{
    public struct StoredProcedures
    {
        public const string SANTA_GET_CONFIG = "Santa_GetConfig";
        public const string SANTA_SAVE_CONFIG = "Santa_SaveConfig";
        public const string SANTA_GET_PARTICIPANTS = "Santa_GetParticipants";
        public const string SANTA_SAVE_PARTICIPANT = "Santa_SaveParticipant";
        public const string SANTA_END_EVENT = "Santa_EndEvent";
    }
    
    protected string ConnectionString => Env.CnnVal;
    
    protected async Task<List<T>> GetData<T>(string sp, DynamicParameters param) 
    {
        try 
        {
            await using SqlConnection cnn = new(ConnectionString);
            param.Add("@Status", dbType: DbType.Int32, direction: ParameterDirection.Output);
            param.Add("@ErrMsg", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
            var data = await cnn.QueryAsync<T>(sp, param, commandType: CommandType.StoredProcedure);
            
            ResultArgs result = new(param.Get<int>("@Status"), param.Get<string>("@ErrMsg"));
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
            await using SqlConnection cnn = new(ConnectionString);
            param.Add("@Status", dbType: DbType.Int32, direction: ParameterDirection.Output);
            param.Add("@ErrMsg", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
            await cnn.ExecuteAsync(sp, param, commandType: CommandType.StoredProcedure);
            
            ResultArgs result = new(param.Get<int>("@Status"), param.Get<string>("@ErrMsg"));
            if (result.Status != StatusCodes.SUCCESS) throw new Exception(result.Message);
        } 
        catch (Exception ex) 
        {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }
    
    protected async Task<DiscordUser> GetUser(DiscordClient client, ulong id) => await client.GetUserAsync(id);
    protected async Task<DiscordUser> GetUser(DiscordClient client, long id) => await client.GetUserAsync((ulong)id);
}