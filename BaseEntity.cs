using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Adroit_v8
{
    public class BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public string UniqueId { get; set; } = null!;
        public string? clientid { get; set; }
        public string? Createdby { get; set; }
        [NotMapped]
        public string? StatusName { get; set; }
        public int Isdeleted { get; set; } = 0;
        public int? Status { get; set; }
    } 
    public class BaseEntityForCRM
    {
        [Key]
        public int Id { get; set; }
        [NotMapped]
        public string? StatusName { get; set; }
        public int? Status { get; set; }
        public string? ClientId { get; set; }
        public string? CreatedBy { get; set; }
    }
}