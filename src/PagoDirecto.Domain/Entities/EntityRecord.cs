using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PagoDirecto.Domain.Entities;

public class EntityRecord
{
    [Required]
    [Column("UserRecordId", TypeName = "bigint")]
    public long UserRecordId { get; set; }

    [Required]
    public int CompanyRecordId { get; set; }

    [Required]
    public bool IsActive { get; set; } = true;

    [Required]
    [Column("RecordFecha", TypeName = "DateTime")]
    public DateTime CreatedDate { get; set; } = DateTime.Now;
}

