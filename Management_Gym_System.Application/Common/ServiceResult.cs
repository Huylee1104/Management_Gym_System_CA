public class ServiceResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;

    public static ServiceResult Success(string message = "") 
        => new ServiceResult { IsSuccess = true, Message = message };

    public static ServiceResult Failure(string message) 
        => new ServiceResult { IsSuccess = false, Message = message };
}

public class ServiceResultWithId
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public long? Id { get; set; }

    public static ServiceResultWithId Success(string message = "", long? id = null)
        => new ServiceResultWithId { IsSuccess = true, Message = message, Id = id };

    public static ServiceResultWithId Failure(string message)
        => new ServiceResultWithId { IsSuccess = false, Message = message, Id = null };
}