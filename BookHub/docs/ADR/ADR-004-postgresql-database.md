# ADR-004 - Choix de PostgreSQL

## Statut
Accepté

## Contexte
Les services nécessitent une base relationnelle fiable, open-source et compatible
avec Docker.

## Décision
PostgreSQL est utilisé comme base de données pour chaque microservice,
avec une base dédiée par service.

## Alternatives considérées
- MySQL
- SQL Server

## Conséquences
### Avantages
- Open-source
- Performant
- Très bien supporté par Docker et .NET

### Inconvénients
- Administration plus avancée qu’une base embarquée
