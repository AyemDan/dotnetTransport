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
        var listPaymentsCommand = new Command("list", "List payment logs");
        var orgArg = new Argument<string>("organization", "Organization name or alias");
        var adminLevelOpt = new Option<string>("--admin-level", "Required admin level (Super, Organization, School)");
        adminLevelOpt.SetDefaultValue("Organization");
        var statusOpt = new Option<string>("--status", "Filter by status (Pending, Completed, Failed)");
        var fromOpt = new Option<DateTime>("--from", "Filter from date");
        var toOpt = new Option<DateTime>("--to", "Filter to date");
        listPaymentsCommand.AddArgument(orgArg);
        listPaymentsCommand.AddOption(adminLevelOpt);
        listPaymentsCommand.AddOption(statusOpt);
        listPaymentsCommand.AddOption(fromOpt);
        listPaymentsCommand.AddOption(toOpt);
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
        }, orgArg, adminLevelOpt, statusOpt, fromOpt, toOpt);

        // Payment reversal
        var reversePaymentCommand = new Command("reverse", "Reverse a payment");
        var paymentIdArg = new Argument<string>("paymentId", "Payment ID to reverse");
        var reverseOrgArg = new Argument<string>("organization", "Organization name or alias");
        var reverseAdminLevelOpt = new Option<string>("--admin-level", "Required admin level (Super, Organization)");
        reverseAdminLevelOpt.SetDefaultValue("Organization");
        var reasonOpt = new Option<string>("--reason", "Reason for reversal");
        reversePaymentCommand.AddArgument(paymentIdArg);
        reversePaymentCommand.AddArgument(reverseOrgArg);
        reversePaymentCommand.AddOption(reverseAdminLevelOpt);
        reversePaymentCommand.AddOption(reasonOpt);
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
        }, paymentIdArg, reverseOrgArg, reverseAdminLevelOpt, reasonOpt);

        // Financial summary
        var financialSummaryCommand = new Command("summary", "Get financial summary");
        var summaryOrgArg = new Argument<string>("organization", "Organization name or alias");
        var summaryAdminLevelOpt = new Option<string>("--admin-level", "Required admin level (Super, Organization)");
        summaryAdminLevelOpt.SetDefaultValue("Organization");
        var summaryFromOpt = new Option<DateTime>("--from", "Summary from date");
        var summaryToOpt = new Option<DateTime>("--to", "Summary to date");
        financialSummaryCommand.AddArgument(summaryOrgArg);
        financialSummaryCommand.AddOption(summaryAdminLevelOpt);
        financialSummaryCommand.AddOption(summaryFromOpt);
        financialSummaryCommand.AddOption(summaryToOpt);
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
        }, summaryOrgArg, summaryAdminLevelOpt, summaryFromOpt, summaryToOpt);

        paymentCommand.AddCommand(listPaymentsCommand);
        paymentCommand.AddCommand(reversePaymentCommand);
        paymentCommand.AddCommand(financialSummaryCommand);

        return paymentCommand;
    }
} 