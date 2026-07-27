namespace PagoDirecto.Domain.Entities
{
    public class DatabaseMapping<T>
    {
        public List<T> Records { get; set; }
        public int? TotalRecords { get; set; }
    }
}

