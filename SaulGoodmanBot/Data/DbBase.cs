using System.ComponentModel;
using System.Data;
using Dapper;
using System.Reflection;
using SaulGoodmanBot.Config;
using System.Data.SqlClient;
using System.Net;
using DSharpPlus.Entities;
using DSharpPlus;

namespace SaulGoodmanBot.Data;

public abstract class DbBase<TModel, TDomain> {
    protected SqlConnection Connection => new(_connectionString);

    protected virtual async Task<ResultArgs<List<TModel>>> GetData(string sp, DynamicParameters param) {
        try {
            using IDbConnection cnn = Connection;
            param.Add("@Status", dbType: DbType.Int32, direction: ParameterDirection.Output);
            param.Add("@ErrMsg", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
            var data = await cnn.QueryAsync<TModel>(sp, param, commandType: CommandType.StoredProcedure);

            return new ResultArgs<List<TModel>>() {
                Result = data.ToList(),
                Status = (StatusCodes)param.Get<int>("@Status"),
                Message = param.Get<string>("@ErrMsg")
            };

        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    protected virtual async Task<ResultArgs<int>> SaveData(string sp, DynamicParameters param) {
        try {
            using IDbConnection cnn = Connection;
            param.Add("@Status", dbType: DbType.Int32, direction: ParameterDirection.Output);
            param.Add("@ErrMsg", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
            var result = await cnn.ExecuteAsync(sp, param, commandType: CommandType.StoredProcedure);

            return new ResultArgs<int>() {
                Result = result,
                Status = (StatusCodes)param.Get<int>("@Status"),
                Message = param.Get<string>("@ErrMsg")
            };
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    protected abstract List<TDomain> MapData(List<TModel> data);
    protected async virtual Task<DiscordUser> GetUser(DiscordClient client, ulong id) => await client.GetUserAsync(id);
    private string? _connectionString = Env.CnnVal;
}

public abstract class DbBase {
    protected SqlConnection Connection => new(_connectionString);
    protected virtual async Task<ResultArgs<List<T>>> GetData<T>(string sp, DynamicParameters param) {
        try {
            using IDbConnection cnn = Connection;
            param.Add("@Status", dbType: DbType.Int32, direction: ParameterDirection.Output);
            param.Add("@ErrMsg", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
            var data = await cnn.QueryAsync<T>(sp, param, commandType: CommandType.StoredProcedure);

            return new ResultArgs<List<T>>() {
                Result = data.ToList(),
                Status = (StatusCodes)param.Get<int>("@Status"),
                Message = param.Get<string>("@ErrMsg")
            };

        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    protected virtual async Task<ResultArgs<int>> SaveData(string sp, DynamicParameters param) {
        try {
            using IDbConnection cnn = Connection;
            param.Add("@Status", dbType: DbType.Int32, direction: ParameterDirection.Output);
            param.Add("@ErrMsg", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
            var result = await cnn.ExecuteAsync(sp, param, commandType: CommandType.StoredProcedure);

            return new ResultArgs<int>() {
                Result = result,
                Status = (StatusCodes)param.Get<int>("@Status"),
                Message = param.Get<string>("@ErrMsg")
            };
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    protected async virtual Task<DiscordUser> GetUser(DiscordClient client, ulong id) => await client.GetUserAsync(id);
    private string? _connectionString = Env.CnnVal;
}