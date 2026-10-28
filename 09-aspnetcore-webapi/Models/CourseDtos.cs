namespace AspNetCoreWebApi.Models;

public record CourseDto(
    Guid Id,
    string Title,
    string Description,
    string Level,
    decimal Price,
    DateTime CreatedAt
);

public record CreateCourseRequest(
    string Title,
    string Description,
    string Level,
    decimal Price
);

public record UpdateCourseRequest(
    string Title,
    string Description,
    decimal Price
);
