using FluentValidation.Results;

namespace Tripod
{
    public interface IProcessValidation
    {
        ValidationResult Validate(IDefineCommand data);
        ValidationResult Validate<TResult>(IDefineQuery<TResult> data);
    }
}
