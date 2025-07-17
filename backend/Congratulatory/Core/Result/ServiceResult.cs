using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreService.Result;

public class ServiceResult<T>
{
    public bool Success { get; private set; }
    public T? Data { get; private set; }
    public string? ErrorMessage { get; private set; }
    public IReadOnlyDictionary<string, string[]>? Errors { get; private set; }
    public int StatusCode { get; private set; }

    private ServiceResult() { }

    public static ServiceResult<T> Ok(T data) =>
        new ServiceResult<T> { Success = true, Data = data, StatusCode = 200 };

    public static ServiceResult<T> Fail(string errorMessage, IReadOnlyDictionary<string, string[]>? errors = null) =>
        new ServiceResult<T> { Success = false, ErrorMessage = errorMessage, Errors = errors, StatusCode = 400 };

    public static ServiceResult<T> Fail(
        int statusCode,
        string errorMessage,
        IReadOnlyDictionary<string, string[]>? errors = null) =>
        new ServiceResult<T> { Success = false, ErrorMessage = errorMessage, Errors = errors, StatusCode = statusCode };

    public static ServiceResult<T> NotFound(string errorMessage = "Не найдено") =>
        new ServiceResult<T> { Success = false, ErrorMessage = errorMessage, StatusCode = 404 };

    public static ServiceResult<T> BadRequest(string errorMessage, IReadOnlyDictionary<string, string[]>? errors = null) =>
        new ServiceResult<T> { Success = false, ErrorMessage = errorMessage, Errors = errors, StatusCode = 400 };

    public static ServiceResult<T> Conflict(string errorMessage) =>
        new ServiceResult<T> { Success = false, ErrorMessage = errorMessage, StatusCode = 409 };
}

