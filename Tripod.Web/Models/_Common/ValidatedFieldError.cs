namespace Tripod.Web.Models
{
    public class ValidatedFieldError
    {
        public string Message { [UsedImplicitly] get; set; }
        public object CustomState { [UsedImplicitly] get; set; }
    }
}