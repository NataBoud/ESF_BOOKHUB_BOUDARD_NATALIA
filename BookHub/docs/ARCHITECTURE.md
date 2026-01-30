# BookHub - Architecture du projet

## 1. Vue d'ensemble

BookHub est une plateforme de gestion de bibliothèque numérique construite avec une **architecture microservices** et un frontend **Blazor WebAssembly**.  

Chaque microservice est autonome et responsable d'un domaine métier spécifique. L'application utilise **PostgreSQL** pour la persistance des données et **RabbitMQ** pour la communication événementielle entre services.

---

## 2. Architecture globale

        ┌─────────────────────────────┐
        │     Blazor WebAssembly      │
        │     (Frontend Client)       │
        └───────────────┬─────────────┘
                        │
                        ▼
        ┌───────────────────────────────┐
        │      API Gateway (Ocelot)     │
        │        Port: 5000             │
        └───────┬─────────┬─────────┬───┘
                │         │         │
                ▼         ▼         ▼
       ┌──────────┐ ┌──────────┐ ┌──────────┐
       │ Catalog  │ │ User     │ │ Loan     │
       │ Service  │ │ Service  │ │ Service  │
       │ :5001    │ │ :5002    │ │ :5003    │
       └──────────┘ └──────────┘ └──────────┘
             │           │          │
             └───────────┴──────────┘
                         │
                         ▼
             ┌───────────────────────┐
             │ PostgreSQL / RabbitMQ │
             └───────────────────────┘


   
### 2.1 Frontend
- **Blazor WebAssembly** : Application SPA exécutée dans le navigateur.
- Communique avec le backend via l’API Gateway.
- Services utilisés : `IBookService`, `IAuthService`.

### 2.2 API Gateway
- **Ocelot** : Route toutes les requêtes du frontend vers les microservices.
- Point d'entrée unique : `http://localhost:5000`.
- Facilite la gestion des routes et la sécurité (JWT).

### 2.3 Microservices
- **Catalog Service** (`5001`) : Gestion du catalogue de livres.
- **User Service** (`5002`) : Gestion des utilisateurs et authentification.
- **Loan Service** (`5003`) : Gestion des emprunts et retours.
- **Notification Service** (à créer) : Envoi d'événements/notifications aux utilisateurs.

Chaque service est isolé et dispose de sa propre base de données dans PostgreSQL.

### 2.4 Base de données et messagerie
- **PostgreSQL** : Persistance des données relationnelles.
- **RabbitMQ** : Broker de messages pour communication asynchrone entre services, utile pour notifications et événements métier.

---

## 3. Architecture hexagonale

Chaque microservice suit le principe **Hexagonal / Ports & Adapters** :

    ┌───────────────┐
    │  External     │
    │  Interfaces   │
    │ (Controllers) │
    └──────┬────────┘
           │
    ┌──────▼────────┐
    │  Application  │
    │   Services    │
    └──────┬────────┘
           │
    ┌──────▼────────┐
    │ Domain Models │
    │ & Business    │
    │  Logic        │
    └──────┬────────┘
           │
    ┌──────▼────────┐
    │ Persistence & │
    │ Infrastructure│
    └───────────────┘

- **Domain Models** : Entités métier (`Book`, `User`, `Loan`, etc.)
- **Application Services** : Logique métier orchestrant les opérations.
- **Adapters / Ports** : Interfaces pour persistance (Entity Framework Core) ou communication (RabbitMQ, HTTP API).
- **Controllers / API** : Point d’entrée des requêtes HTTP.

---

## 4. Communication inter-services

1. **Synchronous** : via HTTP/REST à travers l’API Gateway.
2. **Asynchronous** : via RabbitMQ pour les événements métier (ex. `LoanCreatedEvent`, `UserRegisteredEvent`).

---

## 5. Avantages de cette architecture
- Scalabilité indépendante des microservices.
- Développement et tests isolés.
- Communication événementielle pour découplage et réactivité.
- Facilité de maintenance et d’extension (ex. ajout du Notification Service).

---

## 6. Notes supplémentaires
- Tous les microservices sont conteneurisés via **Docker**.
- Le projet supporte le développement en **mode local** avec PostgreSQL et RabbitMQ, ou en **production** via Docker Compose.

