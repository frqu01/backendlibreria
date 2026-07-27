using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagoDirecto.Domain.Entities
{
    public record Parameter
    {
        public long UserRecordId { get; set; }
        public int CompanyRecordId { get; set; }
    }
    #region Validaciones
    public class ParameterValidator : AbstractValidator<Parameter>
    {
        public ParameterValidator()
        {
            #region Validaciones del caso de uso
            RuleFor(t => t.UserRecordId).Cascade(CascadeMode.Stop).NotNull().NotEmpty().Must(x => x > 0);
            RuleFor(t => t.CompanyRecordId).Cascade(CascadeMode.Stop).NotNull().NotEmpty().Must(x => x > 0);
            #endregion
        }
    }
    #endregion
}

