// c'est la classe principal qui permet de lancer le programme
class Program
{
    // on créer le client quui vas envoyer des requetes HTTP a l’API
    static readonly HttpClient client = new()
    {
        BaseAddress = new Uri("http://localhost:5229/")
    };

    // petitefonction pour arreter le programme un peu
    static void Pause()
    {
        Console.WriteLine("\nAppuyez sur Entrée pour continuer...");
        Console.ReadLine(); 
    }

    // elle va lister tout les livre du serveur
    static async Task ListAll()
    {
        try
        {
            var livres = await client.GetFromJsonAsync<List<Media>>("livres");
            Console.WriteLine();
            if (livres == null || livres.Count == 0)
                Console.WriteLine("Aucun livre trouvé."); // ya pas de livre donc rien a montrez
            else
                livres.ForEach(l => Console.WriteLine($"{l.Id}: {l.Title} by {l.Author}")); // affiche les livre un par un
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de la récupération : {ex.Message}"); 
        }
        Pause();
    }

    // cette fonction elle sert a faire une recherche de livre
    static async Task Search()
    {
        Console.Write("Auteur (vide = skip) : ");
        var author = Console.ReadLine() ?? "";
        Console.Write("Titre (vide = skip)  : ");
        var title  = Console.ReadLine() ?? "";

        var url = $"livres?author={Uri.EscapeDataString(author)}&title={Uri.EscapeDataString(title)}";
        try
        {
            var livres = await client.GetFromJsonAsync<List<Media>>(url);
            Console.WriteLine();
            if (livres == null || livres.Count == 0)
                Console.WriteLine("Aucun résultat.");
            else
                livres.ForEach(l => Console.WriteLine($"{l.Id}: {l.Title} by {l.Author}")); // montre les livres touver
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de la recherche : {ex.Message}"); 
        }
        Pause();
    }

    // fonction pour ajouter un livre dans la base de donné
    static async Task Add()
    {
        Console.Write("Titre du livre : ");
        var title = Console.ReadLine() ?? "";

        Console.Write("Auteur                          : ");
        var author = Console.ReadLine() ?? "";

        Console.Write("Date de publication (yyyy-MM-dd): ");
        DateTime publishedDate;
        while (!DateTime.TryParse(Console.ReadLine(), out publishedDate))
            Console.Write("Format invalide. Réessayez (yyyy-MM-dd) : "); 
        Console.Write("Type (1 = PaperBook, 2 = Ebook) : ");
        var type = Console.ReadLine()?.Trim();

        var dto = new CreateMediaDto
        {
            Title = title,
            Author = author,
            PublishedDate = publishedDate
        };

        // si c’est un livre papier on demande le nombre de pages
        if (type == "1")
        {
            Console.Write("Nombre de pages                : ");
            dto.NumberOfPages = int.TryParse(Console.ReadLine(), out var np) ? np : 0;
        }
        else
        {
            // sinon on demande le lien pour le télécharger
            Console.Write("URL de téléchargement          : ");
            dto.DownloadUrl = Console.ReadLine();
        }

        try
        {
            var resp = await client.PostAsJsonAsync("livres", dto);
            resp.EnsureSuccessStatusCode(); // on verifie si c’est bien envoyer
            Console.WriteLine("Livre ajouté avec succès !"); 
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de l'ajout     : {ex.Message}"); // erreur encore une foi
        }
        Pause();
    }

    // modifier un livre qui existe deja
    static async Task Update()
    {
        Console.Write("ID du livre à modifier         : ");
        if (!int.TryParse(Console.ReadLine(), out var id))
        {
            Console.WriteLine("ID invalide."); Pause(); return;
        }

        Media? existing = null;
        try
        {
            existing = await client.GetFromJsonAsync<Media>($"livres/{id}");
        }
        catch { }

        if (existing == null)
        {
            Console.WriteLine("Livre non trouvé."); Pause(); return;
        }

        // demander les nouvelles info pour modifier le livre
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
            Console.WriteLine("Livre modifié avec succès !"); // c’est bon on a changer
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de la modification : {ex.Message}");
        }
        Pause();
    }

    // supprimer un livre grace a son ID
    static async Task Delete()
    {
        Console.Write("ID du livre à supprimer        : ");
        if (!int.TryParse(Console.ReadLine(), out var id))
        {
            Console.WriteLine("ID invalide."); Pause(); return;
        }

        try
        {
            var resp = await client.DeleteAsync($"livres/{id}");
            if (resp.IsSuccessStatusCode)
                Console.WriteLine("Livre supprimé avec succès !"); 
            else
                Console.WriteLine($"Échec suppression : {resp.StatusCode}"); 
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de la suppression : {ex.Message}");
        }
        Pause();
    }

    // le menu principale de l’application ou on choisi une action
    static async Task Main(string[] args)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== Books Client ===");
            Console.WriteLine("1) Liste");
            Console.WriteLine("2) Recherche");
            Console.WriteLine("3) Ajout");
            Console.WriteLine("4) Modification");
            Console.WriteLine("5) Suppression");
            Console.WriteLine("0) Quitter");
            Console.Write("Votre choix : ");

            var choice = Console.ReadLine()?.Trim();
            switch (choice)
            {
                case "1": await ListAll();   break;
                case "2": await Search();    break;
                case "3": await Add();       break;
                case "4": await Update();    break;
                case "5": await Delete();    break;
                case "0": return;
                default:
                    Console.WriteLine("Option invalide, réessayez."); 
                    Pause();
                    break;
            }
        }
    }
}
