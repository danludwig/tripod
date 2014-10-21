using System;

namespace Tripod.Domain.Security
{
    /// <summary>
    /// Generate secret text of a specific length or length range.
    /// </summary>
    public class RandomSecret : IDefineQuery<string>
    {
        /// <summary>
        /// Generate secret text of a specific length range.
        /// </summary>
        /// <param name="minLength">Minimum length of the secret text to generate.</param>
        /// <param name="maxLength">Maximum length of the secret text to generate.</param>
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

        /// <summary>
        /// Generate secret text of a specific length.
        /// </summary>
        /// <param name="exactLength">Exact length of the secret text to generate.</param>
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
