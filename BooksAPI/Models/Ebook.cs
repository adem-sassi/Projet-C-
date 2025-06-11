using System.ComponentModel.DataAnnotations;

namespace BooksAPI.Models
{
    public class Ebook : Media, IReadable
    {
        [Required]
        public string DownloadUrl { get; set; } = null!;

        public void DisplayInformation()
        {
            Console.WriteLine($"[EBOOK] {Title} by {Author}, published {PublishedDate:d}. Download: {DownloadUrl}");
        }
    }
}
