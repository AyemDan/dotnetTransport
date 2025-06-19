namespace API.Gateway;

public class ServiceSettings
{
    public string AuthService { get; set; } = "http://localhost:5001";
    public string ProviderService { get; set; } = "http://localhost:5002";
    public string OrganizationService { get; set; } = "http://localhost:5003";
    public string StudentService { get; set; } = "http://localhost:5004";
    public string RFIDCardService { get; set; } = "http://localhost:5005";
    public string SubscriptionService { get; set; } = "http://localhost:5006";
    public string AdminService { get; set; } = "http://localhost:5007";
} 