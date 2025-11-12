# TwinsWins - Image Pair Matching Game

A blockchain-integrated memory game where players find logical connections between image pairs, with free practice and paid competitive modes.

## 🚀 Quick Start

### Prerequisites
- .NET 8.0 SDK
- Docker & Docker Compose
- Node.js 18+ (for Blazor client)
- PostgreSQL 15+
- Redis 7+

### Development Setup

1. **Clone and Setup**
```bash
cd /home/claude/TwinsWins
```

2. **Start Infrastructure with Docker Compose**
```bash
cd infrastructure
docker-compose up -d
```

This will start:
- PostgreSQL on port 5432
- Redis on port 6379
- MinIO (S3-compatible storage) on port 9000

3. **Configure Application**

Create `appsettings.Development.json` in src/TwinsWins.API:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=twinswins;Username=postgres;Password=postgres",
    "Redis": "localhost:6379"
  },
  "TON": {
    "Network": "testnet",
    "TreasuryWallet": "YOUR_TREASURY_WALLET_ADDRESS",
    "ContractAddress": "YOUR_FACTORY_CONTRACT_ADDRESS"
  },
  "Storage": {
    "Type": "MinIO",
    "Endpoint": "localhost:9000",
    "AccessKey": "minioadmin",
    "SecretKey": "minioadmin",
    "BucketName": "twinswins-images"
  }
}
```

4. **Apply Database Migrations**
```bash
cd src/TwinsWins.API
dotnet ef database update
```

5. **Run the Application**
```bash
# Terminal 1: API
cd src/TwinsWins.API
dotnet run

# Terminal 2: Blazor Client
cd src/TwinsWins.BlazorClient
dotnet run
```

6. **Access the Application**
- API: http://localhost:5000
- Swagger: http://localhost:5000/swagger
- Client: http://localhost:5001

## 📁 Project Structure

```
TwinsWins/
├── src/
│   ├── TwinsWins.API/              # ASP.NET Core Web API
│   │   ├── Controllers/            # API endpoints
│   │   ├── Hubs/                   # SignalR hubs
│   │   └── Middleware/             # Custom middleware
│   ├── TwinsWins.Core/             # Domain layer
│   │   ├── Entities/               # Domain models
│   │   ├── Enums/                  # Enumerations
│   │   ├── Interfaces/             # Service interfaces
│   │   └── DTOs/                   # Data transfer objects
│   ├── TwinsWins.Infrastructure/   # Data access layer
│   │   ├── Data/                   # DbContext, migrations
│   │   ├── Services/               # Service implementations
│   │   └── Repositories/           # Data repositories
│   ├── TwinsWins.BlazorClient/     # Blazor WebAssembly client
│   │   ├── Pages/                  # Razor pages
│   │   ├── Components/             # Reusable components
│   │   └── State/                  # Fluxor state management
│   └── TwinsWins.Contracts/        # TON smart contracts
├── tests/                          # Test projects
├── docs/                           # Documentation
├── scripts/                        # Utility scripts
└── infrastructure/                 # Docker configs
```

## 🎮 Game Features

### Free Game Mode
- Practice without staking TON
- Full game mechanics
- Statistics tracking
- Skill rating calculation

### Paid Game Mode
- Stake TON currency
- Competitive matchmaking
- Winner takes pot (minus fees)
- Smart contract escrow

### Core Mechanics
- 18 cards (9 pairs) displayed face-up
- Find logical connections between pairs
- 60-second time limit
- Dynamic scoring based on speed and accuracy
- Combo multipliers for streaks

### Scoring Formula
```csharp
basePoints = 100 (correct) or -50 (incorrect)
timeBonus = (60 - elapsedSeconds) * 10  // 0-600 points
comboMultiplier = 1.0 + (0.1 * consecutiveCorrectPairs)
finalPoints = (basePoints + timeBonus) * comboMultiplier
```

## 🔧 Technology Stack

### Backend
- **Framework**: ASP.NET Core 8.0
- **Database**: PostgreSQL 15 with EF Core
- **Cache**: Redis
- **Real-time**: SignalR
- **Blockchain**: TON
- **Storage**: MinIO (S3-compatible)

### Frontend
- **Framework**: Blazor WebAssembly
- **State**: Fluxor
- **UI**: MudBlazor / TailwindCSS
- **PWA**: Service Workers

### Infrastructure
- **Containerization**: Docker
- **Orchestration**: Kubernetes (production)
- **Monitoring**: Prometheus + Grafana
- **Logging**: Serilog

## 🔐 Security Features

- TON wallet authentication with nonce-based replay protection
- Server-side game validation
- Anti-cheat mechanisms:
  - Move timing validation
  - Bot pattern detection
  - Heartbeat monitoring
  - State hash verification
- Rate limiting (100 req/min per wallet)
- JWT tokens with refresh
- Encrypted database connections

## 📊 Database Schema

Key entities:
- **Users**: Wallet addresses, ratings, affiliates
- **Games**: Game state, stakes, contracts
- **GameParticipants**: Player scores and results
- **GameMoves**: Complete move history for audit
- **Transactions**: All blockchain operations
- **ImageSets & ImagePairs**: Content management
- **Coupons**: Promotional codes
- **AffiliateLinks**: Referral tracking

## 🚢 Deployment

### Docker Deployment
```bash
docker build -t twinswins-api:latest -f src/TwinsWins.API/Dockerfile .
docker build -t twinswins-client:latest -f src/TwinsWins.BlazorClient/Dockerfile .
docker-compose -f docker-compose.prod.yml up -d
```

### Kubernetes Deployment
```bash
kubectl apply -f infrastructure/k8s/
```

## 📈 Monitoring

### Health Checks
- `/health/live` - Application is alive
- `/health/ready` - Ready to accept traffic
- `/health/startup` - Startup complete

### Metrics (Prometheus)
- Request rate and latency
- Active games count
- Transaction success rate
- Cache hit ratio
- Database query performance

### Dashboards (Grafana)
- Import dashboards from `infrastructure/grafana/`

## 🧪 Testing

```bash
# Unit tests
dotnet test tests/TwinsWins.Core.Tests

# Integration tests
dotnet test tests/TwinsWins.Integration.Tests

# Load tests
cd tests/LoadTests
k6 run game-load-test.js
```

## 📝 API Documentation

Swagger UI available at: http://localhost:5000/swagger

### Key Endpoints

**Game Management:**
- `POST /api/games/free` - Create free game
- `POST /api/games/paid` - Create paid game
- `POST /api/games/{id}/join` - Join game
- `POST /api/games/{id}/moves` - Submit move
- `POST /api/games/{id}/complete` - Complete game
- `GET /api/games/lobbies` - Get active lobbies

**User:**
- `POST /api/auth/connect` - Connect TON wallet
- `GET /api/users/me` - Get current user
- `GET /api/users/stats` - Get statistics

**Admin:**
- `POST /api/admin/coupons` - Create coupon
- `GET /api/admin/games` - List all games
- `POST /api/admin/imagesets` - Upload image set

## 🎯 Roadmap

### Phase 1: MVP (Current)
- ✅ Core game mechanics
- ✅ Free and paid modes
- ✅ TON integration
- ✅ Basic anti-cheat
- 🔄 Blazor client implementation

### Phase 2: Beta
- Matchmaking system
- Skill-based rating (ELO)
- Leaderboards
- Admin dashboard
- Telegram Mini App integration

### Phase 3: Launch
- Smart contract audit
- Legal compliance
- Affiliate system
- Coupon system
- Mobile PWA optimization

### Phase 4: Post-Launch
- Tournament mode
- Achievement system
- Power-ups
- Seasonal events
- Mobile native apps

## 🤝 Contributing

1. Fork the repository
2. Create feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit changes (`git commit -m 'Add AmazingFeature'`)
4. Push to branch (`git push origin feature/AmazingFeature`)
5. Open Pull Request

## 📜 License

This project is licensed under the MIT License - see LICENSE file for details.

## 👥 Team

- Project Manager: [Name]
- Lead Developer: [Name]
- Blockchain Developer: [Name]
- UI/UX Designer: [Name]

## 📞 Support

- Documentation: https://docs.twinswins.com
- Discord: https://discord.gg/twinswins
- Email: support@twinswins.com

## ⚠️ Legal Notice

This application involves real money transactions. Users must:
- Be 18+ years old
- Comply with local gambling regulations
- Play responsibly

Geo-blocking is implemented for restricted jurisdictions.
