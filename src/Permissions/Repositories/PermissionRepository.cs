using Microsoft.EntityFrameworkCore;
using BackendApi.Permissions.Models;
using BackendApi.Data;

namespace BackendApi.Permissions.Repositories
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly ApplicationDbContext _context;

        public PermissionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PermissionModel>> GetAll()
        {
            return await _context.Permissions
                .Include(p => p.PermissionType)
                .ToListAsync();
        }

        public async Task<PermissionModel> GetById(int id)
        {
            return await _context.Permissions
                .Include(p => p.PermissionType)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task Create(PermissionModel permission)
        {
            await _context.Permissions.AddAsync(permission);
        }

        public void Update(PermissionModel permission)
        {
            _context.Permissions.Attach(permission);
            _context.Entry(permission).State = EntityState.Modified;
        }

        public void Delete(PermissionModel permission)
        {
            _context.Permissions.Remove(permission);
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }

        public IEnumerable<PermissionModel> Search(Func<PermissionModel, bool> filter)
        {
            return _context.Permissions
                .Include(p => p.PermissionType)
                .Where(filter)
                .ToList();
        }
    }
} 