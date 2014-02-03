namespace Tripod.Web.Models
{
    public class AlertModel
    {
        public AlertFlavor Flavor { get; set; }
        public bool IsDismissable { get; set; }
        public string Message { get; set; }
    }
}