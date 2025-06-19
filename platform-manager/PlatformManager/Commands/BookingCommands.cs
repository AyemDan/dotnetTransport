using System.CommandLine;
using System.Text.Json;
using PlatformManager.OrgAliases;

namespace PlatformManager.Commands;

public static class BookingCommands
{
    public static Command CreateBookingCommands(OrgAliasManager aliasManager)
    {
        var bookingCommand = new Command("booking", "Booking and route management commands");

        // Route commands
        var createRouteCommand = new Command("create-route", "Create a new transport route")
        {
            new Argument<string>("name", "Route name"),
            new Argument<string>("organization", "Organization name or alias"),
            new Argument<string>("startLocation", "Starting location"),
            new Argument<string>("endLocation", "Ending location"),
            new Option<string>("--provider", "Provider name"),
            new Option<decimal>("--fare", "Route fare amount"),
            new Option<string>("--schedule", "Route schedule (e.g., 'Mon-Fri 7:00 AM')")
        };
        createRouteCommand.SetHandler(async (name, organization, startLocation, endLocation, provider, fare, schedule) =>
        {
            try
            {
                var orgName = aliasManager.GetOrganizationName(organization) ?? organization;
                var routeData = new
                {
                    Name = name,
                    OrganizationName = orgName,
                    StartLocation = startLocation,
                    EndLocation = endLocation,
                    ProviderName = provider,
                    Fare = fare,
                    Schedule = schedule
                };

                using var client = new HttpClient();
                var json = JsonSerializer.Serialize(routeData);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                
                var response = await client.PostAsync("http://localhost:5001/api/gateway/routes", content);
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"✓ Route '{name}' created successfully in '{orgName}'");
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
        }, createRouteCommand.Arguments[0], createRouteCommand.Arguments[1], createRouteCommand.Arguments[2],
           createRouteCommand.Arguments[3], createRouteCommand.Options[0], createRouteCommand.Options[1], createRouteCommand.Options[2]);

        var listRoutesCommand = new Command("list-routes", "List routes in an organization")
        {
            new Argument<string>("organization", "Organization name or alias")
        };
        listRoutesCommand.SetHandler(async (organization) =>
        {
            try
            {
                var orgName = aliasManager.GetOrganizationName(organization) ?? organization;
                using var client = new HttpClient();
                var response = await client.GetAsync($"http://localhost:5001/api/gateway/routes?organization={Uri.EscapeDataString(orgName)}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var routes = JsonSerializer.Deserialize<List<object>>(content);
                    Console.WriteLine($"Found {routes?.Count ?? 0} routes in '{orgName}'");
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
        }, listRoutesCommand.Arguments[0]);

        // Booking commands
        var createBookingCommand = new Command("create", "Create a new booking")
        {
            new Argument<string>("studentEmail", "Student email"),
            new Argument<string>("routeId", "Route ID"),
            new Argument<string>("organization", "Organization name or alias"),
            new Argument<DateTime>("date", "Booking date"),
            new Option<string>("--pickupTime", "Pickup time"),
            new Option<string>("--notes", "Booking notes")
        };
        createBookingCommand.SetHandler(async (studentEmail, routeId, organization, date, pickupTime, notes) =>
        {
            try
            {
                var orgName = aliasManager.GetOrganizationName(organization) ?? organization;
                var bookingData = new
                {
                    StudentEmail = studentEmail,
                    RouteId = routeId,
                    OrganizationName = orgName,
                    Date = date,
                    PickupTime = pickupTime,
                    Notes = notes
                };

                using var client = new HttpClient();
                var json = JsonSerializer.Serialize(bookingData);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                
                var response = await client.PostAsync("http://localhost:5001/api/gateway/bookings", content);
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"✓ Booking created successfully for student '{studentEmail}' on route '{routeId}'");
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
        }, createBookingCommand.Arguments[0], createBookingCommand.Arguments[1], createBookingCommand.Arguments[2],
           createBookingCommand.Arguments[3], createBookingCommand.Options[0], createBookingCommand.Options[1]);

        var listBookingsCommand = new Command("list", "List bookings")
        {
            new Argument<string>("organization", "Organization name or alias"),
            new Option<string>("--status", "Filter by status (Pending, Confirmed, Completed, Cancelled)"),
            new Option<DateTime>("--date", "Filter by date"),
            new Option<string>("--student", "Filter by student email")
        };
        listBookingsCommand.SetHandler(async (organization, status, date, student) =>
        {
            try
            {
                var orgName = aliasManager.GetOrganizationName(organization) ?? organization;
                using var client = new HttpClient();
                
                var queryParams = new List<string>
                {
                    $"organization={Uri.EscapeDataString(orgName)}"
                };
                
                if (!string.IsNullOrEmpty(status))
                    queryParams.Add($"status={Uri.EscapeDataString(status)}");
                if (date != default)
                    queryParams.Add($"date={date:yyyy-MM-dd}");
                if (!string.IsNullOrEmpty(student))
                    queryParams.Add($"student={Uri.EscapeDataString(student)}");

                var url = $"http://localhost:5001/api/gateway/bookings?{string.Join("&", queryParams)}";
                var response = await client.GetAsync(url);
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var bookings = JsonSerializer.Deserialize<List<object>>(content);
                    Console.WriteLine($"Found {bookings?.Count ?? 0} bookings in '{orgName}'");
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
        }, listBookingsCommand.Arguments[0], listBookingsCommand.Options[0],
           listBookingsCommand.Options[1], listBookingsCommand.Options[2]);

        var cancelBookingCommand = new Command("cancel", "Cancel a booking")
        {
            new Argument<string>("bookingId", "Booking ID to cancel"),
            new Argument<string>("organization", "Organization name or alias"),
            new Option<string>("--reason", "Cancellation reason")
        };
        cancelBookingCommand.SetHandler(async (bookingId, organization, reason) =>
        {
            try
            {
                var orgName = aliasManager.GetOrganizationName(organization) ?? organization;
                var cancelData = new
                {
                    BookingId = bookingId,
                    OrganizationName = orgName,
                    Reason = reason ?? "Admin cancellation"
                };

                using var client = new HttpClient();
                var json = JsonSerializer.Serialize(cancelData);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                
                var response = await client.PostAsync("http://localhost:5001/api/gateway/bookings/cancel", content);
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"✓ Booking '{bookingId}' cancelled successfully");
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
        }, cancelBookingCommand.Arguments[0], cancelBookingCommand.Arguments[1], cancelBookingCommand.Options[0]);

        bookingCommand.AddCommand(createRouteCommand);
        bookingCommand.AddCommand(listRoutesCommand);
        bookingCommand.AddCommand(createBookingCommand);
        bookingCommand.AddCommand(listBookingsCommand);
        bookingCommand.AddCommand(cancelBookingCommand);

        return bookingCommand;
    }
} 