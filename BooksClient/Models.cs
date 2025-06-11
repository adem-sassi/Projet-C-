namespace BibliothequeClient.Models;

public class CreateMediaDto
{
    // identifiant du livre (sert pour modif et suppression)
    public int Id { get; set; }

    // le titre du livre
    public string Title { get; set; } = "";

    // le nom de l'auteur
    public string Author { get; set; } = "";

    // date de publication
    public DateTime PublishedDate { get; set; }

    // nb de pages si c'est un livre papier (null sinon)
    public int? NumberOfPages { get; set; }

    // lien pour telecharger si c'est un ebook
    public string? DownloadUrl { get; set; }
}
