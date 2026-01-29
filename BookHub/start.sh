#!/bin/bash

# Script de démarrage pour BookHub
# Usage: ./start.sh [dev|prod]

set -e

MODE=${1:-dev}

echo "=========================================="
echo "  BookHub - Démarrage en mode: $MODE"
echo "=========================================="

if [ "$MODE" = "dev" ]; then
    echo "Démarrage des services d'infrastructure..."
    docker-compose up -d postgres rabbitmq

    echo "Attente du démarrage de PostgreSQL..."
    sleep 5

    echo ""
    echo "Infrastructure démarrée!"
    echo "- PostgreSQL: localhost:5432"
    echo "- RabbitMQ: localhost:5672 (Management: localhost:15672)"
    echo ""
    echo "Pour démarrer les services .NET, exécutez dans des terminaux séparés:"
    echo "  cd src/Services/BookHub.CatalogService && dotnet run"
    echo "  cd src/Services/BookHub.UserService && dotnet run"
    echo "  cd src/Services/BookHub.LoanService && dotnet run"
    echo "  cd src/Gateway/BookHub.ApiGateway && dotnet run"
    echo "  cd src/Web/BookHub.BlazorClient && dotnet run"

elif [ "$MODE" = "prod" ]; then
    echo "Construction et démarrage de tous les services..."
    docker-compose up -d --build

    echo ""
    echo "Tous les services sont démarrés!"
    echo ""
    echo "URLs disponibles:"
    echo "  - Frontend: http://localhost:8080"
    echo "  - API Gateway: http://localhost:5000"
    echo "  - RabbitMQ Management: http://localhost:15672"
    echo ""
    echo "Pour voir les logs: docker-compose logs -f"
    echo "Pour arrêter: docker-compose down"

else
    echo "Usage: ./start.sh [dev|prod]"
    echo "  dev  - Démarre uniquement PostgreSQL et RabbitMQ"
    echo "  prod - Démarre tous les services via Docker Compose"
    exit 1
fi
