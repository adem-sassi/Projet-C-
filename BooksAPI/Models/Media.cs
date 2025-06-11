namespace BooksAPI.Models
{
    public abstract class Media
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Author { get; set; } = null!;
        public DateTime PublishedDate { get; set; }
    }
}
