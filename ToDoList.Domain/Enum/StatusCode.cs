namespace ToDoList.Domain.Enum;

public enum StatusCode
{
    TaskIsHasAlready = 1,
    OK = 200,
    InternalServerError = 500,
    BadRequest = 400,
    Unauthorized = 401,
    Forbidden = 403,
    NotFound = 404
}