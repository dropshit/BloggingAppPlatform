namespace BloggingApp.Application.Common.Interfaces;

public interface IHashingService
{
    void CreateHash(string password, out byte[] hash, out byte[] salt);
    bool VerifyHash(string password, byte[] hash, byte[] salt);
}
