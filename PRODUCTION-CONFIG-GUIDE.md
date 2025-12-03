# 🚀 Production Configuration Guide

## 📋 Overview

This guide covers all production configuration settings for the .NET 9 Minimal API.

---

## 🔐 Security Configuration

### 1. **JWT Settings** (`Config/Jwt/appsettings.jwt.Production.json`)

```json
{
  "JwtSettings": {
    "SecretKey": "YOUR-SECURE-RANDOM-KEY-HERE",
    "Issuer": "SiraAPI",
    "Audience": "SiraClient",
    "TokenExpiryMinutes": 30
  }
}
```

**⚠️ Important:**
- Generate a **secure random key** at least 32 bytes long
- Use a key generator: `openssl rand -base64 32`
- Store in **Azure Key Vault** or **AWS Secrets Manager**
- Never commit the actual production key to source control

---

## 🗄️ Database Configuration

### **PostgreSQL Connection** (`Config/Database/appsettings.database.Production.json`)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=your-host;Port=5432;Database=SiraDB;Username=user;Password=pass;SSL Mode=Require"
  }
}
```

**Production Settings:**
- Use **SSL/TLS** connections (`SSL Mode=Require`)
- Use **managed database** services (Azure Database, AWS RDS)
- Enable **connection pooling**: `Pooling=true;Maximum Pool Size=100`
- Use **read replicas** for scalability
- Store credentials in **environment variables** or **secrets manager**

---

## 🌐 CORS Configuration

### **Allowed Origins** (`Config/Base/appsettings.Production.json`)

```json
{
  "Cors": {
    "AllowedOrigins": [ "https://yourdomain.com", "https://www.yourdomain.com" ],
    "AllowedMethods": [ "GET", "POST", "PUT", "DELETE", "OPTIONS" ],
    "AllowedHeaders": [ "*" ],
    "AllowCredentials": true
  }
}
```

**Production Checklist:**
- ✅ Specify **exact domain names** (no wildcards)
- ✅ Use **HTTPS only**
- ✅ Limit methods to what's needed
- ✅ Enable credentials for authenticated requests

---

## 📊 Logging Configuration

### **Serilog Production** (`Config/Serilog/appsettings.serilog.Production.json`)

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Warning",
      "Override": {
        "Microsoft": "Warning",
        "System": "Error"
      }
    },
    "WriteTo": [
      { "Name": "Console" },
      {
        "Name": "File",
        "Args": {
          "path": "Logs/log-.txt",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 30
        }
      }
    ]
  }
}
```

**Best Practices:**
- Use **Warning/Error** levels in production
- Integrate with **Application Insights**, **Seq**, or **ELK Stack**
- Set up **alerts** for critical errors
- Retain logs for **30-90 days**

---

## ⚡ Redis Cache Configuration

### **Production Redis** (`Config/Redis/appsettings.redis.Production.json`)

```json
{
  "RedisSettings": {
    "Enabled": true,
    "ConnectionString": "your-redis-host:6379,password=pass,ssl=true,abortConnect=false"
  }
}
```

**Production Setup:**
- Use **Azure Redis Cache**, **AWS ElastiCache**, or **Redis Cloud**
- Enable **SSL/TLS** connections
- Set `abortConnect=false` for resilience
- Configure **failover** and **clustering**

---

## 🛡️ Rate Limiting

### **Production Limits** (`Config/RateLimiting/appsettings.rate-limiting.Production.json`)

```json
{
  "RateLimiting": {
    "DefaultRequests": 50,
    "DefaultWindow": 60
  }
}
```

**Recommendations:**
- **Public APIs**: 50-100 requests/minute
- **Authenticated users**: 100-500 requests/minute
- **Admin endpoints**: Custom limits per endpoint
- Use **API Gateway** for advanced rate limiting

---

## ⏱️ Timeout Settings

### **Request Timeouts** (`Config/TimeOut/appsettings.timeout.Production.json`)

```json
{
  "TimeoutSettings": {
    "RequestTimeoutSeconds": 30
  },
  "SlowRequestThreshold": 5
}
```

---

## 🔧 Environment Variables (Recommended)

Instead of storing secrets in JSON files, use **environment variables**:

### **Linux/macOS:**
```bash
export JwtSettings__SecretKey="your-secret-key"
export ConnectionStrings__DefaultConnection="Host=..."
export RedisSettings__ConnectionString="redis-host:6379,password=..."
```

### **Windows:**
```powershell
$env:JwtSettings__SecretKey="your-secret-key"
$env:ConnectionStrings__DefaultConnection="Host=..."
$env:RedisSettings__ConnectionString="redis-host:6379,password=..."
```

### **Docker:**
```yaml
environment:
  - JwtSettings__SecretKey=your-secret-key
  - ConnectionStrings__DefaultConnection=Host=...
  - RedisSettings__ConnectionString=redis:6379
```

---

## 🚀 Deployment Checklist

### Before Deploying to Production:

#### Security
- [ ] Update JWT secret key to a secure random value
- [ ] Configure CORS with specific allowed origins (no wildcards)
- [ ] Enable HTTPS/SSL for all connections
- [ ] Store secrets in Azure Key Vault / AWS Secrets Manager
- [ ] Enable SSL for PostgreSQL connection
- [ ] Enable SSL for Redis connection

#### Database
- [ ] Update database connection string
- [ ] Run migrations: `dotnet ef database update`
- [ ] Set up database backups
- [ ] Configure connection pooling
- [ ] Test failover scenarios

#### Logging & Monitoring
- [ ] Set log level to Warning/Error
- [ ] Configure external logging (Application Insights, Seq, ELK)
- [ ] Set up health check endpoints
- [ ] Configure alerts for errors
- [ ] Enable performance monitoring

#### Performance
- [ ] Enable Redis caching
- [ ] Configure rate limiting
- [ ] Enable response compression
- [ ] Set appropriate timeout values
- [ ] Test under load

#### Infrastructure
- [ ] Set up load balancer
- [ ] Configure auto-scaling
- [ ] Set up CDN for static assets
- [ ] Configure firewall rules
- [ ] Set up DDoS protection

---

## 🐳 Docker Production Deployment

### Dockerfile (Production-Optimized)

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["API/API.csproj", "API/"]
RUN dotnet restore "API/API.csproj"
COPY . .
WORKDIR "/src/API"
RUN dotnet build "API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "API.dll"]
```

### Run with Docker:

```bash
# Build
docker build -t sira-api:latest .

# Run with environment variables
docker run -d \
  --name sira-api \
  -p 80:80 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e JwtSettings__SecretKey="your-secret-key" \
  -e ConnectionStrings__DefaultConnection="Host=db;Port=5432;Database=SiraDB;Username=user;Password=pass" \
  -e RedisSettings__ConnectionString="redis:6379" \
  sira-api:latest
```

---

## ☸️ Kubernetes Deployment

### Secrets Configuration:

```yaml
apiVersion: v1
kind: Secret
metadata:
  name: sira-api-secrets
type: Opaque
stringData:
  jwt-secret: "your-secret-key-base64"
  db-connection: "Host=postgres;Port=5432;Database=SiraDB;..."
  redis-connection: "redis:6379,password=..."
```

### Deployment:

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: sira-api
spec:
  replicas: 3
  selector:
    matchLabels:
      app: sira-api
  template:
    metadata:
      labels:
        app: sira-api
    spec:
      containers:
      - name: api
        image: sira-api:latest
        ports:
        - containerPort: 80
        env:
        - name: ASPNETCORE_ENVIRONMENT
          value: "Production"
        - name: JwtSettings__SecretKey
          valueFrom:
            secretKeyRef:
              name: sira-api-secrets
              key: jwt-secret
        - name: ConnectionStrings__DefaultConnection
          valueFrom:
            secretKeyRef:
              name: sira-api-secrets
              key: db-connection
```

---

## 🔍 Health Checks (TODO - Add if needed)

Consider adding health check endpoints:

```csharp
builder.Services.AddHealthChecks()
    .AddNpgSql(connectionString)
    .AddRedis(redisConnection);

app.MapHealthChecks("/health");
```

---

## 📈 Monitoring & Alerts

### Application Insights (Azure)

```bash
dotnet add package Microsoft.ApplicationInsights.AspNetCore
```

```csharp
builder.Services.AddApplicationInsightsTelemetry();
```

### Prometheus Metrics

```bash
dotnet add package prometheus-net.AspNetCore
```

---

## 🔑 Key Takeaways

1. **Never commit secrets** to source control
2. Use **environment variables** or **secrets managers**
3. Enable **SSL/TLS** for all connections
4. Set up **monitoring and alerts**
5. Test **failover scenarios**
6. Configure **auto-scaling** for high availability
7. Implement **circuit breakers** for external dependencies
8. Use **managed services** (Azure, AWS) when possible

---

## 📞 Support

For production issues, refer to:
- Application logs in `Logs/` directory
- Application Insights dashboard
- Health check endpoint: `/health`

**Status**: Ready for Production Deployment 🚀

