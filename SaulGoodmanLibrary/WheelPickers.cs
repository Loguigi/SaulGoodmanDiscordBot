using Dapper;
using DSharpPlus.Entities;
using SaulGoodmanBot.Helpers;
using SaulGoodmanData;
using SaulGoodmanLibrary.Models;

namespace SaulGoodmanLibrary;

public class WheelPickers : DataAccess 
{
    #region Properties
    private DiscordGuild Guild { get; set; }
    public List<Wheel> Wheels { get; private set; } = [];
    #endregion

    #region Public Methods
    public WheelPickers(DiscordGuild guild) 
    {
        Guild = guild;
        Load();
    }

    public sealed override void Load()
    {
        var data = GetData<WheelsModel>(StoredProcedures.GET_WHEEL_DATA, new DynamicParameters(new { GuildId = (long)Guild.Id})).Result;
        
        foreach (var w in data) 
        {
            if (this[w.WheelName] is null) 
            {
                Wheels.Add(new Wheel(w.WheelName, w.ImageUrl ?? string.Empty));
            }
                
            if (w.TempRemoved == 1) 
            {
                this[w.WheelName]!.RemovedOptions.Add(w.WheelOption);
            } 
            else 
            {
                this[w.WheelName]!.AvailableOptions.Add(w.WheelOption);
            }
        }
    }

    public Wheel? this[string key] => Wheels.FirstOrDefault(x => x.Name == key);

    public async Task CreateWheel(Wheel wheel, string firstOption) 
    {
        DynamicParameters param = new(new WheelsModel(wheel, Guild, firstOption));
        param.AddDynamicParams(new { Mode = (int)DataMode.ADD_WHEEL });
        await SaveData(StoredProcedures.PROCESS_WHEEL_DATA, param);
        Load();
    }

    public async Task AddOption(Wheel wheel, string option)
    {
        DynamicParameters param = new(new WheelsModel(wheel, Guild, option));
        param.AddDynamicParams(new { Mode = (int)DataMode.ADD_OPTION });
        await SaveData(StoredProcedures.PROCESS_WHEEL_DATA, param);
        Load();
    }

    public async Task DeleteWheel(Wheel wheel) 
    {
        DynamicParameters param = new(new WheelsModel(wheel, Guild));
        param.AddDynamicParams(new { Mode = (int)DataMode.DELETE_WHEEL });
        await SaveData(StoredProcedures.PROCESS_WHEEL_DATA, param);
        Load();
    }

    public async Task DeleteOption(Wheel wheel, string option) 
    {
        DynamicParameters param = new(new WheelsModel(wheel, Guild, option));
        param.AddDynamicParams(new { Mode = (int)DataMode.DELETE_OPTION });
        await SaveData(StoredProcedures.PROCESS_WHEEL_DATA, param);
        Load();
    }

    public async Task TemporarilyRemoveOption(Wheel wheel, string option) 
    {
        DynamicParameters param = new(new WheelsModel(wheel, Guild, option, true));
        param.AddDynamicParams(new { Mode = (int)DataMode.TEMP_REMOVE });
        await SaveData(StoredProcedures.PROCESS_WHEEL_DATA, param);
        Load();
    }

    public async Task Restore(Wheel wheel) 
    {
        DynamicParameters param = new(new WheelsModel(wheel, Guild));
        param.AddDynamicParams(new { Mode = (int)DataMode.RESTORE });
        await SaveData(StoredProcedures.PROCESS_WHEEL_DATA, param);
        Load();
    }
    #endregion

    private enum DataMode 
    {
        ADD_WHEEL = 0,
        ADD_OPTION = 1,
        DELETE_WHEEL = 2,
        DELETE_OPTION = 3,
        TEMP_REMOVE = 4,
        RESTORE = 5
    }
    
    public class Wheel(string name, string imgurl)
    {
        #region Properties
        public string Name { get; private set; } = name;
        public List<string> AvailableOptions { get; private set; } = [];
        public List<string> RemovedOptions { get; private set; } = [];
        public List<string> Options => [.. AvailableOptions, .. RemovedOptions];
        public string Image { get; set; } = imgurl;

        #endregion

        public string Spin() 
        {
            var i = RandomHelper.RNG.Next(AvailableOptions.Count);
            return AvailableOptions[i];
        }
    }
}
