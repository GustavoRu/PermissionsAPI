using Microsoft.AspNetCore.Mvc;
using BackendApi.Permissions.DTOs;
using BackendApi.Permissions.Services;
using FluentValidation;
using BackendApi.Permissions.Validators;

namespace BackendApi.Permissions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionService _permissionService;
        private readonly IValidator<PermissionInsertDto> _permissionInsertValidator;
        private readonly IValidator<PermissionUpdateDto> _permissionUpdateValidator;

        public PermissionController(
            IPermissionService permissionService, 
            IValidator<PermissionInsertDto> permissionInsertValidator, 
            IValidator<PermissionUpdateDto> permissionUpdateValidator)
        {
            _permissionService = permissionService;
            _permissionInsertValidator = permissionInsertValidator;
            _permissionUpdateValidator = permissionUpdateValidator;
        }

        [HttpGet]
        public async Task<IEnumerable<PermissionDto>> GetAll()
        {
            var permissions = await _permissionService.GetAll();
            return permissions;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PermissionDto>> GetById(int id)
        {
            var permissionDto = await _permissionService.GetById(id);
            return permissionDto != null ? Ok(permissionDto) : NotFound();
        }

        [HttpPost("request")]
        public async Task<ActionResult<PermissionDto>> RequestPermission([FromBody] PermissionInsertDto permissionInsertDto)
        {
            var validationResult = await _permissionInsertValidator.ValidateAsync(permissionInsertDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            
            if (!_permissionService.Validate(permissionInsertDto))
            {
                return BadRequest(_permissionService.Errors);
            }

            var permissionDto = await _permissionService.Create(permissionInsertDto);
            return CreatedAtAction(nameof(GetById), new { id = permissionDto.Id }, permissionDto);
        }

        [HttpPut("modify/{id}")]
        public async Task<ActionResult<PermissionDto>> ModifyPermission(int id, PermissionUpdateDto permissionUpdateDto)
        {
            var validationResult = await _permissionUpdateValidator.ValidateAsync(permissionUpdateDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            if (!_permissionService.Validate(permissionUpdateDto))
            {
                return BadRequest(_permissionService.Errors);
            }

            var permissionDto = await _permissionService.Update(id, permissionUpdateDto);
            if (permissionDto == null)
            {
                return NotFound();
            }
            return Ok(permissionDto);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<PermissionDto>> Delete(int id)
        {
            var permissionDto = await _permissionService.Delete(id);
            if (permissionDto == null)
            {
                return NotFound();
            }
            return Ok(permissionDto);
        }
    }
}
