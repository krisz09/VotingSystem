using System.Text.Json;

namespace VotingSystem.AdminClient.Services
{
    public abstract class BaseService
    {
        private readonly IToastService _toastService;

        protected BaseService(IToastService toastService)
        {
            _toastService = toastService;
        }

        protected async Task HandleHttpError(HttpResponseMessage response)
        {
            string responseBody = await response.Content.ReadAsStringAsync();
            try
            {
                var jsonDoc = JsonDocument.Parse(responseBody);
                if (jsonDoc.RootElement.TryGetProperty("detail", out JsonElement detailElement))
                {
                    string errorMessage = detailElement.GetString() ?? "Unknown error occured";
                    ShowErrorMessage(errorMessage);
                    return;
                }
            }
            catch
            {
                // Swallow parsing errors
            }

            ShowErrorMessage("Unknown error occured");
        }

        protected void ShowErrorMessage(string message)
        {
            _toastService.ShowToast(message);
        }
    }
}
