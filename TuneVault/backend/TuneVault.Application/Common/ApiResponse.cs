namespace TuneVault.Application.Common;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public List<string> Errors { get; set; } = new();

    public static ApiResponse<T> Ok(T data) => new() { Success = true, Data = data };
    public static ApiResponse<T> Fail(params string[] errors) => new() { Success = false, Errors = errors.ToList() };
}

public class ApiResponse
{
    public bool Success { get; set; }
    public object? Data { get; set; }
    public List<string> Errors { get; set; } = new();

    public static ApiResponse Ok(object? data = null) => new() { Success = true, Data = data };
    public static ApiResponse Fail(params string[] errors) => new() { Success = false, Errors = errors.ToList() };
}
