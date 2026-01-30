# ADR-001 - Adoption de l’architecture hexagonale

## Statut
Accepté

## Contexte
Le projet BookHub est composé de plusieurs microservices (.NET) devant rester
maintenables, testables et indépendants des technologies externes
(base de données, messaging, API externes).

Une architecture classique en couches risquait de créer un fort couplage
entre la logique métier et l’infrastructure.

## Décision
Nous adoptons une architecture hexagonale (Ports & Adapters) pour chaque service.

La logique métier est isolée dans le cœur applicatif.
Les accès aux bases de données, aux brokers de messages et aux autres services
sont réalisés via des ports (interfaces) et des adaptateurs.

## Alternatives considérées
- Architecture en couches classiques
- Clean Architecture

## Conséquences
### Avantages
- Meilleure testabilité
- Faible couplage aux technologies
- Facilité d’évolution et de maintenance

### Inconvénients
- Complexité initiale plus élevée
- Plus de fichiers et d’interfaces
