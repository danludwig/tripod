using System.Collections.Generic;
using System.Linq;

namespace Tripod.Web.Models
{
    public class ValidatedField
    {
        public object AttemptedValue { [UsedImplicitly] get; set; }

        [UsedImplicitly]
        public string AttemptedString
        {
            get { return AttemptedValue == null ? null : AttemptedValue.ToString(); }
        }

        public IList<ValidatedFieldError> Errors { get; set; }

        [UsedImplicitly]
        public bool IsValid
        {
            get { return Errors == null || !Errors.Any(); }
        }
    }
}