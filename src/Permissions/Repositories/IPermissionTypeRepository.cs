using BackendApi.Permissions.Models;

namespace BackendApi.Permissions.Repositories
{
    public interface IPermissionTypeRepository
    {
        Task<IEnumerable<PermissionTypeModel>> GetAll();
        Task<PermissionTypeModel> GetById(int id);
        Task Create(PermissionTypeModel permissionType);
        void Update(PermissionTypeModel permissionType);
        void Delete(PermissionTypeModel permissionType);
        Task Save();
        IEnumerable<PermissionTypeModel> Search(Func<PermissionTypeModel, bool> filter);
    }
} 