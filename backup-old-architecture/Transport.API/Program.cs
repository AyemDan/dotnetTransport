using Microsoft.EntityFrameworkCore;
using Transport.Application.Services;
using Transport.Domain.Entities;
using Transport.Domain.Entities.MongoDB;
using Transport.Domain.Interfaces;
using Transport.Domain.Interfaces.MongoDB;
using Transport.Infrastructure.Data;
using Transport.Infrastructure.MongoDB;
using Transport.Infrastructure.Repositories;
using Transport.Infrastructure.Repositories.MongoDB;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder
    .Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter()
        );
    });
;
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add authentication and authorization services
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

// ✅ Register PostgresDbContext
builder.Services.AddDbContext<PostgresDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection"))
);

// ✅ Register IUserRepository with UserRepository
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Register MongoDBHelper
builder.Services.AddSingleton<MongoDBHelper>();

// Register MongoRepository for each entity
builder.Services.AddScoped<IMongoRepository<Trip>>(sp => new MongoRepository<Trip>(
    sp.GetRequiredService<MongoDBHelper>(),
    "Trips"
));
builder.Services.AddScoped<IMongoRepository<Booking>>(sp => new MongoRepository<Booking>(
    sp.GetRequiredService<MongoDBHelper>(),
    "Bookings"
));
builder.Services.AddScoped<IMongoRepository<Carpool>>(sp => new MongoRepository<Carpool>(
    sp.GetRequiredService<MongoDBHelper>(),
    "Carpools"
));
builder.Services.AddScoped<IMongoRepository<PaymentLog>>(sp => new MongoRepository<PaymentLog>(
    sp.GetRequiredService<MongoDBHelper>(),
    "Payments"
));
builder.Services.AddScoped<IMongoRepository<Notification>>(sp => new MongoRepository<Notification>(
    sp.GetRequiredService<MongoDBHelper>(),
    "Notifications"
));
builder.Services.AddScoped<IMongoRepository<Provider>>(sp => new MongoRepository<Provider>(
    sp.GetRequiredService<MongoDBHelper>(),
    "Providers"
));
builder.Services.AddScoped<IMongoRepository<Transport.Domain.Entities.MongoDB.Route>>(
    sp => new MongoRepository<Transport.Domain.Entities.MongoDB.Route>(
        sp.GetRequiredService<MongoDBHelper>(),
        "Routes"
    )
);
builder.Services.AddScoped<IMongoRepository<Subscription>>(sp => new MongoRepository<Subscription>(
    sp.GetRequiredService<MongoDBHelper>(),
    "Subscriptions"
));
builder.Services.AddScoped<IMongoRepository<RFIDCard>>(sp => new MongoRepository<RFIDCard>(
    sp.GetRequiredService<MongoDBHelper>(),
    "RFIDCards"
));
builder.Services.AddScoped<IMongoRepository<Driver>>(sp => new MongoRepository<Driver>(
    sp.GetRequiredService<MongoDBHelper>(),
    "Drivers"
));
builder.Services.AddScoped<IMongoRepository<TripAttendance>>(
    sp => new MongoRepository<TripAttendance>(
        sp.GetRequiredService<MongoDBHelper>(),
        "TripAttendances"
    )
);
builder.Services.AddScoped<IMongoRepository<ProviderDocument>>(
    sp => new MongoRepository<ProviderDocument>(
        sp.GetRequiredService<MongoDBHelper>(),
        "ProviderDocuments"
    )
);
builder.Services.AddScoped<IMongoRepository<ProviderInvite>>(
    sp => new MongoRepository<ProviderInvite>(
        sp.GetRequiredService<MongoDBHelper>(),
        "ProviderInvites"
    )
);

// Register new services
builder.Services.AddScoped<StudentService>();
builder.Services.AddScoped<ProviderService>(sp => new ProviderService(
    sp.GetRequiredService<IMongoRepository<Trip>>(),
    sp.GetRequiredService<IMongoRepository<Booking>>(),
    sp.GetRequiredService<IMongoRepository<Carpool>>(),
    sp.GetRequiredService<IMongoRepository<PaymentLog>>(),
    sp.GetRequiredService<IMongoRepository<Notification>>(),
    sp.GetRequiredService<IMongoRepository<Driver>>()
));
builder.Services.AddScoped<AdminService>();
builder.Services.AddScoped<RouteService>();
builder.Services.AddScoped<SubscriptionService>();
builder.Services.AddScoped<RFIDCardService>(sp => new RFIDCardService(
    sp.GetRequiredService<IMongoRepository<RFIDCard>>(),
    sp.GetRequiredService<IMongoRepository<TripAttendance>>(),
    sp.GetRequiredService<IMongoRepository<PaymentLog>>()
));

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
