# ECF - Architecture Microservices .NET et Blazor

## Titre : Plateforme de Gestion de Bibliotheque Numerique "BookHub"

---

## Contexte Professionnel

Vous integrez l'equipe technique de la startup **BookHub** qui developpe une plateforme de gestion de bibliotheque numerique. L'entreprise a demarre un projet base sur une architecture microservices avec .NET 8 et une interface utilisateur Blazor WebAssembly.

Le developpeur principal a quitte l'entreprise en laissant un projet partiellement termine. Votre mission est de :
1. **Analyser** l'architecture existante
2. **Completer** les fonctionnalites manquantes
3. **Documenter** l'ensemble du projet
4. **Deployer** l'application via Docker Compose

---

## Objectifs Pedagogiques

Cette epreuve evalue les competences suivantes du referentiel CDA :

| Competence | Description |
|------------|-------------|
| **C1** | Concevoir et developper des composants d'acces aux donnees |
| **C2** | Concevoir et developper la partie back-end d'une application |
| **C3** | Concevoir et developper la partie front-end d'une application |
| **C5** | Collaborer a la gestion d'un projet informatique |
| **C6** | Concevoir une application repondant a un cahier des charges |

---

## Architecture du Projet Fourni

```
BookHub/
├── src/
│   ├── Services/
│   │   ├── BookHub.CatalogService/        # Service de gestion du catalogue
│   │   │   ├── Api/                       # Controllers
│   │   │   ├── Application/               # Services applicatifs
│   │   │   ├── Domain/                    # Entites et Ports
│   │   │   └── Infrastructure/            # Implementations (Persistence)
│   │   ├── BookHub.UserService/           # Service de gestion des utilisateurs
│   │   │   ├── Api/                       # Controllers
│   │   │   ├── Application/               # Services applicatifs
│   │   │   ├── Domain/                    # Entites et Ports
│   │   │   └── Infrastructure/            # Implementations (Persistence, Security)
│   │   └── BookHub.LoanService/           # Service de gestion des emprunts
│   │       ├── Api/                       # Controllers
│   │       ├── Application/               # Services applicatifs
│   │       ├── Domain/                    # Entites et Ports
│   │       └── Infrastructure/            # Implementations (Persistence, HttpClients)
│   ├── Web/
│   │   └── BookHub.BlazorClient/          # Application Blazor WASM
│   └── Shared/
│       └── BookHub.Shared/                # DTOs et contrats partages
├── tests/
│   └── ...                                # Tests unitaires
├── docs/
│   └── ...                                # Documentation
├── docker-compose.yml                     # Orchestration
└── README.md
```

### Architecture Hexagonale

Chaque microservice est structure selon l'**Architecture Hexagonale** (Ports & Adapters) :

```
Service/
├── Domain/              # Coeur metier (aucune dependance externe)
│   ├── Entities/        # Entites du domaine
│   └── Ports/           # Interfaces (contrats)
├── Application/         # Cas d'utilisation
│   └── Services/        # Services applicatifs
├── Infrastructure/      # Implementations des ports
│   ├── Persistence/     # DbContext, Repositories
│   ├── HttpClients/     # Clients HTTP pour communication inter-services
│   └── Security/        # Implementations securite (JWT, hashing)
└── Api/                 # Point d'entree
    └── Controllers/     # Controllers REST
```

---

## Travail Demande

### Partie 1 : Analyse et Documentation

#### 1.1 Analyse de l'Architecture Existante
- Produire un **diagramme d'architecture** (C4 Model - niveau Context et Container)
- Identifier les **patterns** utilises (Repository, Hexagonal Architecture, etc.)
- Lister les **dependances** entre les services
- Analyser les **choix techniques** et leur pertinence

#### 1.2 Documentation Technique
Creer les documents suivants dans le dossier `docs/` :
- `ARCHITECTURE.md` : Description de l'architecture globale et hexagonale
- `API_REFERENCE.md` : Documentation des endpoints REST
- `DEPLOYMENT.md` : Guide de deploiement
- `CONTRIBUTING.md` : Guide de contribution au projet

#### Livrables attendus :
- Diagrammes d'architecture (PNG/SVG + source)
- Documentation technique complete
- Fichier `docs/ADR/` avec les Architecture Decision Records

---

### Partie 2 : Developpement Back-End

#### 2.1 Service d'Emprunts (LoanService)

Le service `LoanService` doit gerer les emprunts de livres. La structure hexagonale est en place mais certaines methodes ne sont pas implementees.

**Endpoints :**
```
POST   /api/loans              # Creer un emprunt
GET    /api/loans/{id}         # Recuperer un emprunt
GET    /api/loans/user/{userId}# Emprunts d'un utilisateur
PUT    /api/loans/{id}/return  # Retourner un livre
GET    /api/loans/overdue      # Liste des emprunts en retard
```

**Regles metier :**
- Un utilisateur ne peut pas emprunter plus de 5 livres simultanement
- La duree maximale d'un emprunt est de 21 jours
- Un livre deja emprunte ne peut pas etre emprunte a nouveau
- Les emprunts en retard generent une penalite de 0.50 EUR/jour

**Architecture a respecter :**
- Les regles metier doivent etre dans le **Domain** (entite `Loan`)
- La logique applicative dans **Application/Services**
- La communication avec les autres services via **Infrastructure/HttpClients**

#### 2.2 Communication Inter-Services

Le LoanService communique avec les autres services via HTTP :
- **CatalogService** : Verifier la disponibilite, decrementer/incrementer le stock
- **UserService** : Verifier l'existence de l'utilisateur

Les clients HTTP sont deja configures dans `Infrastructure/HttpClients/`.

#### 2.3 Tests

Implementer :
- Tests unitaires pour les entites du domaine
- Tests unitaires pour les services applicatifs
- Tests d'integration pour les endpoints

#### Livrables attendus :
- Implementation complete du LoanService
- Tests unitaires (couverture minimale : 60%)
- Tests d'integration pour les endpoints

---

### Partie 3 : Developpement Front-End Blazor

#### 3.1 Interface Utilisateur

L'application Blazor WASM doit contenir :
- Page d'accueil
- Liste des livres (catalogue)
- Page de detail d'un livre
- Gestion des emprunts utilisateur
- Tableau de bord administrateur

**Pages a developper :**

1. **Page Detail Livre** (`/books/{id}`)
   - Affichage des informations du livre
   - Bouton "Emprunter" (si disponible)
   - Historique des emprunts (admin uniquement)

2. **Page Mes Emprunts** (`/my-loans`)
   - Liste des emprunts en cours
   - Historique des emprunts passes
   - Bouton "Retourner" pour chaque emprunt actif

3. **Tableau de Bord Admin** (`/admin/dashboard`)
   - Statistiques : nombre d'emprunts, livres les plus empruntes
   - Liste des emprunts en retard
   - Graphique d'activite (optionnel)

#### 3.2 Composants Reutilisables

Creer les composants Blazor suivants :
- `BookCard.razor` : Carte d'affichage d'un livre
- `LoanStatus.razor` : Badge de statut d'emprunt
- `Pagination.razor` : Composant de pagination generique
- `ConfirmDialog.razor` : Modal de confirmation

#### 3.3 Gestion d'Etat

Implementer un **State Container** pour gerer :
- L'etat de l'utilisateur connecte
- Le panier d'emprunts
- Les notifications en temps reel

#### Livrables attendus :
- Pages Blazor fonctionnelles
- Composants reutilisables
- State management implemente
- Responsive design (Bootstrap 5)

---

### Partie 4 : Conteneurisation et Deploiement

#### 4.1 Dockerfiles

Verifier et optimiser les Dockerfiles pour chaque service :
- Multi-stage build
- Optimisation de la taille des images
- Non-root user pour la securite

#### 4.2 Docker Compose

Le fichier `docker-compose.yml` contient :

```yaml
services:
  # Microservices
  catalog-service:
    ...
  user-service:
    ...
  loan-service:
    ...

  # Frontend
  blazor-client:
    ...

  # Infrastructure
  postgres:
    ...

  # Monitoring (bonus)
  seq:           # Centralisation des logs
    ...
```

#### 4.3 Configuration

- Variables d'environnement pour chaque service
- Health checks
- Gestion des secrets (via fichier `.env`)
- Volumes pour la persistance des donnees

#### Livrables attendus :
- Dockerfiles optimises
- `docker-compose.yml` fonctionnel
- `docker-compose.override.yml` pour le developpement
- Script de demarrage (`start.sh` / `start.ps1`)
- Documentation de deploiement

---

## Criteres d'Evaluation

### Grille d'Evaluation (100 points)

| Critere | Points | Description |
|---------|--------|-------------|
| **Analyse & Documentation** | 20 | Qualite des diagrammes, clarte de la documentation |
| **Back-End** | 30 | Fonctionnalites, architecture hexagonale, qualite du code |
| **Front-End Blazor** | 25 | UI/UX, composants, gestion d'etat |
| **Conteneurisation** | 15 | Docker, orchestration, configuration |
| **Tests** | 10 | Couverture, pertinence des tests |

### Bonus (10 points supplementaires)
- Implementation de **OpenTelemetry** pour le tracing
- Ajout d'un **cache Redis** pour le catalogue
- **Swagger/OpenAPI** documente avec exemples
- Implementation d'un **NotificationService** avec envoi d'emails simules

---

## Environnement Technique

### Prerequis
- .NET 8 SDK
- Docker Desktop
- Visual Studio 2022 / VS Code / Rider
- Git

### Stack Technique
| Composant | Technologie |
|-----------|-------------|
| Backend | .NET 8, ASP.NET Core Web API |
| Frontend | Blazor WebAssembly |
| Architecture | Hexagonale (Ports & Adapters) |
| Base de donnees | PostgreSQL |
| ORM | Entity Framework Core 8 |
| Authentification | JWT |
| Conteneurisation | Docker, Docker Compose |
| Tests | xUnit, Moq, FluentAssertions |

---

## Modalites de Rendu

1. **Repository Git** avec historique de commits pertinent
2. **Archive ZIP** contenant :
   - Code source complet
   - Documentation (`docs/`)
   - Fichiers Docker
   - README avec instructions de demarrage

---

## Consignes Importantes

1. **Commits reguliers** avec messages explicites (conventional commits)
2. **Code propre** : Respect des conventions .NET, pas de code commente
3. **Pas de copier-coller** depuis des sources externes sans comprehension
4. **Gestion des erreurs** : Logging approprie, messages utilisateur clairs
5. **Securite** : Validation des entrees, pas de secrets dans le code
6. **Architecture Hexagonale** : Respecter la separation des couches


---

**Bon courage !**
