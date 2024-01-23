using System.ComponentModel;
using System.Reflection;

namespace SaulGoodmanBot;

internal class DbBase {
    public class ResultArgs {
        public int Result { get; set; }
        public string Message { get; set; } = string.Empty;
        public string ExtraData { get; set; } = string.Empty;

        public ResultArgs(int result, string message) {
            Result = result;
            Message = message;
        }
    }

    public struct StoredProcedures {
        #region Wheel Picker SP
        public const string GET_WHEELS = "Wheels_GetData";
        public const string WHEEL_PROCESS = "Wheels_Process";
        #endregion
    }

    private string _connectionString = string.Empty;

    // public DbBase(string cnnString) {
    //     _connectionString = cnnString;
    // }

    [Browsable(false)]
    public string ConnectionString {
        get {
            try {
                return _connectionString ?? string.Empty;
            } catch (Exception ex) {
                ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
                throw;
            }
        }
        set {
            try {
                _connectionString = value ?? string.Empty;
            } catch (Exception ex) {
                ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
                throw;
            }
        }
    }

    public T DeNull<T>(object? data, T defaultValue) => data == null ? defaultValue : (T)data;
}