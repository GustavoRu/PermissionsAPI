namespace BackendApi.Permissions.DTOs
{
    public class PermissionInsertDto
    {
        public string EmployeeName { get; set; } = string.Empty;
        public string EmployeeSurname { get; set; } = string.Empty;
        public int PermissionTypeId { get; set; }
        // public DateTime PermissionDate { get; set; }
    }
} 