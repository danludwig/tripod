using System;

namespace Tripod.Domain.Security
{
    public class RandomSecret : IDefineQuery<string>
    {
        public RandomSecret(int minLength, int maxLength)
        {
            if (minLength < 1)
                throw new ArgumentOutOfRangeException("minLength",
                    string.Format(Resources.Exception_ArgumentOutOfRange_CannotBeLessThan, 1));
            if (maxLength < minLength)
                throw new ArgumentOutOfRangeException("maxLength",
                    string.Format(Resources.Exception_ArgumentOutOfRange_CannotBeLessThan, "minLength"));

            MinLength = minLength;
            MaxLength = maxLength;
        }

        [UsedImplicitly]
        public RandomSecret(int exactLength)
        {
            if (exactLength < 1)
                throw new ArgumentOutOfRangeException("exactLength",
                    string.Format(Resources.Exception_ArgumentOutOfRange_CannotBeLessThan, 1));

            MinLength = exactLength;
            MaxLength = exactLength;
        }

        public int MinLength { get; private set; }
        public int MaxLength { get; private set; }
    }

    [UsedImplicitly]
    public class HandleRandomSecretQuery : IHandleQuery<RandomSecret, string>
    {
        private readonly ICreateSecrets _secrets;

        public HandleRandomSecretQuery(ICreateSecrets secrets)
        {
            _secrets = secrets;
        }

        public string Handle(RandomSecret query)
        {
            return _secrets.CreateSecret(query.MinLength, query.MaxLength);
        }
    }
}
