namespace SmartAttendanceSystem.Presentation.Abstraction;

public static class ResultExtensions
{
    public static ObjectResult ToProblem(this Result result)
    {
        if (result.IsSuccess)
            throw new InvalidOperationException("Cannot create a problem details response for a successful result.");

        var problem = Results.Problem(statusCode: result.Error.StatusCode);

        var problemDetails = problem.GetType().GetProperty(nameof(ProblemDetails))!.GetValue(problem) as ProblemDetails;

        problemDetails!.Extensions = new Dictionary<string, object?>
        {
            {
                "error", new Dictionary<string, string>
                {
                    { "code", result.Error.Code },
                    { "description", result.Error.Description }
                }
            }
        };

        return new ObjectResult(problemDetails);
    }
}
