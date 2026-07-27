namespace PagoDirecto.Domain.Entities
{
    public class DatabaseMapping<T>
    {
        public List<T> Records { get; set; } = new List<T>();
        public int? TotalRecords { get; set; }
    }
}

