using System.CommandLine;
using System.Text.Json;
using PlatformManager.OrgAliases;

namespace PlatformManager.Commands;

public static class PaymentCommands
{
    public static Command CreatePaymentCommands(OrgAliasManager aliasManager)
    {
        var paymentCommand = new Command("payment", "Payment management commands");

        // Payment logs
        var listPaymentsCommand = new Command("list", "List payment logs")
        {
            new Argument<string>("organization", "Organization name or alias"),
            new Option<string>("--admin-level", "Required admin level (Super, Organization, School)") { DefaultValueFactory = () => "Organization" },
            new Option<string>("--status", "Filter by status (Pending, Completed, Failed)"),
            new Option<DateTime>("--from", "Filter from date"),
            new Option<DateTime>("--to", "Filter to date")
        };
        listPaymentsCommand.SetHandler(async (organization, adminLevel, status, from, to) =>
        {
            try
            {
                var orgName = aliasManager.GetOrganizationName(organization) ?? organization;
                using var client = new HttpClient();
                
                var queryParams = new List<string>
                {
                    $"organization={Uri.EscapeDataString(orgName)}",
                    $"adminLevel={Uri.EscapeDataString(adminLevel)}"
                };
                
                if (!string.IsNullOrEmpty(status))
                    queryParams.Add($"status={Uri.EscapeDataString(status)}");
                if (from != default)
                    queryParams.Add($"from={from:yyyy-MM-dd}");
                if (to != default)
                    queryParams.Add($"to={to:yyyy-MM-dd}");

                var url = $"http://localhost:5001/api/gateway/payments?{string.Join("&", queryParams)}";
                var response = await client.GetAsync(url);
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var payments = JsonSerializer.Deserialize<List<object>>(content);
                    Console.WriteLine($"Found {payments?.Count ?? 0} payments in '{orgName}'");
                }
                else
                {
                    Console.WriteLine($"✗ Error: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Error: {ex.Message}");
            }
        }, listPaymentsCommand.Arguments[0], listPaymentsCommand.Options[0], listPaymentsCommand.Options[1],
           listPaymentsCommand.Options[2], listPaymentsCommand.Options[3]);

        // Payment reversal
        var reversePaymentCommand = new Command("reverse", "Reverse a payment")
        {
            new Argument<string>("paymentId", "Payment ID to reverse"),
            new Argument<string>("organization", "Organization name or alias"),
            new Option<string>("--admin-level", "Required admin level (Super, Organization)") { DefaultValueFactory = () => "Organization" },
            new Option<string>("--reason", "Reason for reversal")
        };
        reversePaymentCommand.SetHandler(async (paymentId, organization, adminLevel, reason) =>
        {
            try
            {
                var orgName = aliasManager.GetOrganizationName(organization) ?? organization;
                var reversalData = new
                {
                    PaymentId = paymentId,
                    OrganizationName = orgName,
                    AdminLevel = adminLevel,
                    Reason = reason ?? "Admin reversal"
                };

                using var client = new HttpClient();
                var json = JsonSerializer.Serialize(reversalData);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                
                var response = await client.PostAsync("http://localhost:5001/api/gateway/payments/reverse", content);
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"✓ Payment '{paymentId}' reversed successfully");
                }
                else
                {
                    Console.WriteLine($"✗ Error: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Error: {ex.Message}");
            }
        }, reversePaymentCommand.Arguments[0], reversePaymentCommand.Arguments[1],
           reversePaymentCommand.Options[0], reversePaymentCommand.Options[1]);

        // Financial summary
        var financialSummaryCommand = new Command("summary", "Get financial summary")
        {
            new Argument<string>("organization", "Organization name or alias"),
            new Option<string>("--admin-level", "Required admin level (Super, Organization)") { DefaultValueFactory = () => "Organization" },
            new Option<DateTime>("--from", "Summary from date"),
            new Option<DateTime>("--to", "Summary to date")
        };
        financialSummaryCommand.SetHandler(async (organization, adminLevel, from, to) =>
        {
            try
            {
                var orgName = aliasManager.GetOrganizationName(organization) ?? organization;
                using var client = new HttpClient();
                
                var queryParams = new List<string>
                {
                    $"organization={Uri.EscapeDataString(orgName)}",
                    $"adminLevel={Uri.EscapeDataString(adminLevel)}"
                };
                
                if (from != default)
                    queryParams.Add($"from={from:yyyy-MM-dd}");
                if (to != default)
                    queryParams.Add($"to={to:yyyy-MM-dd}");

                var url = $"http://localhost:5001/api/gateway/payments/summary?{string.Join("&", queryParams)}";
                var response = await client.GetAsync(url);
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Financial Summary for '{orgName}':");
                    Console.WriteLine(content);
                }
                else
                {
                    Console.WriteLine($"✗ Error: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Error: {ex.Message}");
            }
        }, financialSummaryCommand.Arguments[0], financialSummaryCommand.Options[0],
           financialSummaryCommand.Options[1], financialSummaryCommand.Options[2]);

        paymentCommand.AddCommand(listPaymentsCommand);
        paymentCommand.AddCommand(reversePaymentCommand);
        paymentCommand.AddCommand(financialSummaryCommand);

        return paymentCommand;
    }
} 