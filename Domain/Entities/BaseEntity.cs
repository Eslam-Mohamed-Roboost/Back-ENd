using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Domain.Entities;

public class BaseEntity
{
    [Required]
    public long ID { get; set; }
    public long CompanyID { get; set; }
    
    [Column(TypeName = "timestamp with time zone")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public long? CreatedBy { get; set; }
    
    [Column(TypeName = "timestamp with time zone")]
    public DateTime? UpdatedAt { get; set; }
    public long? UpdatedBy { get; set; }
    
    public bool IsDeleted { get; set; } = false;
    public DateTime? TTL { get; set; }
    public byte[] RowVersion { get; set; } = new byte[8];
}

