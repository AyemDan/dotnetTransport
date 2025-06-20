# 🚌 Transport Application - Application-Based Architecture

A modern, scalable transport management system built with .NET 9 and MongoDB, designed using an **application-based architecture** that focuses on user-centric functionality rather than technical microservices.

## 🎯 **Architecture Overview**

### **3 Main Applications** (User-Centric)
1. **🎓 StudentApp** (Port 5001) - All student-related functionality
2. **🚌 ProviderApp** (Port 5002) - All provider and transport functionality  
3. **🏢 OrganizationApp** (Port 5004) - All admin and business functionality

### **3 Shared Services** (Cross-Cutting Concerns)
1. **🔐 AuthService** (Port 5003) - JWT authentication and user sessions
2. **📧 NotificationService** (Port 5005) - Cross-application messaging
3. **💳 PaymentService** (Port 5006) - Centralized payment processing

### **Infrastructure**
- **🌐 API Gateway** (Port 5000) - Central routing and orchestration
- **🛠️ Platform Manager** - CLI and API for system administration (Port 5007)

## 🏗️ **Architecture Benefits**

### **Application-Based Approach**
- **User-Centric**: Organized around user types (Student, Provider, Organization)
- **High Cohesion**: Related functionality grouped together
- **Low Coupling**: Minimal dependencies between applications
- **Scalability**: Each application can scale independently
- **Maintainability**: Easier to understand and modify

### **Shared Services Strategy**
- **Authentication**: Centralized JWT management
- **Notifications**: Cross-application messaging
- **Payments**: Centralized financial operations

## 📁 **Project Structure**

```
dotnetTransport/
├── src/                           # 🚌 Transport Applications
│   ├── StudentApp/                # 🎓 Student Application
│   │   └── StudentApp/
│   │       ├── Controllers/
│   │       │   └── StudentController.cs
│   │       ├── Program.cs
│   │       └── StudentApp.csproj
│   ├── ProviderApp/               # 🚌 Provider Application
│   │   └── ProviderApp/
│   │       ├── Controllers/
│   │       │   └── ProviderController.cs
│   │       ├── Program.cs
│   │       └── ProviderApp.csproj
│   ├── OrganizationApp/           # 🏢 Organization Application
│   │   └── OrganizationApp/
│   │       ├── Controllers/
│   │       │   └── OrganizationController.cs
│   │       ├── Program.cs
│   │       └── OrganizationApp.csproj
│   ├── SharedServices/            # 🔧 Shared Services
│   │   ├── AuthService/
│   │   ├── NotificationService/
│   │   ├── PaymentService/
│   │   └── Shared/                # Shared entities and DTOs
│   └── API.Gateway/               # 🌐 API Gateway
├── platform-manager/              # 🛠️ Platform Management (Separate)
│   ├── PlatformManager/           # CLI Management Tool
│   ├── PlatformManager.API/       # Platform Management API
│   └── PlatformManager.sln        # Platform Manager Solution
├── backup-old-architecture/       # 📦 Old architecture backup
└── TransportApp.sln               # Main Transport Solution
```

## 🚀 **Getting Started**

### **Prerequisites**
- .NET 9 SDK
- MongoDB (local or cloud)
- Visual Studio 2022 or VS Code

### **Quick Start**

1. **Clone and Build**
   ```bash
   git clone <repository-url>
   cd dotnetTransport
   dotnet build
   ```

2. **Start MongoDB**
   ```bash
   # Local MongoDB
   mongod
   
   # Or use MongoDB Atlas (cloud)
   ```

3. **Run Transport Applications**
   ```bash
   # Terminal 1: API Gateway
   cd src/API.Gateway
   dotnet run
   
   # Terminal 2: StudentApp
   cd src/StudentApp/StudentApp
   dotnet run
   
   # Terminal 3: ProviderApp
   cd src/ProviderApp/ProviderApp
   dotnet run
   
   # Terminal 4: OrganizationApp
   cd src/OrganizationApp/OrganizationApp
   dotnet run
   
   # Terminal 5: AuthService
   cd src/SharedServices/AuthService
   dotnet run
   
   # Terminal 6: NotificationService
   cd src/SharedServices/NotificationService
   dotnet run
   
   # Terminal 7: PaymentService
   cd src/SharedServices/PaymentService
   dotnet run
   ```

4. **Run Platform Manager (Optional)**
   ```bash
   # Terminal 8: Platform Manager API
   cd platform-manager/PlatformManager.API
   dotnet run
   
   # Terminal 9: Platform Manager CLI
   cd platform-manager/PlatformManager
   dotnet run
   ```

5. **Access Applications**
   - **API Gateway**: http://localhost:5000
   - **StudentApp**: http://localhost:5001
   - **ProviderApp**: http://localhost:5002
   - **AuthService**: http://localhost:5003
   - **OrganizationApp**: http://localhost:5004
   - **NotificationService**: http://localhost:5005
   - **PaymentService**: http://localhost:5006
   - **Platform Manager API**: http://localhost:5007

## 📋 **API Endpoints**

### **StudentApp** (Port 5001)
- `POST /api/students/register` - Register new student
- `GET /api/students/{id}` - Get student details
- `POST /api/students/bookings` - Create booking
- `GET /api/students/bookings/{studentId}` - Get student bookings
- `POST /api/students/rfid-cards` - Assign RFID card

### **ProviderApp** (Port 5002)
- `POST /api/providers/register` - Register new provider
- `POST /api/providers/routes` - Create route
- `GET /api/providers/routes/{providerId}` - Get provider routes
- `POST /api/providers/trips` - Create trip
- `GET /api/providers/trips/{providerId}` - Get provider trips

### **OrganizationApp** (Port 5004)
- `POST /api/organizations/register` - Register organization
- `POST /api/organizations/admins` - Add admin
- `POST /api/organizations/subscriptions` - Create subscription
- `GET /api/organizations/analytics/{orgId}` - Get analytics

### **AuthService** (Port 5003)
- `POST /api/auth/login` - User login
- `POST /api/auth/validate` - Validate token
- `POST /api/auth/refresh` - Refresh token
- `POST /api/auth/logout` - User logout

### **Shared Services**
- **NotificationService** (Port 5005): Cross-app messaging
- **PaymentService** (Port 5006): Payment processing

## 🛠️ **Platform Manager**

### **CLI Commands**
```bash
# Organization management
pm org list
pm org create --name "School Name" --address "123 Main St" --phone "+1234567890" --email "school@example.com" --type "School" --admin-name "Admin User" --admin-email "admin@school.com" --admin-phone "+1234567890"

# User management
pm user list --org "School Name"
pm user create --org "School Name" --type student --name "John Doe" --email "john@school.com" --phone "+1234567890"

# Payment management
pm payment list --org "School Name"
pm payment reverse --transaction-id "txn_123" --reason "Refund requested"

# Booking management
pm booking list --org "School Name"
pm booking cancel --booking-id "booking_123"

# System management
pm health
pm analytics --org "School Name"
pm emergency stop
```

### **Platform Manager API**
- `GET /api/platform/health` - System health check
- `GET /api/platform/analytics` - System-wide analytics
- `POST /api/platform/emergency/stop` - Emergency system stop
- `GET /api/platform/organizations` - List organizations
- `GET /api/platform/users` - List users
- `GET /api/platform/bookings` - List bookings

## 🔧 **Configuration**

### **Environment Variables**
```json
{
  "ConnectionStrings": {
    "MongoDB": "mongodb://localhost:27017"
  },
  "Jwt": {
    "Key": "YourSuperSecretKeyHereMakeItLongEnoughForSecurity",
    "Issuer": "TransportApp",
    "Audience": "TransportApp"
  }
}
```

### **Service Ports**
- API Gateway: 5000
- StudentApp: 5001
- ProviderApp: 5002
- AuthService: 5003
- OrganizationApp: 5004
- NotificationService: 5005
- PaymentService: 5006
- Platform Manager API: 5007

## 🧪 **Testing**

### **Health Checks**
```bash
# Check all services via Gateway
curl http://localhost:5000/api/gateway/health

# Individual service health
curl http://localhost:5001/health
curl http://localhost:5002/health
curl http://localhost:5003/health
curl http://localhost:5004/health
curl http://localhost:5005/health
curl http://localhost:5006/health
curl http://localhost:5007/api/platform/health
```

### **Sample API Calls**
```bash
# Register a student
curl -X POST http://localhost:5000/api/gateway/students/register \
  -H "Content-Type: application/json" \
  -d '{
    "name": "John Doe",
    "email": "john@school.com",
    "phone": "+1234567890",
    "organizationName": "School Name",
    "grade": "10th"
  }'

# Login
curl -X POST http://localhost:5000/api/gateway/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "john@school.com",
    "password": "password123"
  }'
```

## 🔒 **Security**

- **JWT Authentication**: Secure token-based authentication
- **Role-Based Access Control**: Student, Provider, Admin roles
- **Organization Scoping**: Data isolation per organization
- **Input Validation**: Comprehensive request validation
- **Error Handling**: Secure error responses

## 📊 **Monitoring & Analytics**

- **Health Checks**: All services provide health endpoints
- **Logging**: Structured logging across all applications
- **Analytics**: Organization-level analytics and reporting
- **Audit Trail**: Complete audit logging for admin operations

## 🚀 **Deployment**

### **Docker Support**
```bash
# Build and run with Docker Compose
docker-compose up -d
```

### **Production Considerations**
- Use environment-specific configuration
- Implement proper logging and monitoring
- Set up database backups
- Configure SSL/TLS certificates
- Implement rate limiting
- Set up CI/CD pipelines

## 🤝 **Contributing**

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests
5. Submit a pull request

## 📄 **License**

This project is licensed under the MIT License - see the LICENSE file for details.

## 🆘 **Support**

For support and questions:
- Create an issue in the repository
- Check the documentation
- Review the API documentation in Swagger UI

---

**🎯 Architecture Philosophy**: This system is designed around user applications rather than technical microservices, providing better cohesion, maintainability, and user experience while maintaining the benefits of distributed architecture.

**🧹 Clean Architecture**: Redundant services have been removed and functionality consolidated into the 3 main applications, with the platform manager separated for better organization.
