## Étapes effectuées

1. **Correction du `docker-compose.yml`**  
   - Ajout du service `rabbitmq` avec ports `5672` et `15672`  
   - Vérification des dépendances et network pour tous les services  

2. **Lancement manuel de PostgreSQL et RabbitMQ**  
   - Utilisation de Docker Desktop pour démarrer les conteneurs  
   - Vérification que PostgreSQL est `healthy` et RabbitMQ est en `Up` 

## Diagrammes d’architecture

Voici le diagramme global de l’architecture du projet BookHub :

🔗 [Voir le diagramme d’architecture](https://viewer.diagrams.net/?tags=%7B%7D&lightbox=1&highlight=0000ff&edit=_blank&layers=1&nav=1&dark=auto#G1m-vruKjKeDpjlCb3RytkfmjUMNA5SRLU)

## Architecture Decision Records (ADR)

Les décisions d’architecture du projet sont documentées dans le dossier `docs/ADR/`.
Chaque ADR décrit le contexte, la décision prise, les alternatives envisagées
et les conséquences associées.


## dotnet ef migrations add InitialCreate
