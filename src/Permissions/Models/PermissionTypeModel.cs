using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PermissionsAPI.Permissions.Models
{
    public class PermissionTypeModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Description { get; set; } = string.Empty;

        // Navigation property
        public virtual ICollection<PermissionModel> Permissions { get; set; } = new List<PermissionModel>();
    }
} 