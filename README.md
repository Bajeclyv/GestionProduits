# GestionProduits

Lien vers le dépôt GitHub : [Bajeclyv/GestionProduits](https://github.com/Bajeclyv/GestionProduits)

Application .NET 8 de gestion de produits, marques et types de produits, avec une API ASP.NET Core, une interface Blazor Server et PostgreSQL.

## Projets

- **GestionProduits.API** : API REST, accès aux données via Entity Framework Core et PostgreSQL. Swagger est disponible en environnement Development.
- **GestionProduits.Application** : interface Blazor Server interactive. Elle consomme l’API par HTTP et n’accède pas directement à la base de données, ce qui sépare la présentation de la persistance.
- **GestionProduits.Tests** : tests MSTest. Moq sert à isoler les contrôleurs dans les tests unitaires ; les tests d’intégration vérifient le fonctionnement avec PostgreSQL.

## Choix d’architecture

- **DTO et AutoMapper** : les objets échangés par l’API sont distincts des entités EF Core.
- **CRUD générique** : `GenericDataManager<TEntity, TDto>` et `GenericCrudController<TDto>` factorisent les opérations communes.
- **Contrat `IEntityDto`** : fournit un identifiant commun aux DTO utilisés par le CRUD générique, indépendamment du nom de clé primaire dans chaque entité.
- **Clés générées par PostgreSQL** : les clés sont générées à l’insertion ; le gestionnaire reporte l’identifiant créé dans le DTO après `SaveChangesAsync`.
- **Composants Blazor partagés** : les pages Marques et Types de produits réutilisent le même composant de consultation et de gestion afin d’éviter de dupliquer l’interface.

## Configuration et démarrage

1. Créer la base de données PostgreSQL à l'aide du Dockerfile.
2. Adapter `GestionProduits.API/appsettings.json` (`ConnectionStrings:GestionProduitsContext`) à votre environnement.
3. Appliquer les migrations :

   ```bash
   dotnet ef database update --project GestionProduits.API/GestionProduits.API.csproj
   ```

4. Démarrer l’API et l’application Blazor
   Le profil HTTPS de l’API écoute sur `https://localhost:7112`. Cette adresse est configurée dans `GestionProduits.Application/appsettings.json` (`ApiBaseUrl`) ; adaptez-la si nécessaire. Swagger est disponible à `https://localhost:7112/swagger`.

Les routes de gestion sont `api/Produits`, `api/Marques` et `api/TypesProduits`. L’interface Blazor propose les pages `/`, `/marques` et `/types-produits`.

## Tests

Les tests unitaires de contrôleurs n’ont pas besoin de PostgreSQL. **Attention :** `ProduitsControllerIntegrationTests` appelle `EnsureDeletedAsync()` sur la base `produitsdb` configurée dans le test. Cette opération supprime la base et ses données. Avant de lancer ces tests, configurez une base dédiée et remplacez la chaîne de connexion du test ; ne pointez pas les tests vers une base contenant des données à conserver.
