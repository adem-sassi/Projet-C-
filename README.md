
🎯 BooksClient - Client Console
Développé par : Adem Sassi
Date : Juin 2025

🧾 Description
BooksClient est une application console développée en C# (.NET 7) permettant de consommer les services de l’API REST BooksAPI. Elle offre une interface en ligne de commande permettant aux utilisateurs d’interagir avec la bibliothèque numérique (consultation, ajout, suppression, etc.).

⚙️ Technologies
.NET 7

C#

System.Net.Http.Json (pour la consommation d'API)

📁 Structure
bash
Copier
Modifier
BooksClient/
├── Program.cs          # Point d’entrée de l’application
├── ApiService.cs       # Classe de communication avec l’API
├── Models.cs           # Modèles locaux alignés avec ceux de l’API
├── *.csproj            # Fichier de projet .NET
✅ Fonctionnalités
🔍 Liste des livres disponibles

➕ Ajout de nouveaux livres

🗑️ Suppression de livres

📝 Interaction simple via la console

▶️ Lancer l'application
S’assurer que l’API BooksAPI est lancée.

Se placer dans le dossier BooksClient :

bash
Copier
Modifier
cd BooksClient
Lancer le client :

bash
Copier
Modifier
dotnet run
🔗 Dépendances
BooksClient utilise HttpClient et System.Text.Json pour interagir avec l’API.

Assurez-vous que les endpoints de l’API sont accessibles depuis le client.

🧪 Exemple d'utilisation
markdown
Copier
Modifier
Bienvenue dans la bibliothèque 📚

1. Voir tous les livres
2. Ajouter un livre
3. Supprimer un livre
4. Quitter

Votre choix : _
