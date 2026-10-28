namespace AspNetCoreWebApi.Endpoints;

using System.Collections.Concurrent;
using AspNetCoreWebApi.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

public static class CourseEndpoints
{
    private static readonly ConcurrentDictionary<Guid, CourseDto> _courses = new();

    static CourseEndpoints()
    {
        // Seed default course
        var seedId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        _courses[seedId] = new CourseDto(
            seedId,
            "Mastering .NET 10 & C# 13",
            "Comprehensive from zero to advanced course",
            "Advanced",
            49.99m,
            DateTime.UtcNow
        );
    }

    public static IEndpointRouteBuilder MapCourseEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/courses")
                       .WithTags("Courses");

        // GET /api/courses
        group.MapGet("/", (string? level, decimal? maxPrice) =>
        {
            var query = _courses.Values.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(level))
            {
                query = query.Where(c => c.Level.Equals(level, StringComparison.OrdinalIgnoreCase));
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(c => c.Price <= maxPrice.Value);
            }

            return TypedResults.Ok(query.OrderByDescending(c => c.CreatedAt).ToList());
        })
        .WithName("GetCourses")
        .WithSummary("List all courses with optional filters");

        // GET /api/courses/{id}
        group.MapGet("/{id:guid}", Results<Ok<CourseDto>, NotFound<ProblemDetails>> (Guid id) =>
        {
            if (_courses.TryGetValue(id, out var course))
            {
                return TypedResults.Ok(course);
            }

            return TypedResults.NotFound(new ProblemDetails
            {
                Title = "Course Not Found",
                Detail = $"No course found matching identifier '{id}'.",
                Status = StatusCodes.Status404NotFound
            });
        })
        .WithName("GetCourseById")
        .WithSummary("Retrieve a course by its unique ID");

        // POST /api/courses
        group.MapPost("/", Results<Created<CourseDto>, BadRequest<ProblemDetails>> (CreateCourseRequest req) =>
        {
            // Simple validation
            if (string.IsNullOrWhiteSpace(req.Title) || req.Price < 0)
            {
                return TypedResults.BadRequest(new ProblemDetails
                {
                    Title = "Validation Failed",
                    Detail = "Course Title is required and Price must be non-negative.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            var course = new CourseDto(
                Guid.NewGuid(),
                req.Title.Trim(),
                req.Description?.Trim() ?? string.Empty,
                req.Level?.Trim() ?? "Beginner",
                req.Price,
                DateTime.UtcNow
            );

            _courses[course.Id] = course;

            return TypedResults.Created($"/api/courses/{course.Id}", course);
        })
        .WithName("CreateCourse")
        .WithSummary("Create a new course item");

        // PUT /api/courses/{id}
        group.MapPut("/{id:guid}", Results<Ok<CourseDto>, NotFound<ProblemDetails>, BadRequest<ProblemDetails>> (Guid id, UpdateCourseRequest req) =>
        {
            if (!_courses.TryGetValue(id, out var existing))
            {
                return TypedResults.NotFound(new ProblemDetails
                {
                    Title = "Course Not Found",
                    Detail = $"Cannot update non-existent course '{id}'.",
                    Status = StatusCodes.Status404NotFound
                });
            }

            if (string.IsNullOrWhiteSpace(req.Title) || req.Price < 0)
            {
                return TypedResults.BadRequest(new ProblemDetails
                {
                    Title = "Validation Failed",
                    Detail = "Title cannot be empty and price must be >= 0.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            var updated = existing with
            {
                Title = req.Title.Trim(),
                Description = req.Description?.Trim() ?? existing.Description,
                Price = req.Price
            };

            _courses[id] = updated;
            return TypedResults.Ok(updated);
        })
        .WithName("UpdateCourse")
        .WithSummary("Update an existing course");

        // DELETE /api/courses/{id}
        group.MapDelete("/{id:guid}", Results<NoContent, NotFound<ProblemDetails>> (Guid id) =>
        {
            if (_courses.TryRemove(id, out _))
            {
                return TypedResults.NoContent();
            }

            return TypedResults.NotFound(new ProblemDetails
            {
                Title = "Course Not Found",
                Detail = $"Cannot delete course with ID '{id}'.",
                Status = StatusCodes.Status404NotFound
            });
        })
        .WithName("DeleteCourse")
        .WithSummary("Delete a course");

        return app;
    }
}
