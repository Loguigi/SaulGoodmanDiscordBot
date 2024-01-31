namespace SaulGoodmanBot.Data;

public class ResultArgs<T> {
    public enum StatusCodes {
        SUCCESS = 0,
        ERROR = 1,
        NEEDS_SETUP = 2,
        UNKNOWN = 99
    }

    public T Result { get; set; }
    public StatusCodes Status { get; set; } = StatusCodes.UNKNOWN;
    public string Message { get; set; } = string.Empty;
    public string ExtraData { get; set; } = string.Empty;

    public ResultArgs(T result, int status, string message) {
        Result = result;
        Status = (StatusCodes)status;
        Message = message;
    }
}