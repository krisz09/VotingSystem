namespace VotingSystem.AdminClient.Services
{
    public enum ToastLevel
    {
        Info,
        Success,
        Warning,
        Error
    }

    public class ToastService : IToastService
    {
        public event Action<string, ToastLevel>? OnShow;

        public void ShowToast(string message, ToastLevel level = ToastLevel.Info)
        {
            OnShow?.Invoke(message, level);
        }
    }
}
