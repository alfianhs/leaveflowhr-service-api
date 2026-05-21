namespace LeaveFlowHR.Api.Common.Results;

public class ServiceResult<T>
{
    public bool IsSuccess { get; }
    public string Error { get; }
    public T? Value { get; }

    private ServiceResult(bool success, T? value, string error)
    {
        IsSuccess = success;
        Value = value;
        Error = error;
    }

    public static ServiceResult<T> Success(T value)
        => new(true, value, "");

    public static ServiceResult<T> Failure(string error)
        => new(false, default, error);
}