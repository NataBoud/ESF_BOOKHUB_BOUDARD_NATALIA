# Guide de Déploiement - BookHub

Ce guide explique comment déployer et exécuter l'application **BookHub**, que ce soit en développement local ou en production via Docker.

---

## 1. Prérequis

Avant de commencer, assurez-vous d’avoir installé :

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) ou VS Code
- Accès à Internet pour télécharger les images Docker

---

## 2. Structure du projet

```
BookHub/
├── src/
│   ├── Services/
│   │   ├── BookHub.CatalogService/    # Microservice catalogue
│   │   ├── BookHub.UserService/       # Microservice utilisateurs
│   │   ├── BookHub.LoanService/       # Microservice emprunts (partiel)
│   │   └── BookHub.NotificationService/ # À créer
│   ├── Gateway/
│   │   └── BookHub.ApiGateway/        # API Gateway Ocelot À créer égalemet !
│   ├── Web/
│   │   └── BookHub.BlazorClient/      # Frontend Blazor WASM
│   └── Shared/
│       └── BookHub.Shared/            # DTOs et contrats
├── tests/                             # Tests unitaires
├── docs/                              # Documentation
├── scripts/                           # Scripts utilitaires
├── docker-compose.yml
└── BookHub.sln
```

---

## 3. Déploiement en développement (Dev)

1. **Ouvrir PowerShell** dans le dossier racine du projet `BookHub`.
2. **Démarrer PostgreSQL et RabbitMQ** via le script PowerShell ou Bash :

```powershell
.\start.ps1 dev
# ou en Bash
./start.sh dev
```
- Ce script démarre uniquement les services d’infrastructure nécessaires (PostgreSQL et RabbitMQ).
- Vérifiez que les conteneurs sont bien Up et PostgreSQL est healthy.

3. **Démarrer les microservices .NET** dans des terminaux séparés :

```bash

cd src/Services/BookHub.CatalogService
dotnet run

cd src/Services/BookHub.UserService
dotnet run

cd src/Services/BookHub.LoanService
dotnet run

cd src/Gateway/BookHub.ApiGateway
dotnet run

cd src/Web/BookHub.BlazorClient
dotnet run
```
4. **Accéder à l’application :**

Frontend : http://localhost:8080

API Gateway : http://localhost:5000

RabbitMQ Management : http://localhost:15672
 (guest/guest)

## 4. Déploiement en production (Prod) via Docker Compose

1. Construire et démarrer tous les services :

```bash
.\start.ps1 prod
# ou en Bash
./start.sh prod
```
Ce mode va builder les images Docker et démarrer tous les services, y compris le frontend Blazor et les microservices.

2. Vérifier le statut des conteneurs :

```bash
docker-compose ps
```
3. Arrêter les services :

```bash
docker-compose down
```
3. Voir les logs :

```bash
docker-compose logs -f
```
## 5. Notes importantes

### Ports utilisés par défaut

| Service            | Port   | Description                                 |
|-------------------|--------|---------------------------------------------|
| PostgreSQL         | 5432   | Base de données principale                  |
| RabbitMQ           | 5672   | Broker de messages                          |
| RabbitMQ Management| 15672  | Interface web de gestion de RabbitMQ        |
| Catalog Service    | 5001   | Microservice de gestion des livres          |
| User Service       | 5002   | Microservice de gestion des utilisateurs   |
| Loan Service       | 5003   | Microservice de gestion des emprunts        |
| API Gateway        | 5000   | Point d’entrée unique (Ocelot)             |
| Blazor Frontend    | 8080   | Interface utilisateur (WebAssembly)        |

### Notes importantes

- Les variables d’environnement pour les connexions à PostgreSQL et RabbitMQ sont définies dans `docker-compose.yml`.
- Pour le développement, il est possible d’utiliser `docker-compose.override.yml` pour surcharger certaines variables ou ports.
- La configuration avancée (health checks, gestion des secrets via `.env`, volumes pour persistance des données) sera documentée après validation des services backend.

> ⚠️ Avant de déployer tous les services, assurez-vous que PostgreSQL et RabbitMQ démarrent correctement et que leurs conteneurs sont en état `healthy` ou `Up`.


## 6. Déploiement manuel sans script

```bash
docker-compose up -d postgres rabbitmq
docker build -t bookhub-catalog ./src/Services/BookHub.CatalogService
docker run -p 5001:8080 --network bookhub-network bookhub-catalog
# répéter pour user-service, loan-service et blazor-client

```