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
    protected abstract ResultArgs<List<TModel>> GetData(string sp);
    protected abstract ResultArgs<int> SaveData(TModel data, string sp);
    protected abstract List<TDomain> MapData(List<TModel> data);
    protected async virtual Task<DiscordUser> GetUser(DiscordClient client, ulong id) => await client.GetUserAsync(id);
    protected T DeNull<T>(object? data, T defaultValue) => data == null ? defaultValue : (T)data;
    private string? _connectionString = Env.CnnVal;
}