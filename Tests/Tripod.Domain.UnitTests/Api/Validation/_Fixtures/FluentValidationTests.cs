using FluentValidation;

namespace Tripod
{
    public abstract class FluentValidationTests
    {
        protected FluentValidationTests()
        {
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;
            ValidatorOptions.ResourceProviderType = typeof(Resources);
        }
    }
}
