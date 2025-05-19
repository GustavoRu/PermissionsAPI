using BackendApi.Permissions.DTOs;
using BackendApi.Permissions.Models;
using BackendApi.Permissions.Repositories;

namespace BackendApi.Permissions.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly IPermissionTypeRepository _permissionTypeRepository;
        public List<string> Errors { get; }

        public PermissionService(
            IPermissionRepository permissionRepository,
            IPermissionTypeRepository permissionTypeRepository)
        {
            _permissionRepository = permissionRepository;
            _permissionTypeRepository = permissionTypeRepository;
            Errors = new List<string>();
        }

        public async Task<IEnumerable<PermissionDto>> GetAll()
        {
            var permissions = await _permissionRepository.GetAll();
            return await MapPermissionsToDtos(permissions);
        }

        public async Task<PermissionDto> GetById(int id)
        {
            var permission = await _permissionRepository.GetById(id);
            if (permission != null)
            {
                return await MapPermissionToDto(permission);
            }
            return null;
        }

        public async Task<PermissionDto> Create(PermissionInsertDto permissionInsertDto)
        {
            var permission = new PermissionModel
            {
                EmployeeName = permissionInsertDto.EmployeeName,
                EmployeeSurname = permissionInsertDto.EmployeeSurname,
                PermissionTypeId = permissionInsertDto.PermissionTypeId,
                PermissionDate = DateTime.UtcNow
            };

            await _permissionRepository.Create(permission);
            await _permissionRepository.Save();

            return await MapPermissionToDto(permission);
        }

        public async Task<PermissionDto> Update(int id, PermissionUpdateDto permissionUpdateDto)
        {
            var permission = await _permissionRepository.GetById(id);
            if (permission != null)
            {
                permission.EmployeeName = permissionUpdateDto.EmployeeName;
                permission.EmployeeSurname = permissionUpdateDto.EmployeeSurname;
                permission.PermissionTypeId = permissionUpdateDto.PermissionTypeId;
                permission.PermissionDate = DateTime.UtcNow;

                _permissionRepository.Update(permission);
                await _permissionRepository.Save();

                return await MapPermissionToDto(permission);
            }
            return null;
        }

        public async Task<PermissionDto> Delete(int id)
        {
            var permission = await _permissionRepository.GetById(id);
            if (permission != null)
            {
                _permissionRepository.Delete(permission);
                await _permissionRepository.Save();
                return await MapPermissionToDto(permission);
            }
            return null;
        }

        public bool Validate(PermissionInsertDto permissionInsertDto)
        {
            var permissionType = _permissionTypeRepository.GetById(permissionInsertDto.PermissionTypeId).Result;
            if (permissionType == null)
            {
                Errors.Add("El tipo de permiso especificado no existe");
                return false;
            }
            return true;
        }

        public bool Validate(PermissionUpdateDto permissionUpdateDto)
        {
            var permissionType = _permissionTypeRepository.GetById(permissionUpdateDto.PermissionTypeId).Result;
            if (permissionType == null)
            {
                Errors.Add("El tipo de permiso especificado no existe");
                return false;
            }
            return true;
        }

        private async Task<PermissionDto> MapPermissionToDto(PermissionModel permission)
        {
            var permissionType = await _permissionTypeRepository.GetById(permission.PermissionTypeId);
            return new PermissionDto
            {
                Id = permission.Id,
                EmployeeName = permission.EmployeeName,
                EmployeeSurname = permission.EmployeeSurname,
                PermissionTypeId = permission.PermissionTypeId,
                PermissionTypeDescription = permissionType?.Description ?? string.Empty,
                PermissionDate = permission.PermissionDate
            };
        }

        private async Task<IEnumerable<PermissionDto>> MapPermissionsToDtos(IEnumerable<PermissionModel> permissions)
        {
            var dtos = new List<PermissionDto>();
            foreach (var permission in permissions)
            {
                dtos.Add(await MapPermissionToDto(permission));
            }
            return dtos;
        }
    }
} 