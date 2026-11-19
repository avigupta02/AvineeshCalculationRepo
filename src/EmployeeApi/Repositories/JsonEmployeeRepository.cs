using System.Text.Json;
using EmployeeApi.Models;

namespace EmployeeApi.Repositories;

public sealed class JsonEmployeeRepository : IEmployeeRepository
{
    private readonly string _path;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public JsonEmployeeRepository(IWebHostEnvironment environment) => _path = Path.Combine(environment.ContentRootPath, "Data", "employees.json");

    public async Task<IReadOnlyList<Employee>> GetAllAsync(CancellationToken cancellationToken)
    {
        await _lock.WaitAsync(cancellationToken);
        try { return await ReadAsync(cancellationToken); }
        finally { _lock.Release(); }
    }

    public async Task<Employee?> GetByIdAsync(string employeeId, CancellationToken cancellationToken) =>
        (await GetAllAsync(cancellationToken)).FirstOrDefault(e => string.Equals(e.EmployeeId, employeeId, StringComparison.OrdinalIgnoreCase));

    public async Task AddAsync(Employee employee, CancellationToken cancellationToken)
    {
        await _lock.WaitAsync(cancellationToken);
        try { var employees = (await ReadAsync(cancellationToken)).ToList(); employees.Add(employee); await WriteAsync(employees, cancellationToken); }
        finally { _lock.Release(); }
    }

    public async Task<bool> UpdateAsync(Employee employee, CancellationToken cancellationToken)
    {
        await _lock.WaitAsync(cancellationToken);
        try { var employees = (await ReadAsync(cancellationToken)).ToList(); var index = employees.FindIndex(e => string.Equals(e.EmployeeId, employee.EmployeeId, StringComparison.OrdinalIgnoreCase)); if (index < 0) return false; employees[index] = employee; await WriteAsync(employees, cancellationToken); return true; }
        finally { _lock.Release(); }
    }

    public async Task<bool> DeleteAsync(string employeeId, CancellationToken cancellationToken)
    {
        await _lock.WaitAsync(cancellationToken);
        try { var employees = (await ReadAsync(cancellationToken)).ToList(); var removed = employees.RemoveAll(e => string.Equals(e.EmployeeId, employeeId, StringComparison.OrdinalIgnoreCase)) > 0; if (removed) await WriteAsync(employees, cancellationToken); return removed; }
        finally { _lock.Release(); }
    }

    private async Task<List<Employee>> ReadAsync(CancellationToken token)
    {
        if (!File.Exists(_path)) return [];
        await using var stream = File.OpenRead(_path);
        return await JsonSerializer.DeserializeAsync<List<Employee>>(stream, JsonOptions, token) ?? [];
    }

    private async Task WriteAsync(List<Employee> employees, CancellationToken token)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
        await using var stream = File.Create(_path);
        await JsonSerializer.SerializeAsync(stream, employees, JsonOptions, token);
    }
}
