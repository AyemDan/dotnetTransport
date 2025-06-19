using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Transport.Shared.Interfaces;
using Transport.Shared.Services;
using Transport.Shared.Repositories;
using Transport.Shared.Entities.MongoDB;

namespace Transport.Shared;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTransportServices(this IServiceCollection services, string mongoConnectionString, string databaseName)
    {
        // Configure MongoDB
        var mongoClient = new MongoClient(mongoConnectionString);
        var database = mongoClient.GetDatabase(databaseName);
        
        // Register MongoDB database
        services.AddSingleton<IMongoDatabase>(database);

        // Register repositories
        services.AddScoped<IMongoRepository<Booking>>(provider => 
            new MongoRepository<Booking>(database, "Bookings"));
        services.AddScoped<IMongoRepository<Route>>(provider => 
            new MongoRepository<Route>(database, "Routes"));
        services.AddScoped<IMongoRepository<Trip>>(provider => 
            new MongoRepository<Trip>(database, "Trips"));
        services.AddScoped<IMongoRepository<RFIDCard>>(provider => 
            new MongoRepository<RFIDCard>(database, "RFIDCards"));
        services.AddScoped<IMongoRepository<Subscription>>(provider => 
            new MongoRepository<Subscription>(database, "Subscriptions"));
        services.AddScoped<IMongoRepository<Notification>>(provider => 
            new MongoRepository<Notification>(database, "Notifications"));
        services.AddScoped<IMongoRepository<PaymentLog>>(provider => 
            new MongoRepository<PaymentLog>(database, "PaymentLogs"));

        // Register domain services
        services.AddScoped<IBookingService, BookingService>();
        services.AddScoped<IRouteService, RouteService>();
        services.AddScoped<ITripService, TripService>();
        services.AddScoped<IRFIDService, RFIDService>();
        services.AddScoped<ISubscriptionService, SubscriptionService>();
        services.AddScoped<IPaymentService, PaymentService>();

        return services;
    }
} 