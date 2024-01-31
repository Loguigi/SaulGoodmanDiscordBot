using DSharpPlus.Entities;
using System.Reflection;
using System.Data;
using System.Collections;
using SaulGoodmanBot.Data;
using SaulGoodmanBot.DTO;
using Dapper;

namespace SaulGoodmanBot.Library.WheelPicker;

internal class WheelPickers : DbBase<WheelsDTO, Wheel>, IEnumerable<Wheel> {
    public WheelPickers(DiscordGuild guild) {
        Guild = guild;
        try {
            var result = GetData();
            if (result.Status == ResultArgs<List<WheelsDTO>>.StatusCodes.ERROR)
                throw new Exception(result.Message);
            else if (result.Status == ResultArgs<List<WheelsDTO>>.StatusCodes.NEEDS_SETUP)
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
            var result = SaveData("soon", new WheelsDTO() {
                GuildId = (long)Guild.Id,
                WheelName = wheel.Name,
                WheelOption = first_option,
                ImageUrl = wheel.Image == string.Empty ? null : wheel.Image
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
    protected override ResultArgs<List<WheelsDTO>> GetData(string sp=StoredProcedures.GET_DATA) {
        try {
            using IDbConnection cnn = Connection;
            var sql = sp + " @Status, @ErrMsg";
            var result = new DbCommonParams();
            var data = cnn.Query<WheelsDTO>(sp, result).ToList();
            return new ResultArgs<List<WheelsDTO>>(data, result.Status, result.ErrMsg);
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    protected override ResultArgs<int> SaveData(string sp, WheelsDTO data)
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

    protected override List<Wheel> MapData(List<WheelsDTO> data) {
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
