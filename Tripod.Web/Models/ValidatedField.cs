using System.Collections.Generic;
using System.Linq;

namespace Tripod.Web.Models
{
    public class ValidatedField
    {
        public object AttemptedValue { get; set; }

        public string AttemptedString
        {
            get { return AttemptedValue == null ? null : AttemptedValue.ToString(); }
        }

        public IList<ValidatedFieldError> Errors { get; set; }

        public bool IsValid
        {
            get { return Errors == null || !Errors.Any(); }
        }
    }
}