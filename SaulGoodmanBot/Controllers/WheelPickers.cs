using DSharpPlus.Entities;
using System.Reflection;
using System.Data;
using System.Collections;
using SaulGoodmanBot.Data;
using SaulGoodmanBot.Models;
using SaulGoodmanBot.Library;
using Dapper;

namespace SaulGoodmanBot.Library.WheelPicker;

internal class WheelPickers : DbBase<WheelsModel, Wheel>, IEnumerable<Wheel> {
    public WheelPickers(DiscordGuild guild) {
        Guild = guild;
        try {
            var result = GetData();
            if (result.Status == ResultArgs<List<WheelsModel>>.StatusCodes.ERROR)
                throw new Exception(result.Message);
            else if (result.Status == ResultArgs<List<WheelsModel>>.StatusCodes.NEEDS_SETUP)
                IsEmpty = true;
            MapData(result.Result);
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
            var result = SaveData("soon", new WheelsModel() {
                GuildId = (long)Guild.Id,
                WheelName = wheel.Name,
                WheelOption = first_option,
                ImageUrl = wheel.Image == string.Empty ? null : wheel.Image,
                Mode = (int)DataMode.ADD_WHEEL
            });
            if (result.Status != 0)
                throw new Exception(result.Message);
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    public void AddOption(Wheel wheel, string option) {
        try {
            var result = SaveData("", new WheelsModel() {
                GuildId = (long)Guild.Id,
                WheelName = wheel.Name,
                WheelOption = option,
                Mode = (int)DataMode.ADD_OPTION
            });
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
            var result = SaveData("soon", new WheelsModel() {
                GuildId = (long)Guild.Id,
                WheelName = wheel.Name,
                Mode = (int)DataMode.DELETE_WHEEL
            });
            if (result.Result != 0)
                throw new Exception(result.Message);
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    public void DeleteOption(Wheel wheel, string option) {
        try {
            var result = SaveData("", new WheelsModel() {
                GuildId = (long)Guild.Id,
                WheelName = wheel.Name,
                WheelOption = option,
                Mode = (int)DataMode.DELETE_OPTION
            });
            if (result.Result != 0)
                throw new Exception(result.Message);
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    public void TemporarilyRemoveOption(Wheel wheel, string option) {
        try {
            var result = SaveData("", new WheelsModel() {
                GuildId = (long)Guild.Id,
                WheelName = wheel.Name,
                WheelOption = option,
                TempRemoved = 0,
                Mode = (int)DataMode.TEMP_REMOVE
            });
            if (result.Result != 0)
                throw new Exception(result.Message);
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    public void Restore(Wheel wheel) {
        try {
            var result = SaveData("", new WheelsModel() {
                GuildId = (long)Guild.Id,
                WheelName = wheel.Name,
                Mode = (int)DataMode.RESTORE
            });
            if (result.Result != 0)
                throw new Exception(result.Message);
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }
    #endregion

    #region DB Methods
    protected override ResultArgs<List<WheelsModel>> GetData(string sp=StoredProcedures.GET_DATA) {
        try {
            using IDbConnection cnn = Connection;
            var sql = sp + " @Status, @ErrMsg";
            var result = new DbCommonParams();
            var data = cnn.Query<WheelsModel>(sp, result).ToList();
            return new ResultArgs<List<WheelsModel>>(data, result.Status, result.ErrMsg);
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    protected override ResultArgs<int> SaveData(string sp, WheelsModel data)
    {
        try {
            using IDbConnection cnn = Connection;
            var sql = sp + 
                @" @GuildId,
                @WheelName,
                @WheelOption,
                @ImageUrl,
                @TempRemoved,
                @Mode,
                @Status,
                @ErrMsg";
            return new ResultArgs<int>(cnn.Execute(sp, data), data.Status, data.ErrMsg);
            
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    protected override List<Wheel> MapData(List<WheelsModel> data) {
        try {
            foreach (var w in data) {
                if (!Contains(w.WheelName)) {
                    Wheels.Add(new Wheel(w.WheelName, w.ImageUrl ?? string.Empty));
                }
                
                if (w.TempRemoved == 1) {
                    this[w.WheelName].RemovedOptions.Add(w.WheelOption);
                } else {
                    this[w.WheelName].AvailableOptions.Add(w.WheelOption);
                }
            }
            return Wheels;
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }
    private enum DataMode {
        ADD_WHEEL = 0,
        ADD_OPTION = 1,
        DELETE_WHEEL = 2,
        DELETE_OPTION = 3,
        TEMP_REMOVE = 4,
        RESTORE = 5
    }
    private struct StoredProcedures {
        public const string GET_DATA = "Wheels_GetData";
    }
    #endregion

    #region Properties
    private DiscordGuild Guild { get; set; }
    public List<Wheel> Wheels { get; private set; } = new();
    public bool IsEmpty { get; private set; } = false;
    private const int WHEEL_LIMIT = 25;
    #endregion
}
