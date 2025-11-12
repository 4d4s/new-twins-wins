#!/bin/bash

echo "========================================="
echo "TwinsWins Project Setup"
echo "========================================="
echo ""

# Check prerequisites
echo "Checking prerequisites..."

if ! command -v dotnet &> /dev/null; then
    echo "❌ .NET 8.0 SDK is not installed"
    echo "   Download from: https://dotnet.microsoft.com/download/dotnet/8.0"
    exit 1
fi
echo "✅ .NET SDK found: $(dotnet --version)"

if ! command -v docker &> /dev/null; then
    echo "❌ Docker is not installed"
    echo "   Download from: https://www.docker.com/products/docker-desktop"
    exit 1
fi
echo "✅ Docker found"

if ! command -v docker-compose &> /dev/null; then
    echo "❌ Docker Compose is not installed"
    exit 1
fi
echo "✅ Docker Compose found"

echo ""
echo "========================================="
echo "Step 1: Starting Infrastructure"
echo "========================================="
cd infrastructure
docker-compose up -d

echo "Waiting for services to be ready..."
sleep 10

echo ""
echo "✅ Infrastructure started:"
echo "   - PostgreSQL: localhost:5432"
echo "   - Redis: localhost:6379"
echo "   - MinIO: localhost:9000"
echo "   - Grafana: localhost:3000 (admin/admin)"
echo "   - Prometheus: localhost:9090"

cd ..

echo ""
echo "========================================="
echo "Step 2: Restoring NuGet Packages"
echo "========================================="
dotnet restore

echo ""
echo "========================================="
echo "Step 3: Building Solution"
echo "========================================="
dotnet build

echo ""
echo "========================================="
echo "Step 4: Database Setup"
echo "========================================="
echo "Installing EF Core tools..."
dotnet tool install --global dotnet-ef

echo "Creating initial migration..."
cd src/TwinsWins.Infrastructure
dotnet ef migrations add InitialCreate --startup-project ../TwinsWins.API

echo "Applying database migrations..."
dotnet ef database update --startup-project ../TwinsWins.API

cd ../..

echo ""
echo "========================================="
echo "Step 5: Seeding Sample Data"
echo "========================================="
echo "Creating sample image set..."
# TODO: Add seed data script

echo ""
echo "========================================="
echo "✅ Setup Complete!"
echo "========================================="
echo ""
echo "To start the application:"
echo ""
echo "1. API Server:"
echo "   cd src/TwinsWins.API"
echo "   dotnet run"
echo "   API will be available at: https://localhost:5000"
echo "   Swagger UI: https://localhost:5000/swagger"
echo ""
echo "2. Blazor Client (in another terminal):"
echo "   cd src/TwinsWins.BlazorClient"
echo "   dotnet run"
echo "   Client will be available at: https://localhost:5001"
echo ""
echo "Default test credentials:"
echo "   Wallet: Connect any TON wallet"
echo ""
echo "========================================="
echo "Useful Commands:"
echo "========================================="
echo ""
echo "View logs:"
echo "  docker-compose -f infrastructure/docker-compose.yml logs -f"
echo ""
echo "Stop infrastructure:"
echo "  docker-compose -f infrastructure/docker-compose.yml down"
echo ""
echo "Reset database:"
echo "  cd src/TwinsWins.Infrastructure"
echo "  dotnet ef database drop --startup-project ../TwinsWins.API"
echo "  dotnet ef database update --startup-project ../TwinsWins.API"
echo ""
echo "Run tests:"
echo "  dotnet test"
echo ""
echo "========================================="
