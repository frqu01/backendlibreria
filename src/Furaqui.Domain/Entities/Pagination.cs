using Furaqui.Domain.Enums;
using FluentValidation;

namespace Furaqui.Domain.Entities;

public record Pagination
{
    public int CurrentPage { get; set; } = 0;
    public int PageSize { get; set; } = 50;
    public string SortBy { get; set; } = string.Empty;
    public string SortDirection { get; set; } = "ASC";
    public long UserRecordId { get; set; }
    public int CompanyRecordId { get; set; }
}

public class PaginationValidator : AbstractValidator<Pagination>
{
    public PaginationValidator()
    {
        RuleFor(t => t.CurrentPage)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage("Current page is required.")
            .GreaterThanOrEqualTo(0).WithMessage("Current page cannot be negative.");

        RuleFor(t => t.PageSize)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage("Page size is required.")
            .GreaterThan(0).WithMessage("Page size must be greater than zero.");

        RuleFor(t => t.SortBy)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage("Sort field is required.")
            .NotEmpty().WithMessage("Sort field cannot be empty.");

        RuleFor(t => t.SortDirection)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage("Sort direction is required.")
            .NotEmpty().WithMessage("Sort direction cannot be empty.")
            .Must(x => x != null && (x.ToUpper() == "ASC" || x.ToUpper() == "DESC"))
            .WithMessage("Sort direction must be 'ASC' or 'DESC'.");

        RuleFor(t => t.UserRecordId)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage("User ID is required.")
            .NotEmpty().WithMessage("User ID cannot be empty.")
            .Must(x => x > 0).WithMessage("User ID must be greater than zero.");

        RuleFor(t => t.CompanyRecordId)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage("Company ID is required.")
            .NotEmpty().WithMessage("Company ID cannot be empty.")
            .Must(x => x > 0).WithMessage("Company ID must be greater than zero.");
    }
}
