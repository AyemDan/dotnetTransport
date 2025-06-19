using Transport.Domain.Entities;

public class Employee
{
    public Guid Id { get; set; }  // Change from int to Guid
    public string FullName { get; set; }
    public string? Email { get; set; }
    public string Position { get; set; }
    public string ContactNumber { get; set; }
    public Guid ProviderId { get; set; }  // Change from int to Guid
    public Provider Provider { get; set; }

    public Employee(string fullName, string position, string contactNumber)
    {
        FullName = fullName;
        Position = position;
        ContactNumber = contactNumber;
    }
}
