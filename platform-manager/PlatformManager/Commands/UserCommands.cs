using System.CommandLine;
using System.Text.Json;
using PlatformManager.OrgAliases;

namespace PlatformManager.Commands;

public static class UserCommands
{
    public static Command CreateUserCommands(OrgAliasManager aliasManager)
    {
        var userCommand = new Command("user", "User management commands");

        // Student commands
        var createStudentCommand = new Command("create-student", "Create a new student")
        {
            new Argument<string>("name", "Student name"),
            new Argument<string>("email", "Student email"),
            new Argument<string>("organization", "Organization name or alias"),
            new Option<string>("--phone", "Phone number"),
            new Option<string>("--grade", "Student grade level")
        };
        createStudentCommand.SetHandler(async (name, email, organization, phone, grade) =>
        {
            try
            {
                var orgName = aliasManager.GetOrganizationName(organization) ?? organization;
                var studentData = new
                {
                    Name = name,
                    Email = email,
                    OrganizationName = orgName,
                    Phone = phone,
                    Grade = grade
                };

                using var client = new HttpClient();
                var json = JsonSerializer.Serialize(studentData);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                
                var response = await client.PostAsync("http://localhost:5001/api/gateway/students", content);
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"✓ Student '{name}' created successfully in '{orgName}'");
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
        }, createStudentCommand.Arguments[0], createStudentCommand.Arguments[1], createStudentCommand.Arguments[2],
           createStudentCommand.Options[0], createStudentCommand.Options[1]);

        var listStudentsCommand = new Command("list-students", "List students in an organization")
        {
            new Argument<string>("organization", "Organization name or alias")
        };
        listStudentsCommand.SetHandler(async (organization) =>
        {
            try
            {
                var orgName = aliasManager.GetOrganizationName(organization) ?? organization;
                using var client = new HttpClient();
                var response = await client.GetAsync($"http://localhost:5001/api/gateway/students?organization={Uri.EscapeDataString(orgName)}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var students = JsonSerializer.Deserialize<List<object>>(content);
                    Console.WriteLine($"Found {students?.Count ?? 0} students in '{orgName}'");
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
        }, listStudentsCommand.Arguments[0]);

        // Provider commands
        var createProviderCommand = new Command("create-provider", "Create a new transport provider")
        {
            new Argument<string>("name", "Provider name"),
            new Argument<string>("email", "Provider email"),
            new Argument<string>("organization", "Organization name or alias"),
            new Option<string>("--phone", "Phone number"),
            new Option<string>("--license", "Transport license number")
        };
        createProviderCommand.SetHandler(async (name, email, organization, phone, license) =>
        {
            try
            {
                var orgName = aliasManager.GetOrganizationName(organization) ?? organization;
                var providerData = new
                {
                    Name = name,
                    Email = email,
                    OrganizationName = orgName,
                    Phone = phone,
                    LicenseNumber = license
                };

                using var client = new HttpClient();
                var json = JsonSerializer.Serialize(providerData);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                
                var response = await client.PostAsync("http://localhost:5001/api/gateway/providers", content);
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"✓ Provider '{name}' created successfully in '{orgName}'");
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
        }, createProviderCommand.Arguments[0], createProviderCommand.Arguments[1], createProviderCommand.Arguments[2],
           createProviderCommand.Options[0], createProviderCommand.Options[1]);

        var listProvidersCommand = new Command("list-providers", "List providers in an organization")
        {
            new Argument<string>("organization", "Organization name or alias")
        };
        listProvidersCommand.SetHandler(async (organization) =>
        {
            try
            {
                var orgName = aliasManager.GetOrganizationName(organization) ?? organization;
                using var client = new HttpClient();
                var response = await client.GetAsync($"http://localhost:5001/api/gateway/providers?organization={Uri.EscapeDataString(orgName)}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var providers = JsonSerializer.Deserialize<List<object>>(content);
                    Console.WriteLine($"Found {providers?.Count ?? 0} providers in '{orgName}'");
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
        }, listProvidersCommand.Arguments[0]);

        // Admin commands
        var createAdminCommand = new Command("create-admin", "Create a new admin user")
        {
            new Argument<string>("name", "Admin name"),
            new Argument<string>("email", "Admin email"),
            new Argument<string>("organization", "Organization name or alias"),
            new Option<string>("--phone", "Phone number")
        };
        var levelOpt = new Option<string>("--level", "Admin level (Super, Organization, School)");
        levelOpt.SetDefaultValue("School");
        createAdminCommand.AddOption(levelOpt);
        createAdminCommand.SetHandler(async (name, email, organization, phone) =>
        {
            try
            {
                var orgName = aliasManager.GetOrganizationName(organization) ?? organization;
                var adminData = new
                {
                    Name = name,
                    Email = email,
                    OrganizationName = orgName,
                    AdminLevel = levelOpt.Value,
                    Phone = phone
                };

                using var client = new HttpClient();
                var json = JsonSerializer.Serialize(adminData);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                
                var response = await client.PostAsync("http://localhost:5001/api/gateway/admins", content);
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"✓ Admin '{name}' created successfully in '{orgName}' with level '{levelOpt.Value}'");
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
        }, createAdminCommand.Arguments[0], createAdminCommand.Arguments[1], createAdminCommand.Arguments[2],
           createAdminCommand.Options[0]);

        userCommand.AddCommand(createStudentCommand);
        userCommand.AddCommand(listStudentsCommand);
        userCommand.AddCommand(createProviderCommand);
        userCommand.AddCommand(listProvidersCommand);
        userCommand.AddCommand(createAdminCommand);

        return userCommand;
    }
} 