namespace Tripod
{
    public interface ICreateSecrets
    {
        string CreateSecret(int minLength, int maxLength);
        [UsedImplicitly]
        string CreateSecret(int exactLength);
    }
}
