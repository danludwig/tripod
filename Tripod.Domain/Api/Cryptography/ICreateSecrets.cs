namespace Tripod
{
    public interface ICreateSecrets
    {
        string CreateSecret(int minLength, int maxLength);
        string CreateSecret(int exactLength);
    }
}
