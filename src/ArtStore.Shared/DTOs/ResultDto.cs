namespace ArtStore.Shared.DTOs;

public class Result
{
    public bool Succeeded { get; set; }
    public IReadOnlyList<string> Errors { get; set; } = [];
    public string ErrorMessage => string.Join(", ", Errors);

    [JsonConstructor]
    private Result() { }

    public static Result Success() => new() { Succeeded = true };

    public static Task<Result> SuccessAsync() =>
        Task.FromResult(Success());

    public static Result Failure(params string[] errors) =>
        new() { Succeeded = false, Errors = errors };

    public static Task<Result> FailureAsync(params string[] errors) =>
        Task.FromResult(Failure(errors));
}

public class Result<T>
{
    public bool Succeeded { get; set; }
    public IReadOnlyList<string> Errors { get; set; } = [];
    public T? Data { get; set; }

    [JsonConstructor]
    private Result() { }

    public static Result<T> Success(T data) => new() { Succeeded = true, Data = data };

    public static Task<Result<T>> SuccessAsync(T data) =>
        Task.FromResult(Success(data));

    public static Result<T> Failure(params string[] errors) =>
        new() { Succeeded = false, Errors = errors };

    public static Task<Result<T>> FailureAsync(params string[] errors) =>
        Task.FromResult(Failure(errors));

    public static implicit operator Result<T>(T data) =>
        Success(data);
}