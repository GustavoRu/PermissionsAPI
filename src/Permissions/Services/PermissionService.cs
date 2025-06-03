using BackendApi.Permissions.DTOs;
using BackendApi.Permissions.Models;
using BackendApi.Permissions.Repositories;
using BackendApi.MessagingQueue.Interfaces;
using BackendApi.MessagingQueue.Services;
using BackendApi.MessagingQueue.DTOs;
using Microsoft.Extensions.Logging;

namespace BackendApi.Permissions.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly IPermissionTypeRepository _permissionTypeRepository;
        private readonly IKafkaService _kafkaService;
        private readonly ILogger<PermissionService> _logger;
        public List<string> Errors { get; }

        public PermissionService(
            IPermissionRepository permissionRepository,
            IPermissionTypeRepository permissionTypeRepository,
            IKafkaService kafkaService,
            ILogger<PermissionService> logger)
        {
            _permissionRepository = permissionRepository;
            _permissionTypeRepository = permissionTypeRepository;
            _kafkaService = kafkaService;
            _logger = logger;
            Errors = new List<string>();
        }

        public async Task<IEnumerable<PermissionDto>> GetAll()
        {
            _logger.LogInformation("Getting all permissions");
            var permissions = await _permissionRepository.GetAll();
            
            if (permissions != null)
            {
                try
                {
                    _logger.LogInformation("Sending Kafka message for GetAll operation");
                    await _kafkaService.SendMessageAsync(OperationMessageDto.Operations.Get);
                    _logger.LogInformation("Kafka message sent successfully for GetAll operation");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send Kafka message for GetAll operation. Will continue with permission retrieval.");
                    // We don't rethrow here as we want to return permissions even if Kafka fails
                }
            }
            
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
            _logger.LogInformation("Creating new permission for employee: {EmployeeName} {EmployeeSurname}", 
                permissionInsertDto.EmployeeName, 
                permissionInsertDto.EmployeeSurname);

            var permission = new PermissionModel
            {
                EmployeeName = permissionInsertDto.EmployeeName,
                EmployeeSurname = permissionInsertDto.EmployeeSurname,
                PermissionTypeId = permissionInsertDto.PermissionTypeId,
                PermissionDate = DateTime.UtcNow
            };

            try
            {
                await _permissionRepository.Create(permission);
                await _permissionRepository.Save();
                
                try
                {
                    _logger.LogInformation("Sending Kafka message for Create operation");
                    await _kafkaService.SendMessageAsync(OperationMessageDto.Operations.Request);
                    _logger.LogInformation("Kafka message sent successfully for Create operation");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send Kafka message for Create operation. Permission was created successfully.");
                    // We don't rethrow as the permission was created successfully
                }

                return await MapPermissionToDto(permission);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create permission");
                throw;
            }
        }

        public async Task<PermissionDto> Update(int id, PermissionUpdateDto permissionUpdateDto)
        {
            _logger.LogInformation("Updating permission {Id}", id);
            
            var permission = await _permissionRepository.GetById(id);
            if (permission != null)
            {
                try
                {
                    permission.EmployeeName = permissionUpdateDto.EmployeeName;
                    permission.EmployeeSurname = permissionUpdateDto.EmployeeSurname;
                    permission.PermissionTypeId = permissionUpdateDto.PermissionTypeId;
                    permission.PermissionDate = DateTime.UtcNow;

                    _permissionRepository.Update(permission);
                    await _permissionRepository.Save();

                    try
                    {
                        _logger.LogInformation("Sending Kafka message for Update operation");
                        await _kafkaService.SendMessageAsync(OperationMessageDto.Operations.Modify);
                        _logger.LogInformation("Kafka message sent successfully for Update operation");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to send Kafka message for Update operation. Permission was updated successfully.");
                        // We don't rethrow as the permission was updated successfully
                    }

                    return await MapPermissionToDto(permission);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to update permission {Id}", id);
                    throw;
                }
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