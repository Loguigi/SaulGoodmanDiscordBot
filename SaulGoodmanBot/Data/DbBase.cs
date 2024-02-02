using System.ComponentModel;
using System.Data;
using Dapper;
using System.Reflection;
using SaulGoodmanBot.Config;
using System.Data.SqlClient;
using System.Net;
using System.Runtime.ConstrainedExecution;
using System.Reflection.Metadata.Ecma335;

namespace SaulGoodmanBot.Data;

public abstract class DbBase<TModel, TDomain> {
    protected SqlConnection Connection { get => new(_connectionString); }
    protected abstract ResultArgs<List<TModel>> GetData(string sp);
    protected abstract ResultArgs<int> SaveData(string sp, TModel data);
    protected abstract List<TDomain> MapData(List<TModel> data);
    protected T DeNull<T>(object? data, T defaultValue) => data == null ? defaultValue : (T)data;
    private string? _connectionString = Env.CnnVal;
}