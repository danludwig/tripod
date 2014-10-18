namespace Tripod.Domain.Security
{
    public class EmailAddressWithSpecifiedId : EmailAddress
    {
        public EmailAddressWithSpecifiedId(int id)
        {
            Id = id;
        }
    }
}