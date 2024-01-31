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

internal abstract class DbBase<TDTO, TModel> {
    protected SqlConnection Connection { get => new(_connectionString); }
    protected abstract ResultArgs<List<TDTO>> GetData(string sp);
    protected abstract ResultArgs<int> SaveData(string sp, TDTO data);
    protected abstract List<TModel> MapData(List<TDTO> data);
    protected T DeNull<T>(object? data, T defaultValue) => data == null ? defaultValue : (T)data;
    private string? _connectionString = Env.CnnVal;
}