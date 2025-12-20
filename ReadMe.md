# A microservices-based application for managing professional licenses 

# UI FLOW
  - Tenant Visit Home Page
  - Tenant Login or Register
  - Tenant Generate Licence
  - Tenant get Email if License Expiring
  - Tenang can download Licence


## Technology Stack

- .NET 8.0
- PostgreSQL (5 separate databases)
- Entity Framework Core
- JWT Authentication
- Docker & Docker Compose




## Getting Started

### Prerequisites

- .NET 8.0 SDK
- Docker Desktop

### Setup Instructions

1. Clone the repository
2. Start PostgreSQL containers:
```bash
docker-compose up -d
```



3. Run database migrations (coming in next steps)
4. Start services

# MASTER DATA

 - Identity Service DB


```
 docker exec -it postgres_identity psql -U identityuser -d identitydb
 
 \dt

INSERT INTO "UserRoles" ("Id", "Name", "Description") VALUES
(1, 'TenantAdmin', 'Administrator of the tenant'),
(2, 'TenantUser', 'Regular user of the tenant');

\SELECT * FROM "UserRoles";

```

- http://localhost:5001/swagger

- /api/auth/Register

{
  "username": "mpandit",
  "email": "manish24879@gmail.com",
  "password": "password!123",
  "fullName": "Manish Pandit",
  "companyName": "Gov2Biz",
  "phoneNumber": "9022924879"
}

{
  "success": true,
  "message": "Registration successful",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJtcGFuZGl0IiwiZW1haWwiOiJtYW5pc2gyNDg3OUBnbWFpbC5jb20iLCJ1c2VySWQiOiIxIiwidGVuYW50SWQiOiIxIiwidGVuYW50Q29kZSI6IkdPVjk1NzYiLCJ1c2Vycm9sZSI6IlRlbmFudEFkbWluIiwiZnVsbE5hbWUiOiJNYW5pc2ggUGFuZGl0IiwianRpIjoiYTU2ZWM0ZjEtNjAyMi00MGU0LWJhZDItYmIzYmY3MTE5ZTA5IiwiZXhwIjoxNzY2MjE1NTkxLCJpc3MiOiJMaWNlbnNlTWFuYWdlbWVudFN5c3RlbSIsImF1ZCI6IkxpY2Vuc2VNYW5hZ2VtZW50U3lzdGVtIn0.UeKkP4cEr597cz-ERGpAhygSYPNzwH3_2s1UUFPKKh8",
  "user": {
    "userId": 1,
    "username": "mpandit",
    "email": "manish24879@gmail.com",
    "fullName": "Manish Pandit",
    "tenantCode": "GOV9576",
    "role": "TenantAdmin"
  }
}
 

 - /api/auth/Login

{
  "username": "mpandit",
  "password": "password!123",
  "tenantCode": "GOV9576"
}

 {
  "success": true,
  "message": "Login successful",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJtcGFuZGl0IiwiZW1haWwiOiJtYW5pc2gyNDg3OUBnbWFpbC5jb20iLCJ1c2VySWQiOiIxIiwidGVuYW50SWQiOiIxIiwidGVuYW50Q29kZSI6IkdPVjk1NzYiLCJ1c2Vycm9sZSI6IlRlbmFudEFkbWluIiwiZnVsbE5hbWUiOiJNYW5pc2ggUGFuZGl0IiwianRpIjoiNDhkMTJlOGQtOTZmOS00YTg3LWE2ODgtYTVlMzM3YjE5NzU4IiwiZXhwIjoxNzY2MjE1Njc4LCJpc3MiOiJMaWNlbnNlTWFuYWdlbWVudFN5c3RlbSIsImF1ZCI6IkxpY2Vuc2VNYW5hZ2VtZW50U3lzdGVtIn0.LOuX90RZEJTNq_CEuZaAiDib1K0H-jKUemZm8zjs4fc",
  "user": {
    "userId": 1,
    "username": "mpandit",
    "email": "manish24879@gmail.com",
    "fullName": "Manish Pandit",
    "tenantCode": "GOV9576",
    "role": "TenantAdmin"
  }
}



# Hangfire
- curl -X POST http://localhost:5002/api/job/setup-daily-check
- http://localhost:5002/hangfire/




Step 1: Start PostgreSQL Databases
bash# Navigate to project root
cd ~/licencemanagement

# Start all 5 PostgreSQL containers
docker compose up -d

# Wait for databases to initialize
sleep 15

# Verify containers are running
docker ps

Step 2: Start IdentityService
bash# Open a NEW terminal window/tab
cd ~/licencemanagement/IdentityService

# Run the service
dotnet run

# You should see: "Now listening on: http://localhost:5001"
# Keep this terminal open

Step 3: Start LicenseService
bash# Open a NEW terminal window/tab
cd ~/licencemanagement/LicenseService

# Run the service
dotnet run

# You should see: "Now listening on: http://localhost:5002"
# Keep this terminal open

Step 4: Start PaymentService
bash# Open a NEW terminal window/tab
cd ~/licencemanagement/PaymentService

# Run the service
dotnet run

# You should see: "Now listening on: http://localhost:5003"
# Keep this terminal open

Step 5: Start DocumentService
bash# Open a NEW terminal window/tab
cd ~/licencemanagement/DocumentService

# Run the service
dotnet run

# You should see: "Now listening on: http://localhost:5004"
# Keep this terminal open

Step 6: Start NotificationService
bash# Open a NEW terminal window/tab
cd ~/licencemanagement/NotificationService

# Run the service
dotnet run

# You should see: "Now listening on: http://localhost:5005"
# Keep this terminal open

Step 7: Start WebApp/FrontEnd (MVC)
bash# Open a NEW terminal window/tab
cd ~/licencemanagement/FrontEnd
# OR
cd ~/licencemanagement/WebApp

# Run the application
dotnet run

# You should see: "Now listening on: http://localhost:5000"
# Keep this terminal open
```

---

## ✅ **Verification**

After all services are running, open your browser:
```
http://localhost:5000
You should see the login page!

📊 Quick Status Check
Run this in a new terminal to verify all ports:
bash# macOS/Linux
lsof -i :5000 -i :5001 -i :5002 -i :5003 -i :5004 -i :5005

# Expected output: You should see processes on all 6 ports

🔍 Access Swagger UIs
bash# IdentityService
open http://localhost:5001/swagger

# LicenseService
open http://localhost:5002/swagger

# PaymentService
open http://localhost:5003/swagger

# DocumentService
open http://localhost:5004/swagger

# NotificationService
open http://localhost:5005/swagger

# Hangfire Dashboard
open http://localhost:5002/hangfire

🛑 Stop All Services
When you're done, press Ctrl+C in each terminal window, then:
bash# Stop Docker containers
docker compose down

# Or to stop and remove volumes (clean slate)
docker compose down -v

💡 Using VS Code
If you're using VS Code, you can split terminals:

Open VS Code
Terminal → New Terminal (repeat 7 times)
Split terminals (click split icon)
Run each dotnet run command in a separate split



# Design
# Identity Service - this will take care of user authentication and authorization
# Payment Service - this will handle payments (razorpay, stripe)- Currently not implemented
# Notification Service - this will notify users (email, sms, whatsapp) currently only email
# LicenseService - this will handle creation of licence, expiry of licence and all things related to licence
# DocumentService - this will handle downloading of Documents for now
# Frontend- Screens for Home , Login , Register, Dashboard, Licence Request 

# as role based is asked (currently 2 users are created with hardcoded roles)
# tenantadmin role can generate licence 
# tenantsubuser role can download license

# FrontEnd - Asp.net MVC 
# Database - Postgresql
# Architecture based on Clean
 # Domain - Will Store Entity
 # Application - Will Store Use Cases 
 # Infrastructure  - Will have actual implementation and call Database 




# Not Implementing
# Caching
# Payment Service 
# Logging
# Exception Handling
# Test Project (Due to Time Constraints)