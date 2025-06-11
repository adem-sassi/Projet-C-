using System.ComponentModel.DataAnnotations;

namespace BooksAPI.Models
{
    public class PaperBook : Media, IReadable
    {
        [Range(1, 100)]
        public int NumberOfPages { get; set; }

        public void DisplayInformation()
        {
            Console.WriteLine($"[PAPER] {Title} by {Author}, {NumberOfPages} pages, published {PublishedDate:d}");
        }
    }
}
