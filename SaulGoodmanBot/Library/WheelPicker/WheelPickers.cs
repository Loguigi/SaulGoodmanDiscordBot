using DSharpPlus.Entities;
using DataLibrary.Logic;
using DataLibrary.Models;
using System.Reflection;
using System.Data.SqlClient;
using System.Data;
using System.ComponentModel.DataAnnotations.Schema;
using Dapper;
using System.Collections;

namespace SaulGoodmanBot.Library.WheelPicker;

internal class WheelPickers : DbBase, IEnumerable<Wheel> {
    public WheelPickers(DiscordGuild guild) {
        Guild = guild;
        try {
            var result = DBGetData();
            if (result.Result != 0)
                throw new Exception(result.Message);
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    #region Public Methods
    public Wheel this[string key] {
        get => Wheels.Where(x => x.Name == key).FirstOrDefault() ?? throw new Exception("Wheel does not exist");
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public IEnumerator<Wheel> GetEnumerator() => Wheels.GetEnumerator();

    public bool Contains(string wheelName) => Wheels.Where(x => x.Name == wheelName).FirstOrDefault() != null;

    public void AddWheel(Wheel wheel, string first_option) {
        try {
            var result = DBProcess(wheel, DataMode.ADD_WHEEL, first_option);
            if (result.Result != 0)
                throw new Exception(result.Message);
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    public void AddOption(Wheel wheel, string option) {
        try {
            var result = DBProcess(wheel, DataMode.ADD_OPTION, option);
            if (result.Result != 0) 
                throw new Exception(result.Message);
            wheel.AvailableOptions.Add(option);
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    public void DeleteWheel(Wheel wheel) {
        try {
            var result = DBProcess(wheel, DataMode.DELETE_WHEEL);
            if (result.Result != 0)
                throw new Exception(result.Message);
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    public void DeleteOption(Wheel wheel, string option) {
        try {
            var result = DBProcess(wheel, DataMode.DELETE_OPTION, option);
            if (result.Result != 0)
                throw new Exception(result.Message);
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    public void TemporarilyRemoveOption(Wheel wheel, string option) {
        try {
            var result = DBProcess(wheel, DataMode.TEMP_REMOVE, option);
            if (result.Result != 0)
                throw new Exception(result.Message);
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    public void Restore(Wheel wheel) {
        try {
            var result = DBProcess(wheel, DataMode.RESTORE);
            if (result.Result != 0)
                throw new Exception(result.Message);
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }
    #endregion

    #region DB Methods
    private enum DataMode {
        ADD_WHEEL = 0,
        ADD_OPTION = 1,
        DELETE_WHEEL = 2,
        DELETE_OPTION = 3,
        TEMP_REMOVE = 4,
        RESTORE = 5
    }

    private ResultArgs DBGetData() {
        try
        {
            var ds = new DataSet();
            SqlParameter pGuild;
            SqlParameter pStatus;
            SqlParameter pErrMsg;

            using (SqlConnection cnn = new(ConnectionString)) {
                cnn.Open();
                var cmd = cnn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = StoredProcedures.GET_WHEELS;

                pGuild = new SqlParameter("@p_GuildId", SqlDbType.BigInt) {
                    Value = Guild.Id,
                    Direction = ParameterDirection.Input
                };
                cmd.Parameters.Add(pGuild);

                pStatus = new SqlParameter("@p_Status", SqlDbType.Int);
                pStatus.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(pStatus);

                pErrMsg = new SqlParameter("@p_ErrMsg", SqlDbType.VarChar, 500);
                pStatus.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(pStatus);

                var da = new SqlDataAdapter() { SelectCommand = cmd };
                da.Fill(ds);
                var result = new ResultArgs((int)pStatus.Value, (string)pErrMsg.Value);

                if (result.Result > 0) {
                    return result;
                }

                var dtr = ds.CreateDataReader();
                if (dtr.HasRows) {
                    Wheels.Clear();
                    while (dtr.Read()) {
                        string wheelName = DeNull(dtr["WheelName"].ToString(), string.Empty);
                        bool tempDelete = DeNull((bool)dtr["TempRemoved"], false);
                        string option = DeNull(dtr["WheelOption"].ToString(), string.Empty);
                        if (!Contains(wheelName)) {
                            Wheels.Add(new Wheel(wheelName, DeNull(dtr["ImageUrl"].ToString(), string.Empty)));
                        }
                        
                        if (tempDelete) {
                            this[wheelName].RemovedOptions.Add(option);
                        } else {
                            this[wheelName].AvailableOptions.Add(option);
                        }
                    }
                }
                return result;
            }
        }
        catch (Exception ex)
        {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    private ResultArgs DBProcess(Wheel wheel, DataMode mode, string option="") {
        try {
            SqlParameter param;
            SqlParameter pStatus;
            SqlParameter pErrMsg;

            using (SqlConnection cnn = new(ConnectionString)) {
                cnn.Open();
                var cmd = cnn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = StoredProcedures.WHEEL_PROCESS;

                param = new SqlParameter() {
                    ParameterName = "@p_GuildId",
                    SqlDbType = SqlDbType.BigInt,
                    Direction = ParameterDirection.Input,
                    Value = Guild.Id
                };
                cmd.Parameters.Add(param);

                param = new SqlParameter() {
                    ParameterName = "@p_WheelName",
                    SqlDbType = SqlDbType.VarChar,
                    Size = 100,
                    Direction = ParameterDirection.Input,
                    Value = wheel.Name
                };
                cmd.Parameters.Add(param);

                param = new SqlParameter() {
                    ParameterName = "@p_WheelOption",
                    SqlDbType = SqlDbType.VarChar,
                    Size = 100,
                    Direction = ParameterDirection.Input,
                    Value = option
                };
                cmd.Parameters.Add(param);

                param = new SqlParameter() {
                    ParameterName = "@p_ImageUrl",
                    SqlDbType = SqlDbType.VarChar,
                    Size = 1000,
                    Direction = ParameterDirection.Input,
                    Value = wheel.Image
                };
                cmd.Parameters.Add(param);

                param = new SqlParameter() {
                    ParameterName = "@p_TempRemoved",
                    SqlDbType = SqlDbType.Bit,
                    Direction = ParameterDirection.Input,
                    Value = wheel.RemovedOptions.Contains(option) ? 1 : 0
                };
                cmd.Parameters.Add(param);

                param = new SqlParameter() {
                    ParameterName = "@p_Mode",
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input,
                    Value = (int)mode
                };
                cmd.Parameters.Add(param);

                pStatus = new SqlParameter() {
                    ParameterName = "@p_Status",
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(pStatus);

                pErrMsg = new SqlParameter() {
                    ParameterName = "@p_ErrMsg",
                    SqlDbType = SqlDbType.VarChar,
                    Size = 500,
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(pErrMsg);

                cmd.ExecuteNonQuery();
                var result = new ResultArgs((int)pStatus.Value, pErrMsg.Value.ToString() ?? throw new Exception());
                return result;
            }
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }
    #endregion

    #region Properties
    private DiscordGuild Guild { get; set; }
    public List<Wheel> Wheels = new();
    private const int WHEEL_LIMIT = 25;
    #endregion
}
