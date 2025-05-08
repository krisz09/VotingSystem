namespace VotingSystem.AdminClient.Services
{
    public interface IToastService
    {
        event Action<string, ToastLevel> OnShow;
        void ShowToast(string message, ToastLevel level = ToastLevel.Info);
    }
}
