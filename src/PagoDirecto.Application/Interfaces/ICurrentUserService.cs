namespace PagoDirecto.Application.Interfaces;

public interface ICurrentUserService
{
    long UserRecordId { get; }
    int CompanyRecordId { get; }
}
