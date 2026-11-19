using EmployeeApi.Models;

namespace EmployeeApi.Repositories;

public interface IEmployeeRepository
{
    Task<IReadOnlyList<Employee>> GetAllAsync(CancellationToken cancellationToken);
    Task<Employee?> GetByIdAsync(string employeeId, CancellationToken cancellationToken);
    Task AddAsync(Employee employee, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(Employee employee, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(string employeeId, CancellationToken cancellationToken);
}
