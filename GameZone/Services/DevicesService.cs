
using GameZone.Models;
using Microsoft.EntityFrameworkCore;

namespace GameZone.Services
{
    public class DevicesService : IDevicesService
    {
        private readonly ApplicationDbContext _context;
        public DevicesService(ApplicationDbContext context)
        {
            _context = context;
        }
        public IEnumerable<SelectListItem> GetDevicesList()
        {
            var devices = _context.Devices.Select(device => new SelectListItem
            {
                Value = device.Id.ToString(),
                Text = device.Name
            })
            .OrderBy(device => device.Text)
            .AsNoTracking()
            .ToList();

            return devices;
        }
    }
}
