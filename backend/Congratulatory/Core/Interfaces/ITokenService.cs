namespace CoreService.Core.Interfaces;

public interface ITokenService
{
    string GenerateToken(string nameIdentifier, string userName, string role);
}