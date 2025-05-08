namespace VotingSystem.AdminClient.Services
{
    public class AuthState
    {
        public bool IsLoggedIn { get; private set; }
        public event Action? OnChange;

        public void SetLoginState(bool isLoggedIn)
        {
            IsLoggedIn = isLoggedIn;
            OnChange?.Invoke();
        }
    }
}
