# Script de démarrage pour BookHub (Windows PowerShell)
# Usage: .\start.ps1 [dev|prod]

param(
    [string]$Mode = "dev"
)

Write-Host "==========================================" -ForegroundColor Cyan
Write-Host "  BookHub - Démarrage en mode: $Mode" -ForegroundColor Cyan
Write-Host "==========================================" -ForegroundColor Cyan

if ($Mode -eq "dev") {
    Write-Host "Démarrage des services d'infrastructure..." -ForegroundColor Yellow
    docker-compose up -d postgres rabbitmq

    Write-Host "Attente du démarrage de PostgreSQL..." -ForegroundColor Yellow
    Start-Sleep -Seconds 5

    Write-Host ""
    Write-Host "Infrastructure démarrée!" -ForegroundColor Green
    Write-Host "- PostgreSQL: localhost:5432"
    Write-Host "- RabbitMQ: localhost:5672 (Management: localhost:15672)"
    Write-Host ""
    Write-Host "Pour démarrer les services .NET, exécutez dans des terminaux séparés:" -ForegroundColor Yellow
    Write-Host "  cd src\Services\BookHub.CatalogService; dotnet run"
    Write-Host "  cd src\Services\BookHub.UserService; dotnet run"
    Write-Host "  cd src\Services\BookHub.LoanService; dotnet run"
    Write-Host "  cd src\Gateway\BookHub.ApiGateway; dotnet run"
    Write-Host "  cd src\Web\BookHub.BlazorClient; dotnet run"
}
elseif ($Mode -eq "prod") {
    Write-Host "Construction et démarrage de tous les services..." -ForegroundColor Yellow
    docker-compose up -d --build

    Write-Host ""
    Write-Host "Tous les services sont démarrés!" -ForegroundColor Green
    Write-Host ""
    Write-Host "URLs disponibles:" -ForegroundColor Cyan
    Write-Host "  - Frontend: http://localhost:8080"
    Write-Host "  - API Gateway: http://localhost:5000"
    Write-Host "  - RabbitMQ Management: http://localhost:15672"
    Write-Host ""
    Write-Host "Pour voir les logs: docker-compose logs -f"
    Write-Host "Pour arrêter: docker-compose down"
}
else {
    Write-Host "Usage: .\start.ps1 [dev|prod]" -ForegroundColor Red
    Write-Host "  dev  - Démarre uniquement PostgreSQL et RabbitMQ"
    Write-Host "  prod - Démarre tous les services via Docker Compose"
    exit 1
}
