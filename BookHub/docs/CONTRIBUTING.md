# Guide de contribution - BookHub

Merci de contribuer à BookHub ! Vos contributions, qu'il s'agisse de correction de bugs, de nouvelles fonctionnalités ou d'amélioration de la documentation, sont les bienvenues.

## 1. Règles générales

- Lisez attentivement le **README.md** et le **SUJET_ECF.md** avant de commencer.
- Respectez le style de code existant (C# pour les services, Blazor/HTML/CSS pour le frontend).
- Fournissez des commits clairs et atomiques, avec un message descriptif.
- Testez vos modifications avant de proposer un merge.

## 2. Workflow Git recommandé

Nous recommandons d’utiliser le workflow **GitFlow** :

1. **Clonez le repository**

```bash
   git clone <repository-url>
   cd BookHub
```
2. **Créez une branche pour votre fonctionnalité ou correction**

```bash
   git checkout -b feature/nom-de-la-fonctionnalité
```
ou 
```bash
   git checkout -b fix/description-du-bug
```
3. **Faites vos changements**

- Respectez l'architecture microservices.

- Mettez à jour ou ajoutez les tests unitaires si nécessaire.

- Vérifiez que tous les services démarrent correctement via Docker.

4. **Committez vos changements**

```bash
    git add .
    git commit -m "Ajout de la fonctionnalité X dans le LoanService"
```
5. **Poussez votre branche**

```bash
    git push origin feature/nom-de-la-fonctionnalité
```
6. **Créez une Pull Request**
- Décrivez les modifications apportées.

- Mentionnez tout point particulier à vérifier (tests, Docker, endpoints API, etc.).

## 3. Services et microservices

- Chaque service (CatalogService, UserService, LoanService, NotificationService) est indépendant.

- Lors de la contribution, assurez-vous que les modifications n’impactent pas les autres services.

- Pour les nouveaux services (ex. NotificationService), ajoutez la configuration Docker correspondante.

## 4. Tests

- Exécutez les tests unitaires avant de pousser vos modifications :

```bash
    dotnet test
```
- Vérifiez que tous les endpoints REST fonctionnent via Postman ou Swagger.

## 5. Documentation

- Mettez à jour les fichiers dans docs/ si vous ajoutez ou modifiez des fonctionnalités.

- Incluez les changements dans API_REFERENCE.md si vous modifiez des endpoints.

## 6. Bonnes pratiques

- Respectez les conventions de nommage C# (PascalCase pour classes et méthodes, camelCase pour variables locales).

- Évitez de commiter des secrets (mots de passe, clés API).

- Utilisez le versioning semantic pour les services si vous créez des releases.

### Merci encore pour votre contribution ! 🎉