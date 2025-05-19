using Microsoft.EntityFrameworkCore;
using BackendApi.Permissions.Models;
using BackendApi.Data;

namespace BackendApi.Permissions.Repositories
{
    public class PermissionTypeRepository : IPermissionTypeRepository
    {
        private readonly ApplicationDbContext _context;

        public PermissionTypeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PermissionTypeModel>> GetAll()
        {
            return await _context.PermissionTypes
                .Include(pt => pt.Permissions)
                .ToListAsync();
        }

        public async Task<PermissionTypeModel> GetById(int id)
        {
            return await _context.PermissionTypes
                .Include(pt => pt.Permissions)
                .FirstOrDefaultAsync(pt => pt.Id == id);
        }

        public async Task Create(PermissionTypeModel permissionType)
        {
            await _context.PermissionTypes.AddAsync(permissionType);
        }

        public void Update(PermissionTypeModel permissionType)
        {
            _context.PermissionTypes.Attach(permissionType);
            _context.Entry(permissionType).State = EntityState.Modified;
        }

        public void Delete(PermissionTypeModel permissionType)
        {
            _context.PermissionTypes.Remove(permissionType);
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }

        public IEnumerable<PermissionTypeModel> Search(Func<PermissionTypeModel, bool> filter)
        {
            return _context.PermissionTypes
                .Include(pt => pt.Permissions)
                .Where(filter)
                .ToList();
        }
    }
} 