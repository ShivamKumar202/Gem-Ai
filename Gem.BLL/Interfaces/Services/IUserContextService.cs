namespace Gem.BLL.Interfaces.Services
{
    public interface IUserContextService
    {
        string UserId { get; }
        string UserName { get; }
        string Role { get; }
        bool HasRole(params string[] role);
    }
}
