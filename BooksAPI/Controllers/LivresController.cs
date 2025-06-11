using BooksAPI.Models;
using BooksAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BooksAPI.Controllers
{
    [Route("livres")]
    [ApiController]
    public class LivresController : ControllerBase
    {
        private readonly IRepository<Media> _repo;

        public LivresController(IRepository<Media> repo)
        {
            _repo = repo;
        }

        // GET /livres
  [HttpGet]
public async Task<IActionResult> GetAll(
    [FromQuery] string? author,
    [FromQuery] string? title,
    [FromQuery] string? sort)
{
    var query = _repo.Query();

    if (!string.IsNullOrWhiteSpace(author))
        query = query.Where(m => m.Author.Contains(author));

    if (!string.IsNullOrWhiteSpace(title))
        query = query.Where(m => m.Title.Contains(title));

    if (string.Equals(sort, "author", StringComparison.OrdinalIgnoreCase))
        query = query.OrderBy(m => m.Author);
    else if (string.Equals(sort, "title", StringComparison.OrdinalIgnoreCase))
        query = query.OrderBy(m => m.Title);

    var mediaList = await query.ToListAsync();

    var dtoList = mediaList.Select(m => new CreateMediaDto
    {
        Title = m.Title,
        Author = m.Author,
        PublishedDate = m.PublishedDate,
        NumberOfPages = (m as PaperBook)?.NumberOfPages,
        DownloadUrl = (m as Ebook)?.DownloadUrl
    }).ToList();

    return Ok(dtoList);
}



        // GET /livres/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var item = await _repo.GetByIdAsync(id);
            return item == null ? NotFound() : Ok(item);
        }

        // POST /livres
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateMediaDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Media media;
            if (dto.NumberOfPages.HasValue)
            {
                media = new PaperBook
                {
                    Title = dto.Title,
                    Author = dto.Author,
                    PublishedDate = dto.PublishedDate,
                    NumberOfPages = dto.NumberOfPages.Value
                };
            }
            else if (!string.IsNullOrWhiteSpace(dto.DownloadUrl))
            {
                media = new Ebook
                {
                    Title = dto.Title,
                    Author = dto.Author,
                    PublishedDate = dto.PublishedDate,
                    DownloadUrl = dto.DownloadUrl!
                };
            }
            else
            {
                return BadRequest("Soit NumberOfPages (pour PaperBook), soit DownloadUrl (pour Ebook) doit être fourni.");
            }

            var created = await _repo.AddAsync(media);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        // PUT /livres/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateMediaDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            // Mise à jour des champs communs
            existing.Title = dto.Title;
            existing.Author = dto.Author;
            existing.PublishedDate = dto.PublishedDate;

            // Mise à jour spécifique selon le type
            if (existing is PaperBook paper)
            {
                if (dto.NumberOfPages.HasValue)
                    paper.NumberOfPages = dto.NumberOfPages.Value;
                else
                    return BadRequest("Pour un Livre papier, NumberOfPages est requis.");
            }
            else if (existing is Ebook ebook)
            {
                if (!string.IsNullOrWhiteSpace(dto.DownloadUrl))
                    ebook.DownloadUrl = dto.DownloadUrl!;
                else
                    return BadRequest("Pour un Ebook, DownloadUrl est requis.");
            }
            else
            {
                return BadRequest("Type de média non reconnu.");
            }

            await _repo.UpdateAsync(existing);
            return NoContent();
        }

        // DELETE /livres/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            await _repo.DeleteAsync(existing);
            return NoContent();
        }
    }
}
