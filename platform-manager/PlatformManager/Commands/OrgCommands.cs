using System.CommandLine;
using System.Text.Json;
using PlatformManager.OrgAliases;

namespace PlatformManager.Commands;

public static class OrgCommands
{
    public static Command CreateOrgCommands(OrgAliasManager aliasManager)
    {
        var orgCommand = new Command("org", "Organization management commands");

        // Alias commands
        var addAliasCommand = new Command("alias-add", "Add an organization alias")
        {
            new Argument<string>("alias", "Short alias for the organization"),
            new Argument<string>("organization", "Full organization name")
        };
        addAliasCommand.SetHandler((alias, organization) =>
        {
            try
            {
                aliasManager.AddAlias(alias, organization);
                Console.WriteLine($"✓ Alias '{alias}' added for organization '{organization}'");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Error: {ex.Message}");
            }
        }, addAliasCommand.Arguments[0], addAliasCommand.Arguments[1]);

        var removeAliasCommand = new Command("alias-remove", "Remove an organization alias")
        {
            new Argument<string>("alias", "Alias to remove")
        };
        removeAliasCommand.SetHandler((alias) =>
        {
            aliasManager.RemoveAlias(alias);
            Console.WriteLine($"✓ Alias '{alias}' removed");
        }, removeAliasCommand.Arguments[0]);

        var listAliasesCommand = new Command("alias-list", "List all organization aliases");
        listAliasesCommand.SetHandler(() =>
        {
            aliasManager.ListAliases();
        });

        // Organization operations
        var listOrgsCommand = new Command("list", "List all organizations");
        listOrgsCommand.SetHandler(async () =>
        {
            try
            {
                using var client = new HttpClient();
                var response = await client.GetAsync("http://localhost:5001/api/gateway/organizations");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var organizations = JsonSerializer.Deserialize<List<object>>(content);
                    Console.WriteLine($"Found {organizations?.Count ?? 0} organizations");
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
        });

        var createOrgCommand = new Command("create", "Create a new organization")
        {
            new Argument<string>("name", "Organization name"),
            new Argument<string>("address", "Organization address"),
            new Option<string>("--phone", "Phone number"),
            new Option<string>("--email", "Email address")
        };
        createOrgCommand.SetHandler(async (name, address, phone, email) =>
        {
            try
            {
                var orgData = new
                {
                    Name = name,
                    Address = address,
                    Phone = phone,
                    Email = email
                };

                using var client = new HttpClient();
                var json = JsonSerializer.Serialize(orgData);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                
                var response = await client.PostAsync("http://localhost:5001/api/gateway/organizations", content);
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"✓ Organization '{name}' created successfully");
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
        }, createOrgCommand.Arguments[0], createOrgCommand.Arguments[1], 
           createOrgCommand.Options[0], createOrgCommand.Options[1]);

        orgCommand.AddCommand(addAliasCommand);
        orgCommand.AddCommand(removeAliasCommand);
        orgCommand.AddCommand(listAliasesCommand);
        orgCommand.AddCommand(listOrgsCommand);
        orgCommand.AddCommand(createOrgCommand);

        return orgCommand;
    }
} 