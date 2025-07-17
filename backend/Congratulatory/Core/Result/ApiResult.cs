namespace CoreService.Result;

public class ApiResult
{
    public bool Success { get; set; }
    public object? Data { get; set; }
    public ApiError? Error { get; set; }

    public static ApiResult Ok(object? data = null) =>
        new ApiResult { Success = true, Data = data };

    public static ApiResult Fail(string message, IReadOnlyDictionary<string, string[]>? details = null) =>
        new ApiResult { Success = false, Error = new ApiError { Message = message, Details = details } };
}

public class ApiError
{
    public string? Message { get; set; }
    public IReadOnlyDictionary<string, string[]>? Details { get; set; }
}
