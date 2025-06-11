using BibliothequeClient.Models;

class Program
{
    static async Task Main()
    {
        var api = new ApiService();
        bool running = true;

        while (running)
        {
            Console.WriteLine("\n--- MENU ---");
            Console.WriteLine("1. Afficher tous les livres");
            Console.WriteLine("2. Rechercher un livre par ID");
            Console.WriteLine("3. Ajouter un livre");
            Console.WriteLine("4. Modifier un livre");
            Console.WriteLine("5. Supprimer un livre");
            Console.WriteLine("6. Quitter");
            Console.Write("Choix : ");
            var input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    var list = await api.GetAllAsync();
                    Console.WriteLine("\n📚 Liste des livres :\n");
                    foreach (var item in list)
                    {
                        string type = item.NumberOfPages != null ? "PaperBook" : "Ebook";
                        string detail = item.NumberOfPages != null
                            ? $"{item.NumberOfPages} pages"
                            : item.DownloadUrl ?? "-";
                        string icon = item.NumberOfPages != null ? "📕" : "📘";

                        Console.WriteLine($"{icon} [{item.Id}] {item.Title} - {item.Author} - {detail} - {type} - {item.PublishedDate:dd/MM/yyyy}");
                    }
                    break;

                case "2":
                    int idSearch = PromptInt("ID");
                    var result = await api.GetByIdAsync(idSearch);
                    if (result != null)
                    {
                        string type = result.NumberOfPages != null ? "PaperBook" : "Ebook";
                        string detail = result.NumberOfPages != null
                            ? $"{result.NumberOfPages} pages"
                            : result.DownloadUrl ?? "-";
                        Console.WriteLine($"\n📖 [{result.Id}] {result.Title} - {result.Author} - {detail} - {type} - {result.PublishedDate:dd/MM/yyyy}");
                    }
                    else Console.WriteLine("Livre non trouvé");
                    break;

                case "3":
                    var newBook = SaisirLivre(); // permet d'entrer les infos pour un livre
                    await api.AddAsync(newBook);
                    break;

                case "4":
                    int idUpdate = PromptInt("ID du livre à modifier");
                    var updatedBook = SaisirLivre(); // même chose que pour l'ajout
                    await api.UpdateAsync(idUpdate, updatedBook);
                    break;

                case "5":
                    int idDelete = PromptInt("ID du livre à supprimer");
                    await api.DeleteAsync(idDelete); // appel à l'api pour suprimer
                    break;

                case "6":
                    running = false; // quitte la boucle
                    break;
            }
        }
    }

    // Demande a l'utilisateur les info d'un livre
    static CreateMediaDto SaisirLivre()
    {
        var book = new CreateMediaDto
        {
            Title = Prompt("Titre"),
            Author = Prompt("Auteur"),
            PublishedDate = PromptDate("Date de publication (yyyy-MM-dd)")
        };

        Console.WriteLine("Type de livre :");
        Console.WriteLine("1. PaperBook");
        Console.WriteLine("2. Ebook");
        int type = PromptInt("Choix");

        if (type == 1)
        {
            book.NumberOfPages = PromptInt("Nombre de pages");
        }
        else if (type == 2)
        {
            string url;
            do
            {
                url = Prompt("URL de téléchargement (ex: https://...)");
            } while (!Uri.TryCreate(url, UriKind.Absolute, out _));
            book.DownloadUrl = url;
        }
        else
        {
            Console.WriteLine("Type invalide. Par défaut : Ebook.");
        }

        return book;
    }

    // Demande une saisie texte
    static string Prompt(string label)
    {
        Console.Write($"{label} : ");
        return Console.ReadLine() ?? "";
    }

    // Demande un entier à l'utilisateur
    static int PromptInt(string label)
    {
        Console.Write($"{label} : ");
        int.TryParse(Console.ReadLine(), out int val);
        return val;
    }

    // Demande une date, sinon met la date du jour
    static DateTime PromptDate(string label)
    {
        Console.Write($"{label} : ");
        return DateTime.TryParse(Console.ReadLine(), out DateTime dt)
            ? dt
            : DateTime.Now;
    }
}
