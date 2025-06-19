# Start API Gateway with Authentication
Write-Host "🚀 Starting API Gateway with Authentication..." -ForegroundColor Green

# Set environment
$env:ASPNETCORE_ENVIRONMENT = "Development"

# Start API Gateway
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$PWD'; dotnet run --project src/API.Gateway/API.Gateway.csproj"

Write-Host "✅ API Gateway started on http://localhost:5274" -ForegroundColor Green
Write-Host "📖 Swagger UI: http://localhost:5274" -ForegroundColor Cyan
Write-Host "🔐 Auth endpoints:" -ForegroundColor Yellow
Write-Host "   - Register Student: POST /api/auth/register/student" -ForegroundColor White
Write-Host "   - Register Provider: POST /api/auth/register/provider" -ForegroundColor White
Write-Host "   - Register Organization: POST /api/auth/register/organization" -ForegroundColor White
Write-Host "   - Login: POST /api/auth/login" -ForegroundColor White
Write-Host "   - Profile: GET /api/auth/profile" -ForegroundColor White 