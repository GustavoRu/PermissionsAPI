using BackendApi.Permissions.DTOs;

namespace BackendApi.Permissions.Services
{
    public interface IPermissionService
    {
        public List<string> Errors { get; }
        Task<IEnumerable<PermissionDto>> GetAll();
        Task<PermissionDto> GetById(int id);
        Task<PermissionDto> Create(PermissionInsertDto permissionInsertDto);
        Task<PermissionDto> Update(int id, PermissionUpdateDto permissionUpdateDto);
        Task<PermissionDto> Delete(int id);

        bool Validate(PermissionInsertDto dto);
        bool Validate(PermissionUpdateDto dto);
    }
} 