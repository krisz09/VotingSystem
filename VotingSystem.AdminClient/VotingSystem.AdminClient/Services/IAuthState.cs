namespace VotingSystem.AdminClient.Services
{
    public interface IAuthState
    {
        bool IsLoggedIn { get; }
        event Action OnChange;
        void SetLoginState(bool isLoggedIn);
    }
}