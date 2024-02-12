using DSharpPlus.Entities;
using System.Reflection;
using System.Data;
using System.Collections;
using SaulGoodmanBot.Data;
using SaulGoodmanBot.Models;
using SaulGoodmanBot.Library;
using Dapper;

namespace SaulGoodmanBot.Controllers;

public class WheelPickers : DbBase<WheelsModel, Wheel>, IEnumerable<Wheel> {
    #region Properties
    private DiscordGuild Guild { get; set; }
    public List<Wheel> Wheels { get; private set; } = new();
    public bool IsEmpty { get; private set; } = false;
    #endregion

    #region Public Methods
    public WheelPickers(DiscordGuild guild) {
        Guild = guild;
        try {
            var result = GetData(
                StoredProcedures.GET_WHEEL_DATA, 
                new DynamicParameters( new { GuildId = (long)Guild.Id })).Result;
            
            if (result.Status != StatusCodes.SUCCESS)
                throw new Exception(result.Message);
            MapData(result.Result!);
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    public Wheel this[string key] { get => Wheels.Where(x => x.Name == key).First(); }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public IEnumerator<Wheel> GetEnumerator() => Wheels.GetEnumerator();

    public bool Contains(string wheelName) => Wheels.Where(x => x.Name == wheelName).FirstOrDefault() != null;

    public void AddWheel(Wheel wheel, string first_option) {
        try {
            var result = SaveData(StoredProcedures.PROCESS_WHEEL_DATA, new DynamicParameters(
                new WheelsModel() {
                    GuildId = (long)Guild.Id,
                    WheelName = wheel.Name,
                    WheelOption = first_option,
                    ImageUrl = wheel.Image == string.Empty ? null : wheel.Image,
                    Mode = (int)DataMode.ADD_WHEEL
            })).Result;

            if (result.Status != StatusCodes.SUCCESS)
                throw new Exception(result.Message);
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    public void AddOption(Wheel wheel, string option) {
        try {
            var result = SaveData(StoredProcedures.PROCESS_WHEEL_DATA, new DynamicParameters(
                new WheelsModel() {
                    GuildId = (long)Guild.Id,
                    WheelName = wheel.Name,
                    WheelOption = option,
                    Mode = (int)DataMode.ADD_OPTION
            })).Result;
            if (result.Status != StatusCodes.SUCCESS) 
                throw new Exception(result.Message);
            wheel.AvailableOptions.Add(option);
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    public void DeleteWheel(Wheel wheel) {
        try {
            var result = SaveData(StoredProcedures.PROCESS_WHEEL_DATA, new DynamicParameters(
                new WheelsModel() {
                    GuildId = (long)Guild.Id,
                    WheelName = wheel.Name,
                    Mode = (int)DataMode.DELETE_WHEEL
            })).Result;
            if (result.Status != StatusCodes.SUCCESS)
                throw new Exception(result.Message);
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    public void DeleteOption(Wheel wheel, string option) {
        try {
            var result = SaveData(StoredProcedures.PROCESS_WHEEL_DATA, new DynamicParameters(
                new WheelsModel() {
                    GuildId = (long)Guild.Id,
                    WheelName = wheel.Name,
                    WheelOption = option,
                    Mode = (int)DataMode.DELETE_OPTION
            })).Result;
            if (result.Status != StatusCodes.SUCCESS)
                throw new Exception(result.Message);
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    public void TemporarilyRemoveOption(Wheel wheel, string option) {
        try {
            var result = SaveData(StoredProcedures.PROCESS_WHEEL_DATA, new DynamicParameters(
                new WheelsModel() {
                    GuildId = (long)Guild.Id,
                    WheelName = wheel.Name,
                    WheelOption = option,
                    TempRemoved = 1,
                    Mode = (int)DataMode.TEMP_REMOVE
            })).Result;
            if (result.Status != StatusCodes.SUCCESS)
                throw new Exception(result.Message);
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }

    public void Restore(Wheel wheel) {
        try {
            var result = SaveData(StoredProcedures.PROCESS_WHEEL_DATA, new DynamicParameters(
                new WheelsModel() {
                    GuildId = (long)Guild.Id,
                    WheelName = wheel.Name,
                    Mode = (int)DataMode.RESTORE
            })).Result;
            if (result.Status != StatusCodes.SUCCESS)
                throw new Exception(result.Message);
        } catch (Exception ex) {
            ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
            throw;
        }
    }
    #endregion

    #region DB Methods
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
        public const string GET_WHEEL_DATA = "Wheels_GetData";
        public const string PROCESS_WHEEL_DATA = "Wheels_Process";
    }
    #endregion
}
