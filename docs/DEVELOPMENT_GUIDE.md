# TwinsWins - Quick Development Guide

## 🚀 Get Started in 5 Minutes

### Prerequisites
- .NET 8.0 SDK
- Docker Desktop
- Your favorite IDE (VS Code, Visual Studio, or Rider)

### Quick Setup

1. **Start Infrastructure**
```bash
cd TwinsWins/infrastructure
docker-compose up -d
```

2. **Restore & Build**
```bash
cd ../
dotnet restore
dotnet build
```

3. **Run Database Migrations**
```bash
cd src/TwinsWins.Infrastructure
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate --startup-project ../TwinsWins.API
dotnet ef database update --startup-project ../TwinsWins.API
```

4. **Run the API**
```bash
cd ../TwinsWins.API
dotnet run
```

5. **Access Swagger**
Open: https://localhost:5000/swagger

## 📁 Project Overview

```
TwinsWins/
├── src/
│   ├── TwinsWins.API/           # Backend API (START HERE)
│   ├── TwinsWins.Core/          # Business logic & models
│   ├── TwinsWins.Infrastructure/ # Database & services
│   ├── TwinsWins.BlazorClient/  # Frontend (TO BE BUILT)
│   └── TwinsWins.Contracts/     # TON smart contracts (TO BE BUILT)
├── infrastructure/              # Docker configs
└── docs/                        # Documentation
```

## 🎯 What to Work On Next

### 1. Authentication System (Priority: HIGH)
**File:** `src/TwinsWins.API/Controllers/AuthController.cs`

Create endpoints for:
- `POST /api/auth/connect` - Connect TON wallet
- `POST /api/auth/verify` - Verify wallet signature
- `POST /api/auth/refresh` - Refresh JWT token

**Example Implementation:**
```csharp
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    [HttpPost("connect")]
    public async Task<IActionResult> Connect([FromBody] ConnectWalletRequest request)
    {
        // 1. Generate nonce
        // 2. Return nonce for client to sign
        // 3. Store nonce with expiration (5 min)
    }

    [HttpPost("verify")]
    public async Task<IActionResult> Verify([FromBody] VerifySignatureRequest request)
    {
        // 1. Validate signature
        // 2. Check nonce
        // 3. Create/get user
        // 4. Generate JWT token
        // 5. Return token + user info
    }
}
```

### 2. Blazor Client Game UI
**Folder:** `src/TwinsWins.BlazorClient/Pages/`

Create pages:
- `Game.razor` - Main game interface
- `Lobby.razor` - Game lobby browser
- `Profile.razor` - User profile & stats

**Key Components Needed:**
- Card component (displays image)
- Timer component (60 second countdown)
- Score display
- TON wallet connector

### 3. TON Smart Contracts
**Folder:** `src/TwinsWins.Contracts/`

Implement:
- Factory contract (deploys game instances)
- Game contract (escrow, payout logic)
- Test suite

### 4. Image Management
**File:** `src/TwinsWins.API/Controllers/ImageSetsController.cs`

Create endpoints for:
- Upload image pairs
- Create/edit image sets
- Manage difficulty levels

## 🧪 Testing

### Manual API Testing with Swagger

1. Start API: `dotnet run` in TwinsWins.API
2. Open: https://localhost:5000/swagger
3. Try endpoints (will need auth token first)

### Testing Without Auth (Temporary)

Comment out `[Authorize]` attributes in controllers to test without JWT:

```csharp
// [Authorize]  // <-- Comment this out
public class GamesController : ControllerBase
{
    // ... endpoints
}
```

### Creating Test Data

```bash
# Connect to PostgreSQL
docker exec -it twinswins-postgres psql -U postgres -d twinswins

# Insert test user
INSERT INTO "Users" ("Id", "WalletAddress", "Username", "AffiliateCode", "CreatedAt")
VALUES (gen_random_uuid(), 'EQTest123...', 'TestUser', 'TEST01', NOW());

# Insert test image set
INSERT INTO "ImageSets" ("Id", "Name", "Difficulty", "IsActive", "ImageStoragePath", "CreatedAt")
VALUES (gen_random_uuid(), 'Test Set', 0, true, '/images/test/', NOW());
```

## 🐛 Common Issues & Solutions

### Issue: "Connection refused" to PostgreSQL
**Solution:**
```bash
docker-compose -f infrastructure/docker-compose.yml restart postgres
```

### Issue: "Table doesn't exist"
**Solution:**
```bash
cd src/TwinsWins.Infrastructure
dotnet ef database update --startup-project ../TwinsWins.API
```

### Issue: "Unauthorized" on API calls
**Solution:** Implement auth system first, or temporarily remove `[Authorize]` attributes

### Issue: Can't connect to Redis
**Solution:**
```bash
docker-compose -f infrastructure/docker-compose.yml restart redis
```

## 📊 Monitoring During Development

### View Logs
```bash
# API logs
tail -f src/TwinsWins.API/logs/twinswins-*.log

# Docker logs
docker-compose -f infrastructure/docker-compose.yml logs -f
```

### Database Admin
```bash
# Connect with psql
docker exec -it twinswins-postgres psql -U postgres -d twinswins

# Or use GUI: pgAdmin, DBeaver, etc.
```

### Redis Inspection
```bash
# Connect with redis-cli
docker exec -it twinswins-redis redis-cli

# Commands:
# KEYS *           (list all keys)
# GET key          (get value)
# DEL key          (delete key)
```

### MinIO Console
Open: http://localhost:9001
Login: minioadmin / minioadmin

## 🔥 Hot Reload During Development

### API Hot Reload
```bash
cd src/TwinsWins.API
dotnet watch run
```

Changes to .cs files will automatically reload!

### Database Changes

After modifying entities:
```bash
cd src/TwinsWins.Infrastructure
dotnet ef migrations add YourMigrationName --startup-project ../TwinsWins.API
dotnet ef database update --startup-project ../TwinsWins.API
```

## 💡 Pro Tips

1. **Use REST Client in VS Code**
   Create `requests.http` file:
   ```http
   ### Create Free Game
   POST https://localhost:5000/api/games/free
   Content-Type: application/json
   Authorization: Bearer {{token}}

   {
     "imageSetId": "{{imageSetId}}"
   }
   ```

2. **Debug with VS Code**
   Press F5 to start debugging with breakpoints

3. **View EF Core Queries**
   Set logging level to Debug in appsettings.json:
   ```json
   "Microsoft.EntityFrameworkCore": "Debug"
   ```

4. **Quick Database Reset**
   ```bash
   cd src/TwinsWins.Infrastructure
   dotnet ef database drop --startup-project ../TwinsWins.API --force
   dotnet ef database update --startup-project ../TwinsWins.API
   ```

## 📚 Helpful Resources

- **EF Core Docs**: https://docs.microsoft.com/en-us/ef/core/
- **ASP.NET Core**: https://docs.microsoft.com/en-us/aspnet/core/
- **Blazor**: https://docs.microsoft.com/en-us/aspnet/core/blazor/
- **TON Docs**: https://docs.ton.org/
- **SignalR**: https://docs.microsoft.com/en-us/aspnet/core/signalr/

## 🎯 Daily Development Workflow

1. **Morning Setup**
   ```bash
   docker-compose -f infrastructure/docker-compose.yml start
   git pull
   dotnet restore
   ```

2. **During Development**
   ```bash
   # Terminal 1: Run API with hot reload
   cd src/TwinsWins.API
   dotnet watch run

   # Terminal 2: Run client (when ready)
   cd src/TwinsWins.BlazorClient
   dotnet watch run
   ```

3. **End of Day**
   ```bash
   git add .
   git commit -m "Your changes"
   git push
   docker-compose -f infrastructure/docker-compose.yml stop
   ```

## 🚨 Need Help?

1. Check `docs/IMPLEMENTATION_STATUS.md` for what's done
2. Review `README.md` for full documentation
3. Look at existing code for patterns
4. Check logs for error details

---

Happy Coding! 🎮
