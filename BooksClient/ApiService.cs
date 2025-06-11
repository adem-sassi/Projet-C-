using System.Net.Http.Json;
using BibliothequeClient.Models;

public class ApiService
{
    private readonly HttpClient _httpClient;

    public ApiService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:5222/") // ⚠️ adapte l'adresse selon ou tourne ton API
        };
    }

    // recupere tout les livres depuis l'api
    public async Task<List<CreateMediaDto>> GetAllAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<CreateMediaDto>>("livres") ?? new();
    }

    // recupere un livre par son ID
    public async Task<CreateMediaDto?> GetByIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<CreateMediaDto>($"livres/{id}");
    }

    // ajoute un nouveux livre via l'api
    public async Task AddAsync(CreateMediaDto media)
    {
        var response = await _httpClient.PostAsJsonAsync("livres", media);
        Console.WriteLine(response.IsSuccessStatusCode ? "Ajouté" : $"Erreur: {await response.Content.ReadAsStringAsync()}");
    }

    // modifie un livre existant
    public async Task UpdateAsync(int id, CreateMediaDto media)
    {
        var response = await _httpClient.PutAsJsonAsync($"livres/{id}", media);
        Console.WriteLine(response.IsSuccessStatusCode ? "Modifié" : $"Erreur: {await response.Content.ReadAsStringAsync()}");
    }

    // suprime un livre avec son id
    public async Task DeleteAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"livres/{id}");
        Console.WriteLine(response.IsSuccessStatusCode ? "Supprimé" : $"Erreur: {await response.Content.ReadAsStringAsync()}");
    }
}
