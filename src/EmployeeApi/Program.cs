using EmployeeApi.Models;
using EmployeeApi.Repositories;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IEmployeeRepository, JsonEmployeeRepository>();

var app = builder.Build();

app.MapGet("/employees", async (IEmployeeRepository repository, CancellationToken cancellationToken) =>
    Results.Ok(await repository.GetAllAsync(cancellationToken)));

app.MapGet("/employees/{id}", async (string id, IEmployeeRepository repository, CancellationToken cancellationToken) =>
{
    var employee = await repository.GetByIdAsync(id, cancellationToken);
    return employee is null ? Results.NotFound() : Results.Ok(employee);
});

app.MapPost("/employees", async (EmployeeRequest request, IEmployeeRepository repository, CancellationToken cancellationToken) =>
{
    var validation = request.Validate();
    if (validation is not null) return Results.ValidationProblem(validation);
    if (await repository.GetByIdAsync(request.EmployeeId!, cancellationToken) is not null)
        return Results.Conflict(new { message = "An employee with this ID already exists." });

    var employee = new Employee(request.EmployeeId!, request.Name!, request.Email!, request.Department!);
    await repository.AddAsync(employee, cancellationToken);
    return Results.Created($"/employees/{employee.EmployeeId}", employee);
});

app.MapPut("/employees/{id}", async (string id, EmployeeRequest request, IEmployeeRepository repository, CancellationToken cancellationToken) =>
{
    var validation = request.Validate();
    if (validation is not null) return Results.ValidationProblem(validation);
    if (!string.Equals(id, request.EmployeeId, StringComparison.OrdinalIgnoreCase))
        return Results.ValidationProblem(new Dictionary<string, string[]> { ["EmployeeId"] = ["Employee ID cannot be changed."] });
    if (!await repository.UpdateAsync(new Employee(id, request.Name!, request.Email!, request.Department!), cancellationToken))
        return Results.NotFound();
    return Results.NoContent();
});

app.MapDelete("/employees/{id}", async (string id, IEmployeeRepository repository, CancellationToken cancellationToken) =>
    await repository.DeleteAsync(id, cancellationToken) ? Results.NoContent() : Results.NotFound());

app.Run();
