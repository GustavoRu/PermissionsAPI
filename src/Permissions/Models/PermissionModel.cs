using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PermissionsAPI.Permissions.Models
{
    public class PermissionModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string EmployeeName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string EmployeeSurname { get; set; } = string.Empty;

        [Required]
        [ForeignKey("PermissionType")]
        public int PermissionTypeId { get; set; }

        [Required]
        public DateTime PermissionDate { get; set; }

        // Navigation property
        public virtual PermissionTypeModel PermissionType { get; set; } = null!;
    }
} 