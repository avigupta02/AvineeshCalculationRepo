namespace EmployeeApi.Models;

public sealed record Employee(string EmployeeId, string Name, string Email, string Department);

public sealed record EmployeeRequest(string? EmployeeId, string? Name, string? Email, string? Department)
{
    public Dictionary<string, string[]>? Validate()
    {
        var errors = new Dictionary<string, string[]>();
        AddRequired(errors, nameof(EmployeeId), EmployeeId);
        AddRequired(errors, nameof(Name), Name);
        AddRequired(errors, nameof(Department), Department);
        if (string.IsNullOrWhiteSpace(Email) || !System.Net.Mail.MailAddress.TryCreate(Email, out _))
            errors[nameof(Email)] = ["A valid email address is required."];
        return errors.Count == 0 ? null : errors;
    }

    private static void AddRequired(Dictionary<string, string[]> errors, string field, string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) errors[field] = ["This field is required."];
    }
}
