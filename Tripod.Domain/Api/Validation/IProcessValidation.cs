using FluentValidation.Results;

namespace Tripod
{
    public interface IProcessValidation
    {
        ValidationResult Validate(IDefineCommand command);
        ValidationResult Validate<TResult>(IDefineQuery<TResult> query);
    }
}
