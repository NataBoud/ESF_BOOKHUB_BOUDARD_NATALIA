# BookHub - Plateforme de Gestion de Bibliothèque Numérique

## Description

BookHub est une application de gestion de bibliothèque numérique construite avec une architecture microservices utilisant .NET 8 et Blazor WebAssembly.

> **Note pour les candidats ECF** : Ce projet est partiellement implémenté. Consultez le fichier `SUJET_ECF.md` dans le dossier parent pour les instructions complètes.

## Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    Blazor WebAssembly                        │
│                    (Frontend Client)                         │
└─────────────────────┬───────────────────────────────────────┘
                      │
                      ▼
┌─────────────────────────────────────────────────────────────┐
│                    API Gateway (Ocelot)                      │
│                    Port: 5000                                │
└────┬─────────────────┬─────────────────┬────────────────────┘
     │                 │                 │
     ▼                 ▼                 ▼
┌──────────┐    ┌──────────┐    ┌──────────┐    ┌──────────────┐
│ Catalog  │    │  User    │    │  Loan    │    │ Notification │
│ Service  │    │ Service  │    │ Service  │    │   Service    │
│ :5001    │    │ :5002    │    │ :5003    │    │   (TODO)     │
└────┬─────┘    └────┬─────┘    └────┬─────┘    └──────┬───────┘
     │               │               │                 │
     └───────────────┴───────────────┴─────────────────┘
                           │
                           ▼
              ┌────────────────────────┐
              │   PostgreSQL / RabbitMQ │
              └────────────────────────┘
```

## Services

| Service | Port | Description | Statut |
|---------|------|-------------|--------|
| API Gateway | 5000 | Point d'entrée unique (Ocelot) | ✅ Complet |
| Catalog Service | 5001 | Gestion du catalogue de livres | ✅ Complet |
| User Service | 5002 | Gestion des utilisateurs et authentification | ✅ Complet |
| Loan Service | 5003 | Gestion des emprunts | ⚠️ Partiel |
| Notification Service | - | Envoi de notifications | ❌ À créer |
| Blazor Client | 8080 | Interface utilisateur | ⚠️ Partiel |

## Prérequis

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) ou [VS Code](https://code.visualstudio.com/)

## Démarrage rapide

### Avec Docker Compose (recommandé)

```bash
# Cloner le repository
git clone <repository-url>
cd BookHub

# Démarrer tous les services
docker-compose up -d

# Vérifier le statut
docker-compose ps
```

L'application sera accessible à :
- Frontend : http://localhost:8080
- API Gateway : http://localhost:5000
- RabbitMQ Management : http://localhost:15672 (guest/guest)

### En développement local

```bash
# Démarrer PostgreSQL et RabbitMQ
docker-compose up -d postgres rabbitmq

# Dans des terminaux séparés, démarrer chaque service :
cd src/Services/BookHub.CatalogService && dotnet run
cd src/Services/BookHub.UserService && dotnet run
cd src/Services/BookHub.LoanService && dotnet run
cd src/Gateway/BookHub.ApiGateway && dotnet run
cd src/Web/BookHub.BlazorClient && dotnet run
```

## Structure du projet

```
BookHub/
├── src/
│   ├── Services/
│   │   ├── BookHub.CatalogService/    # Microservice catalogue
│   │   ├── BookHub.UserService/       # Microservice utilisateurs
│   │   ├── BookHub.LoanService/       # Microservice emprunts (partiel)
│   │   └── BookHub.NotificationService/ # À créer
│   ├── Gateway/
│   │   └── BookHub.ApiGateway/        # API Gateway Ocelot
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

## Comptes de test

| Email | Mot de passe | Rôle |
|-------|--------------|------|
| admin@bookhub.com | Admin123! | Admin |
| librarian@bookhub.com | Librarian123! | Bibliothécaire |
| user@bookhub.com | User123! | Membre |

## API Endpoints

### Catalog Service
- `GET /api/books` - Liste des livres
- `GET /api/books/{id}` - Détails d'un livre
- `GET /api/books/search?term=` - Recherche
- `POST /api/books` - Créer un livre (admin)
- `PUT /api/books/{id}` - Modifier un livre (admin)
- `DELETE /api/books/{id}` - Supprimer un livre (admin)

### User Service
- `POST /api/users/register` - Inscription
- `POST /api/users/login` - Connexion
- `GET /api/users/{id}` - Profil utilisateur
- `GET /api/users` - Liste des utilisateurs (admin)

### Loan Service (à compléter)
- `POST /api/loans` - Créer un emprunt
- `GET /api/loans/{id}` - Détails d'un emprunt
- `GET /api/loans/user/{userId}` - Emprunts d'un utilisateur
- `PUT /api/loans/{id}/return` - Retourner un livre
- `GET /api/loans/overdue` - Emprunts en retard

## Technologies utilisées

- **Backend** : .NET 8, ASP.NET Core Web API
- **Frontend** : Blazor WebAssembly
- **API Gateway** : Ocelot
- **Base de données** : PostgreSQL
- **Message Broker** : RabbitMQ
- **ORM** : Entity Framework Core 8
- **Authentification** : JWT Bearer
- **Conteneurisation** : Docker, Docker Compose

## Contribution

Voir le fichier [CONTRIBUTING.md](docs/CONTRIBUTING.md) pour les guidelines de contribution.

## Licence

Ce projet est sous licence MIT.
