using System.CommandLine;
using System.Text;
using System.Text.Json;
using System.Net.Http.Json;

namespace PlatformManager;

class Program
{
    private static readonly HttpClient _httpClient = new HttpClient();
    private static readonly string _gatewayUrl = "http://localhost:5000/api/gateway";
    private static readonly string _platformApiUrl = "http://localhost:5007/api/platform";

    static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand(
            "Transport Platform Manager - Application-Based Architecture"
        );

        // Organization Commands
        var orgCommand = new Command("org", "Organization management");
        var orgListCommand = new Command("list", "List all organizations");
        var orgCreateCommand = new Command("create", "Create a new organization");
        var orgCreateNameOption = new Option<string>("--name", "Organization name")
        {
            IsRequired = true,
        };
        var orgCreateAddressOption = new Option<string>("--address", "Organization address")
        {
            IsRequired = true,
        };
        var orgCreatePhoneOption = new Option<string>("--phone", "Organization phone")
        {
            IsRequired = true,
        };
        var orgCreateEmailOption = new Option<string>("--email", "Organization email")
        {
            IsRequired = true,
        };
        var orgCreateTypeOption = new Option<string>("--type", "Organization type")
        {
            IsRequired = true,
        };
        var orgCreateAdminNameOption = new Option<string>("--admin-name", "Admin name")
        {
            IsRequired = true,
        };
        var orgCreateAdminEmailOption = new Option<string>("--admin-email", "Admin email")
        {
            IsRequired = true,
        };
        var orgCreateAdminPhoneOption = new Option<string>("--admin-phone", "Admin phone")
        {
            IsRequired = true,
        };

        orgCreateCommand.AddOption(orgCreateNameOption);
        orgCreateCommand.AddOption(orgCreateAddressOption);
        orgCreateCommand.AddOption(orgCreatePhoneOption);
        orgCreateCommand.AddOption(orgCreateEmailOption);
        orgCreateCommand.AddOption(orgCreateTypeOption);
        orgCreateCommand.AddOption(orgCreateAdminNameOption);
        orgCreateCommand.AddOption(orgCreateAdminEmailOption);
        orgCreateCommand.AddOption(orgCreateAdminPhoneOption);

        orgCreateCommand.SetHandler(
            async (name, address, phone, email, type, adminName, adminEmail, adminPhone) =>
            {
                await CreateOrganization(
                    name,
                    address,
                    phone,
                    email,
                    type,
                    adminName,
                    adminEmail,
                    adminPhone
                );
            },
            orgCreateNameOption,
            orgCreateAddressOption,
            orgCreatePhoneOption,
            orgCreateEmailOption,
            orgCreateTypeOption,
            orgCreateAdminNameOption,
            orgCreateAdminEmailOption,
            orgCreateAdminPhoneOption
        );

        orgListCommand.SetHandler(async () => await ListOrganizations());
        orgCommand.AddCommand(orgListCommand);
        orgCommand.AddCommand(orgCreateCommand);

        // User Commands
        var userCommand = new Command("user", "User management");
        var userListCommand = new Command("list", "List users");
        var userCreateCommand = new Command("create", "Create a new user");
        var userListOrgOption = new Option<string>("--org", "Organization name or alias");
        var userCreateOrgOption = new Option<string>("--org", "Organization name or alias")
        {
            IsRequired = true,
        };
        var userCreateTypeOption = new Option<string>(
            "--type",
            "User type (student/provider/admin/employee)"
        )
        {
            IsRequired = true,
        };
        var userCreateNameOption = new Option<string>("--name", "User name") { IsRequired = true };
        var userCreateEmailOption = new Option<string>("--email", "User email")
        {
            IsRequired = true,
        };
        var userCreatePhoneOption = new Option<string>("--phone", "User phone")
        {
            IsRequired = true,
        };

        userCreateCommand.AddOption(userCreateOrgOption);
        userCreateCommand.AddOption(userCreateTypeOption);
        userCreateCommand.AddOption(userCreateNameOption);
        userCreateCommand.AddOption(userCreateEmailOption);
        userCreateCommand.AddOption(userCreatePhoneOption);

        userListCommand.AddOption(userListOrgOption);
        userListCommand.SetHandler(async (org) => await ListUsers(org));
        userCreateCommand.SetHandler(
            async (org, type, name, email, phone) =>
            {
                await CreateUser(org, type, name, email, phone);
            },
            userCreateOrgOption,
            userCreateTypeOption,
            userCreateNameOption,
            userCreateEmailOption,
            userCreatePhoneOption
        );

        userCommand.AddCommand(userListCommand);
        userCommand.AddCommand(userCreateCommand);

        // Payment Commands
        var paymentCommand = new Command("payment", "Payment management");
        var paymentListCommand = new Command("list", "List payments");
        var paymentReverseCommand = new Command("reverse", "Reverse a payment");
        var paymentListOrgOption = new Option<string>("--org", "Organization name or alias");
        var paymentReverseTransactionOption = new Option<string>(
            "--transaction-id",
            "Transaction ID"
        )
        {
            IsRequired = true,
        };
        var paymentReverseReasonOption = new Option<string>("--reason", "Reversal reason")
        {
            IsRequired = true,
        };

        paymentListCommand.AddOption(paymentListOrgOption);
        paymentListCommand.SetHandler(async (org) => await ListPayments(org));
        paymentReverseCommand.AddOption(paymentReverseTransactionOption);
        paymentReverseCommand.AddOption(paymentReverseReasonOption);
        paymentReverseCommand.SetHandler(
            async (transactionId, reason) =>
            {
                await ReversePayment(transactionId, reason);
            },
            paymentReverseTransactionOption,
            paymentReverseReasonOption
        );

        paymentCommand.AddCommand(paymentListCommand);
        paymentCommand.AddCommand(paymentReverseCommand);

        // Booking Commands
        var bookingCommand = new Command("booking", "Booking management");
        var bookingListCommand = new Command("list", "List bookings");
        var bookingCancelCommand = new Command("cancel", "Cancel a booking");
        var bookingListOrgOption = new Option<string>("--org", "Organization name or alias");
        var bookingCancelIdOption = new Option<string>("--booking-id", "Booking ID")
        {
            IsRequired = true,
        };

        bookingListCommand.AddOption(bookingListOrgOption);
        bookingListCommand.SetHandler(async (org) => await ListBookings(org));
        bookingCancelCommand.AddOption(bookingCancelIdOption);
        bookingCancelCommand.SetHandler(async (bookingId) => await CancelBooking(bookingId));

        bookingCommand.AddCommand(bookingListCommand);
        bookingCommand.AddCommand(bookingCancelCommand);

        // Analytics Commands
        var analyticsCommand = new Command("analytics", "System analytics");
        var analyticsOrgOption = new Option<string>("--org", "Organization name or alias")
        {
            IsRequired = true,
        };
        analyticsCommand.AddOption(analyticsOrgOption);
        analyticsCommand.SetHandler(async (org) => await GetAnalytics(org));

        // Health Commands
        var healthCommand = new Command("health", "System health check");
        healthCommand.SetHandler(async () => await CheckHealth());

        // Emergency Commands
        var emergencyCommand = new Command("emergency", "Emergency operations");
        var emergencyStopCommand = new Command("stop", "Emergency system stop");
        emergencyStopCommand.SetHandler(async () => await EmergencyStop());
        emergencyCommand.AddCommand(emergencyStopCommand);

        rootCommand.AddCommand(orgCommand);
        rootCommand.AddCommand(userCommand);
        rootCommand.AddCommand(paymentCommand);
        rootCommand.AddCommand(bookingCommand);
        rootCommand.AddCommand(analyticsCommand);
        rootCommand.AddCommand(healthCommand);
        rootCommand.AddCommand(emergencyCommand);

        return await rootCommand.InvokeAsync(args);
    }

    static async Task CreateOrganization(
        string name,
        string address,
        string phone,
        string email,
        string type,
        string adminName,
        string adminEmail,
        string adminPhone
    )
    {
        try
        {
            var request = new
            {
                Name = name,
                Address = address,
                Phone = phone,
                Email = email,
                Type = type,
                AdminName = adminName,
                AdminEmail = adminEmail,
                AdminPhone = adminPhone,
            };

            var response = await _httpClient.PostAsJsonAsync(
                $"{_gatewayUrl}/organizations/register",
                request
            );
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"✅ Organization '{name}' created successfully");
                Console.WriteLine(result);
            }
            else
            {
                Console.WriteLine($"❌ Failed to create organization: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
        }
    }

    static async Task ListOrganizations()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_platformApiUrl}/organizations");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                Console.WriteLine("📋 Organizations:");
                Console.WriteLine(result);
            }
            else
            {
                Console.WriteLine($"❌ Failed to list organizations: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
        }
    }

    static async Task CreateUser(string org, string type, string name, string email, string phone)
    {
        try
        {
            object request;
            string endpoint;

            switch (type.ToLower())
            {
                case "student":
                    request = new
                    {
                        Name = name,
                        Email = email,
                        Phone = phone,
                        OrganizationName = org,
                        Grade = "10th",
                    };
                    endpoint = $"{_gatewayUrl}/students/register";
                    break;
                case "provider":
                    request = new
                    {
                        Name = name,
                        Email = email,
                        Phone = phone,
                        OrganizationName = org,
                        LicenseNumber = "LIC123",
                    };
                    endpoint = $"{_gatewayUrl}/providers/register";
                    break;
                case "admin":
                    request = new
                    {
                        Name = name,
                        Email = email,
                        Phone = phone,
                        OrganizationId = org,
                        Level = "Admin",
                    };
                    endpoint = $"{_gatewayUrl}/organizations/admins";
                    break;
                case "employee":
                    request = new
                    {
                        Name = name,
                        Email = email,
                        Phone = phone,
                        OrganizationId = org,
                        Department = "General",
                        Position = "Employee",
                    };
                    endpoint = $"{_gatewayUrl}/organizations/employees";
                    break;
                default:
                    Console.WriteLine($"❌ Invalid user type: {type}");
                    return;
            }

            var response = await _httpClient.PostAsJsonAsync(endpoint, request);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"✅ {type} '{name}' created successfully");
                Console.WriteLine(result);
            }
            else
            {
                Console.WriteLine($"❌ Failed to create {type}: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
        }
    }

    static async Task ListUsers(string? org)
    {
        try
        {
            var url = $"{_platformApiUrl}/users";
            if (!string.IsNullOrEmpty(org))
            {
                url += $"?org={org}";
            }

            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                Console.WriteLine("👥 Users:");
                Console.WriteLine(result);
            }
            else
            {
                Console.WriteLine($"❌ Failed to list users: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
        }
    }

    static async Task ListPayments(string? org)
    {
        try
        {
            var url = $"{_gatewayUrl}/payments/organization/{org ?? "all"}";
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                Console.WriteLine("💳 Payments:");
                Console.WriteLine(result);
            }
            else
            {
                Console.WriteLine($"❌ Failed to list payments: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
        }
    }

    static async Task ReversePayment(string transactionId, string reason)
    {
        try
        {
            var request = new { TransactionId = transactionId, Reason = reason };
            var response = await _httpClient.PostAsJsonAsync(
                $"{_gatewayUrl}/payments/reverse",
                request
            );
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"✅ Payment {transactionId} reversed successfully");
                Console.WriteLine(result);
            }
            else
            {
                Console.WriteLine($"❌ Failed to reverse payment: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
        }
    }

    static async Task ListBookings(string? org)
    {
        try
        {
            var url = $"{_platformApiUrl}/bookings";
            if (!string.IsNullOrEmpty(org))
            {
                url += $"?org={org}";
            }

            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                Console.WriteLine("📅 Bookings:");
                Console.WriteLine(result);
            }
            else
            {
                Console.WriteLine($"❌ Failed to list bookings: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
        }
    }

    static async Task CancelBooking(string bookingId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync(
                $"{_gatewayUrl}/students/bookings/{bookingId}"
            );
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"✅ Booking {bookingId} cancelled successfully");
            }
            else
            {
                Console.WriteLine($"❌ Failed to cancel booking: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
        }
    }

    static async Task GetAnalytics(string org)
    {
        try
        {
            var response = await _httpClient.GetAsync(
                $"{_gatewayUrl}/organizations/analytics/{org}"
            );
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"📊 Analytics for {org}:");
                Console.WriteLine(result);
            }
            else
            {
                Console.WriteLine($"❌ Failed to get analytics: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
        }
    }

    static async Task CheckHealth()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_gatewayUrl}/health");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                Console.WriteLine("🏥 System Health:");
                Console.WriteLine(result);
            }
            else
            {
                Console.WriteLine($"❌ Health check failed: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
        }
    }

    static async Task EmergencyStop()
    {
        try
        {
            Console.WriteLine("⚠️  WARNING: Emergency stop initiated!");
            Console.WriteLine("This will stop all transport operations.");
            Console.Write("Are you sure? (yes/no): ");
            var confirmation = Console.ReadLine();

            if (confirmation?.ToLower() == "yes")
            {
                var response = await _httpClient.PostAsync(
                    $"{_platformApiUrl}/emergency/stop",
                    null
                );
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("🛑 Emergency stop executed successfully");
                }
                else
                {
                    Console.WriteLine($"❌ Emergency stop failed: {response.StatusCode}");
                }
            }
            else
            {
                Console.WriteLine("❌ Emergency stop cancelled");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
        }
    }
}
