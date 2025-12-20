# A microservices-based application for managing professional licenses 

# Tenant Visit Home Page
# Tenant Login or Register
# Tenant Generate Licence
# Tenant get Email if License Expiring
# Tenang can download Licence


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

# Logs can be viewed 





# Not Implementing
# Caching
# Payment Service 
# 