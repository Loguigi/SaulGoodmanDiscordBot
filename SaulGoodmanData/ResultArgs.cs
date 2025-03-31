namespace SaulGoodmanData;

public class ResultArgs(int status, string message)
{
    public StatusCodes Status = (StatusCodes)status;
    public string Message => message;
}

public enum StatusCodes 
{
    SUCCESS = 0,
    ERROR = 1,
    NEEDS_SETUP = 2,
    UNKNOWN = 99
}