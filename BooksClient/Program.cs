// Classe principale qui contient le point d'entrée du programme
class Program
{
    // Client HTTP static et readonly pour envoyer des requêtes à l'APIBaseAddress est configuré pour pointer vers l'URL de l'API localede mo, pc soit le port 5229
    static readonly HttpClient client = new()
    {
        BaseAddress = new Uri("http://localhost:5229/")
    };

    // Fonction utilitaire pour mettre en pause l'exécution du programme
    // et attendre que l'utilisateur appuie sur Entrée
    static void Pause()
    {
        Console.WriteLine("\nAppuyez sur Entrée pour continuer...");
        Console.ReadLine(); 
    }

    // Méthode asynchrone pour lister tous les livres disponibles sur le serveur
    static async Task ListAll()
    {
        try
        {
            // Envoie une requête GET à l'endpoint "livres" et désérialise la réponse en List<Media>
            var livres = await client.GetFromJsonAsync<List<Media>>("livres");
            Console.WriteLine();
            
            // Vérifie si la liste est vide ou nulle
            if (livres == null || livres.Count == 0)
                Console.WriteLine("Aucun livre trouvé."); // Message si aucun livre n'est disponible
            else
                // Affiche chaque livre avec son ID, titre et auteur
                livres.ForEach(l => Console.WriteLine($"{l.Id}: {l.Title} by {l.Author}"));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de la récupération : {ex.Message}"); // Gestion des erreurs
        }
        Pause(); // Met en pause après l'opération
    }

    // Méthode pour rechercher des livres selon des critères (auteur et/ou titre)
    static async Task Search()
    {
        // Demande à l'utilisateur les critères de recherche
        Console.Write("Auteur (vide = skip) : ");
        var author = Console.ReadLine() ?? "";
        Console.Write("Titre (vide = skip)  : ");
        var title  = Console.ReadLine() ?? "";

        // Construit l'URL avec les paramètres de recherche encodés
        var url = $"livres?author={Uri.EscapeDataString(author)}&title={Uri.EscapeDataString(title)}";
        
        try
        {
            // Envoie la requête GET avec les paramètres et désérialise la réponse
            var livres = await client.GetFromJsonAsync<List<Media>>(url);
            Console.WriteLine();
            
            if (livres == null || livres.Count == 0)
                Console.WriteLine("Aucun résultat."); // Aucun livre ne correspond aux critères
            else
                livres.ForEach(l => Console.WriteLine($"{l.Id}: {l.Title} by {l.Author}")); // Affiche les résultats
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de la recherche : {ex.Message}"); // Gestion des erreurs
        }
        Pause();
    }

    // Méthode pour ajouter un nouveau livre à la base de données
    static async Task Add()
    {
        // Collecte les informations de base sur le livre
        Console.Write("Titre du livre : ");
        var title = Console.ReadLine() ?? "";

        Console.Write("Auteur                          : ");
        var author = Console.ReadLine() ?? "";

        Console.Write("Date de publication (yyyy-MM-dd): ");
        DateTime publishedDate;
        // Valide et parse la date de publication
        while (!DateTime.TryParse(Console.ReadLine(), out publishedDate))
            Console.Write("Format invalide. Réessayez (yyyy-MM-dd) : "); 
        
        Console.Write("Type (1 = PaperBook, 2 = Ebook) : ");
        var type = Console.ReadLine()?.Trim();

        // Crée un DTO (Data Transfer Object) avec les informations collectées
        var dto = new CreateMediaDto
        {
            Title = title,
            Author = author,
            PublishedDate = publishedDate
        };

        // Si c'est un livre papier (type 1), demande le nombre de pages
        if (type == "1")
        {
            Console.Write("Nombre de pages                : ");
            dto.NumberOfPages = int.TryParse(Console.ReadLine(), out var np) ? np : 0;
        }
        else
        {
            // Si c'est un ebook (type 2), demande l'URL de téléchargement
            Console.Write("URL de téléchargement          : ");
            dto.DownloadUrl = Console.ReadLine();
        }

        try
        {
            // Envoie une requête POST avec le DTO sérialisé en JSON
            var resp = await client.PostAsJsonAsync("livres", dto);
            resp.EnsureSuccessStatusCode(); // Vérifie que la requête a réussi
            Console.WriteLine("Livre ajouté avec succès !"); // Confirmation de l'ajout
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de l'ajout     : {ex.Message}"); // Gestion des erreurs
        }
        Pause();
    }

    // Méthode pour modifier un livre existant
    static async Task Update()
    {
        Console.Write("ID du livre à modifier         : ");
        // Valide l'ID entré par l'utilisateur
        if (!int.TryParse(Console.ReadLine(), out var id))
        {
            Console.WriteLine("ID invalide."); Pause(); return;
        }

        Media? existing = null;
        try
        {
            // Récupère le livre existant via une requête GET
            existing = await client.GetFromJsonAsync<Media>($"livres/{id}");
        }
        catch { }

        if (existing == null)
        {
            Console.WriteLine("Livre non trouvé."); Pause(); return;
        }

        // Permet à l'utilisateur de modifier chaque champ (ne change que si une valeur est fourniesinon ca reste la meme chose )
        Console.Write($"Nouveau titre (actuel : {existing.Title})        : ");
        var newTitle = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(newTitle))
            existing.Title = newTitle;

        Console.Write($"Nouvel auteur (actuel : {existing.Author})       : ");
        var newAuthor = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(newAuthor))
            existing.Author = newAuthor;

        Console.Write($"Nouvelle date (yyyy-MM-dd) (actuelle : {existing.PublishedDate:yyyy-MM-dd}) : ");
        var inputDate = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(inputDate) && DateTime.TryParse(inputDate, out var newDate))
            existing.PublishedDate = newDate;

        // Gestion spécifique selon le type de livre (papier ou ebook)
        if (existing.NumberOfPages.HasValue)
        {
            Console.Write($"Nbre de pages (actuel : {existing.NumberOfPages}) : ");
            if (int.TryParse(Console.ReadLine(), out var np))
                existing.NumberOfPages = np;
        }
        else
        {
            Console.Write($"URL téléchargement (actuel : {existing.DownloadUrl}) : ");
            var url = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(url))
                existing.DownloadUrl = url;
        }

        try
        {
            // Crée un DTO avec les nouvelles valeurs et envoie une requête PUT
            var dto = new CreateMediaDto
            {
                Title = existing.Title,
                Author = existing.Author,
                PublishedDate = existing.PublishedDate,
                NumberOfPages = existing.NumberOfPages,
                DownloadUrl = existing.DownloadUrl
            };
            var resp = await client.PutAsJsonAsync($"livres/{id}", dto);
            resp.EnsureSuccessStatusCode();
            Console.WriteLine("Livre modifié avec succès !"); // Confirmation de la modification
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de la modification : {ex.Message}"); // Gestion des erreurs
        }
        Pause();
    }

    // Méthode pour supprimer un livre par son ID
    static async Task Delete()
    {
        Console.Write("ID du livre à supprimer        : ");
        // Valide l'ID entré par l'utilisateur
        if (!int.TryParse(Console.ReadLine(), out var id))
        {
            Console.WriteLine("ID invalide."); Pause(); return;
        }

        try
        {
            // Envoie une requête DELETE à l'endpoint correspondant
            var resp = await client.DeleteAsync($"livres/{id}");
            if (resp.IsSuccessStatusCode)
                Console.WriteLine("Livre supprimé avec succès !"); // Confirmation de la suppressionsinion on passe a l'etape ou ya un echec 
            else
                Console.WriteLine($"Échec suppression : {resp.StatusCode}"); // Message d'échec
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de la suppression : {ex.Message}"); // Gestion des erreurs
        }
        Pause();
    }

    // Point d'entrée principal du programme
    static async Task Main(string[] args)
    {
        // Boucle principale du menu
        while (true)
        {
            Console.Clear(); // Nettoie la console à chaque itération
            Console.WriteLine("=== Books Client ===");
            Console.WriteLine("1) Liste");
            Console.WriteLine("2) Recherche");
            Console.WriteLine("3) Ajout");
            Console.WriteLine("4) Modification");
            Console.WriteLine("5) Suppression");
            Console.WriteLine("0) Quitter");
            Console.Write("Votre choix : ");

            var choice = Console.ReadLine()?.Trim(); // Lit le choix de entrer par l'utilisateur
            switch (choice)
            {
                case "1": await ListAll();   break; // Liste tous les livres
                case "2": await Search();    break; // Effectue une recherche
                case "3": await Add();       break; // Ajoute un nouveau livre
                case "4": await Update();    break; // Modifie un livre existant
                case "5": await Delete();    break; // Supprime un livre
                case "0": return; // Quitte le programme
                default:
                    Console.WriteLine("Option invalide, réessayez."); // Choix non valide
                    Pause();
                    break;
            }
        }
    }
}