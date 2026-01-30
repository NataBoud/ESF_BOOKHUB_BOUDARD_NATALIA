# ADR-002 - Architecture microservices

## Statut
Accepté

## Contexte
Le projet doit être scalable, modulaire et permettre une séparation claire
des responsabilités fonctionnelles.

## Décision
Le système est découpé en microservices indépendants :
CatalogService, UserService, LoanService et NotificationService.

Chaque service possède sa propre base de données.

## Alternatives considérées
- Monolithe modulaire
- Architecture SOA

## Conséquences
### Avantages
- Scalabilité indépendante
- Déploiement séparé
- Responsabilités claires

### Inconvénients
- Complexité réseau
- Gestion de la communication inter-services
