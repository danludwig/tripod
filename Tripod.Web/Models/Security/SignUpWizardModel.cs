using System.Collections.Generic;

namespace Tripod.Web.Models
{
    public class SignUpWizardModel
    {
        public SignUpWizardModel(int stepNumber)
        {
            StepNumber = stepNumber;
            StepContents = new Dictionary<string, string>();
        }

        public int StepNumber { get; private set; }
        public IDictionary<string, string> StepContents { get; private set; }
    }
}