📚 BooksAPI - Projet collectif
Développé par : Adem Sassi & Mohamed Aziz Kabissa
Date : Juin 2025

🔧 Description
BooksAPI est une API REST développée avec ASP.NET Core 7 permettant de gérer une collection de livres numériques et papier. Elle est conçue pour permettre à un client console (BooksClient) de consulter, ajouter et manipuler des ressources bibliographiques via des requêtes HTTP.

🚀 Technologies utilisées
ASP.NET Core 7

Entity Framework Core (SQLite)

Swagger / OpenAPI

C#

SQLite

📂 Structure du projet
bash
Copier
Modifier
BooksAPI/
├── Controllers/            # Contrôleur principal (LivresController.cs)
├── Data/                   # Contexte EF Core (BooksContext.cs)
├── Models/                 # Modèles de données (Media, Ebook, PaperBook, etc.)
├── Repositories/           # Pattern Repository (IRepository, Repository)
├── Migrations/             # Migrations EF Core
├── appsettings.json        # Configuration générale
└── Program.cs              # Point d’entrée de l’API
📌 Fonctionnalités principales
📖 CRUD complet sur les livres (Ebooks et livres papier)

🔎 Filtrage par type de média

💾 Base de données SQLite

🧪 Documentation Swagger intégrée

📥 Installation & Exécution
Cloner le dépôt :

bash
Copier
Modifier
git clone <url>
Restaurer les dépendances :

bash
Copier
Modifier
dotnet restore
Appliquer les migrations et lancer :

bash
Copier
Modifier
dotnet ef database update
dotnet run
Accéder à l’API via Swagger :

bash
Copier
Modifier
http://localhost:<port>/swagger
📤 Endpoints principaux
Méthode	Route	Description
GET	/api/livres	Récupérer tous les livres
GET	/api/livres/{id}	Récupérer un livre par ID
POST	/api/livres	Ajouter un nouveau livre
PUT	/api/livres/{id}	Modifier un livre existant
DELETE	/api/livres/{id}	Supprimer un livre

🙌 Remarques
Le projet suit le pattern Repository pour séparer la logique métier et les accès aux données.

Tous les modèles implémentent une interface IReadable commune.

Swagger est activé en mode développement uniquement.

