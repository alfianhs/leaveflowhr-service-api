using System.Text.Json.Serialization;

namespace LeaveFlowHR.Api.Common.Responses;

public class ApiResponse<T>
{
    public int Status { get; set; }
    public string Message { get; set; } = string.Empty;
    public Dictionary<string, string[]> Validation { get; set; } = new Dictionary<string, string[]>();
    public T? Data { get; set; }
    public PaginationMeta? Meta { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Path { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? StackTrace { get; set; }

    public static ApiResponse<T> Success(T data, string message = "success", int status = 200)
    {
        return new ApiResponse<T>
        {
            Status = status,
            Message = message,
            Validation = new Dictionary<string, string[]>(),
            Data = data
        };
    }

    public static ApiResponse<T> SuccessPaged(T data, int page, int pageSize, int totalItems, string message = "success")
    {
        return new ApiResponse<T>
        {
            Status = 200,
            Message = message,
            Validation = new Dictionary<string, string[]>(),
            Data = data,
            Meta = new PaginationMeta
            {
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling((double)totalItems / pageSize)
            }
        };
    }

    public static ApiResponse<T> Fail(string message, Dictionary<string, string[]>? validation, int status = 400)
    {
        return new ApiResponse<T>
        {
            Status = status,
            Message = message,
            Validation = validation ?? new Dictionary<string, string[]>(),
            Data = default
        };
    }

    public static ApiResponse<T> Error(string message = "something went wrong", int status = 500, string? path = null, string? stackTrace = null)
    {
        return new ApiResponse<T>
        {
            Status = status,
            Message = message,
            Validation = new Dictionary<string, string[]>(),
            Data = default,
            Path = path,
            StackTrace = stackTrace
        };
    }
}

public class PaginationMeta
{
    public int Page { get; set; }

    public int PageSize { get; set; }

    public int TotalItems { get; set; }

    public int TotalPages { get; set; }
}
