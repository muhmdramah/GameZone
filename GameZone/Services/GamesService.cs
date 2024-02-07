namespace GameZone.Services
{
    public class GamesService : IGamesService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string _imagesPath;

        public GamesService(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _imagesPath = $"{_webHostEnvironment.WebRootPath}{FileSettings.ImagesPath}";
        }
        public async Task Create(CreateGameFormViewModel model)
        {
            var coverName = await SaveCover(model.Cover);

            Game game = new Game
            {
                Name = model.Name,
                Description = model.Description,
                CategoryId = model.CategoryId,
                Cover = coverName,
                Devices = model.SelectedDevices.Select(device => new GameDevice
                {
                    DeviceId = device
                }).
                ToList()
            };

            _context.Games.Add(game);
            _context.SaveChanges();
        }

        public bool Delete(int id)
        {
            var isDeleted = false;

            var game = _context.Games.Find(id);

            if (game is null) 
                return isDeleted;

            _context.Games.Remove(game);
            
            var effectedRows = _context.SaveChanges();
            
            if(effectedRows > 0) 
            { 
                isDeleted = true;

                var cover = Path.Combine(_imagesPath, game.Cover);

                File.Delete(cover);
            }

            return isDeleted;
        }

        public IEnumerable<Game> GetAll()
        {
            var games = _context.Games
                .Include(game => game.Category)
                .Include(game => game.Devices)
                .ThenInclude(device  => device.Device)
                .AsNoTracking()
                .ToList();

            return games ;
        }

        public Game? GetById(int id)
        {
            var game  = _context.Games
                .Include(game => game.Category)
                .Include(game => game.Devices)
                .ThenInclude(device => device.Device)
                .AsNoTracking()
                .SingleOrDefault(game => game.Id == id);

            return game;
        }

        public async Task<Game?> Update(EditGameFormViewModel model)
        {
            var game = _context.Games
                .Include(game => game.Devices)
                .SingleOrDefault(game => game.Id == model.Id);
            if (game is null)
                return null;

            var hasNewCover = model.Cover is not null;
            var oldCover = game.Cover;

            game.Name = model.Name;
            game.Description = model.Description;
            game.CategoryId = model.CategoryId;
            game.Devices = model.SelectedDevices.Select(device => new GameDevice
            {
                DeviceId = device
            })
            .ToList();

            if (hasNewCover)
            {
                game.Cover = await SaveCover(model.Cover!);
            }

            var effectedRows = _context.SaveChanges();

            if(effectedRows > 0)
            {
                if (hasNewCover)
                {
                    var cover = Path.Combine(_imagesPath, oldCover);
                    File.Delete(cover);
                }

                return game;
            }
            else
            {
                var cover = Path.Combine(_imagesPath, game.Cover);
                File.Delete(cover);
                return null;
            }
        }
        private async Task<string> SaveCover(IFormFile cover)
        {
            var coverName = $"{Guid.NewGuid()}{Path.GetExtension(cover.FileName)}";

            var path = Path.Combine(_imagesPath, coverName);

            using var stream = File.Create(path);
            await cover.CopyToAsync(stream);

            return coverName;
        }
    }
}
