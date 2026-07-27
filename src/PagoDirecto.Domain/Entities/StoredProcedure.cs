using System.Data.Common;

namespace PagoDirecto.Domain.Entities;

public class StoredProcedure
{
    public string ProcedureName { get; set; } = string.Empty;
    public object? Parameters { get; set; }
    public DbCommand? DbCommand { get; set; }
}

