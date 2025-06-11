using System;
using System.ComponentModel.DataAnnotations;

namespace BooksAPI.Models
{
   
    public class CreateMediaDto
    {
        
        [Required] public string Title { get; set; } = null!;
        [Required] public string Author { get; set; } = null!;
        [Required] public DateTime PublishedDate { get; set; }

        // Pour PaperBook
        [Range(1, int.MaxValue)]
        public int? NumberOfPages { get; set; }

        // Pour Ebook
        [Url]
        public string? DownloadUrl { get; set; }
    }
}
