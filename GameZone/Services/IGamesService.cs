namespace GameZone.Services
{
    public interface IGamesService
    {
        Task Create(CreateGameFormViewModel model);
        IEnumerable<Game> GetAll();
        Game? GetById(int id);
        Task<Game?> Update(EditGameFormViewModel model);
        bool Delete(int id);
    }
}
