namespace BackendApi.Permissions.DTOs
{
    public class PermissionDto
    {
        public int Id { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string EmployeeSurname { get; set; } = string.Empty;
        public int PermissionTypeId { get; set; }
        public string PermissionTypeDescription { get; set; } = string.Empty;
        public DateTime PermissionDate { get; set; }
    }
} 