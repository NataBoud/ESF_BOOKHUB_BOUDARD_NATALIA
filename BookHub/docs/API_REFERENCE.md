# API Reference - BookHub

Ce document décrit les endpoints REST exposés par les différents microservices de BookHub.

---

## 1. Catalog Service

Gestion des livres et du catalogue.

| Méthode | Endpoint                 | Description                    | Paramètres/Body                                         | Réponse |
|---------|--------------------------|--------------------------------|--------------------------------------------------------|---------|
| GET     | `/api/books`             | Liste tous les livres          | -                                                      | `200 OK` : liste de `BookDto` |
| GET     | `/api/books/{id}`        | Détails d’un livre             | `id` : Guid du livre                                   | `200 OK` : `BookDto` |
| GET     | `/api/books/search`      | Recherche de livres            | Query : `term` (string)                                | `200 OK` : liste de `BookDto` |
| POST    | `/api/books`             | Créer un nouveau livre         | Body : `CreateBookDto`                                  | `201 Created` : `BookDto` |
| PUT     | `/api/books/{id}`        | Mettre à jour un livre         | `id` : Guid, Body : `UpdateBookDto`                    | `200 OK` : `BookDto` |
| DELETE  | `/api/books/{id}`        | Supprimer un livre             | `id` : Guid                                            | `204 No Content` |

---

## 2. User Service

Gestion des utilisateurs et authentification.

| Méthode | Endpoint                      | Description                  | Paramètres/Body                                         | Réponse |
|---------|-------------------------------|------------------------------|--------------------------------------------------------|---------|
| POST    | `/api/users/register`          | Inscription utilisateur      | Body : `CreateUserDto`                                 | `201 Created` : `UserDto` |
| POST    | `/api/users/login`             | Connexion utilisateur        | Body : `LoginDto`                                      | `200 OK` : `LoginResponseDto` |
| GET     | `/api/users/{id}`              | Profil utilisateur           | `id` : Guid                                            | `200 OK` : `UserDto` |
| GET     | `/api/users`                   | Liste des utilisateurs       | -                                                      | `200 OK` : liste de `UserDto` |

---

## 3. Loan Service

Gestion des emprunts de livres.

| Méthode | Endpoint                           | Description                     | Paramètres/Body                                         | Réponse |
|---------|-----------------------------------|---------------------------------|--------------------------------------------------------|---------|
| POST    | `/api/loans`                       | Créer un nouvel emprunt          | Body : `CreateLoanDto`                                  | `201 Created` : `LoanDto` |
| GET     | `/api/loans/{id}`                  | Détails d’un emprunt             | `id` : Guid                                            | `200 OK` : `LoanDto` |
| GET     | `/api/loans/user/{userId}`         | Emprunts d’un utilisateur        | `userId` : Guid                                        | `200 OK` : liste de `LoanDto` |
| PUT     | `/api/loans/{id}/return`           | Retourner un livre               | `id` : Guid                                            | `200 OK` : `LoanDto` |
| GET     | `/api/loans/overdue`               | Liste des emprunts en retard     | -                                                      | `200 OK` : liste de `LoanDto` |

---

## 4. Notification Service (À créer)

Prévu pour l’envoi de notifications. Les endpoints seront définis ultérieurement.

---

### Notes générales

- Tous les endpoints suivent les conventions REST et renvoient des **codes HTTP standard** (`200`, `201`, `204`, `400`, `404`, etc.).
- Les DTOs utilisés pour les requêtes et réponses sont définis dans `BookHub.Shared/DTOs`.
- L’authentification pour les endpoints sécurisés se fait via **JWT Bearer Tokens**.

---

