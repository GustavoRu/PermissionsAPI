using BackendApi.Permissions.Models;

namespace BackendApi.Permissions.Repositories
{
    public interface IPermissionRepository
    {
        Task<IEnumerable<PermissionModel>> GetAll();
        Task<PermissionModel> GetById(int id);
        Task Create(PermissionModel permission);
        void Update(PermissionModel permission);
        void Delete(PermissionModel permission);
        Task Save();
        IEnumerable<PermissionModel> Search(Func<PermissionModel, bool> filter);
    }
} 