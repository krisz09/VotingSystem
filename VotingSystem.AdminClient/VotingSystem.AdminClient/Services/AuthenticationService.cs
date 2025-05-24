using AutoMapper;
using Blazored.LocalStorage;
using VotingSystem.Shared.Models;
using System.Net.Http.Json;
using VotingSystem.AdminClient.Exception;
using VotingSystem.AdminClient.Infrastructure;
using VotingSystem.AdminClient.ViewModels;

namespace VotingSystem.AdminClient.Services
{
    public class AuthenticationService : BaseService, IAuthenticationService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorageService;
        private readonly IMapper _mapper;
        private readonly IHttpRequestUtility _httpRequestUtility;
        private IAuthState _authState;


        public AuthenticationService(HttpClient httpClient, ILocalStorageService localStorageService,
            IMapper mapper, IToastService toastService, IHttpRequestUtility httpRequestUtility, IAuthState authState) : base(toastService)
        {
            _httpClient = httpClient;
            _mapper = mapper;
            _localStorageService = localStorageService;
            _httpRequestUtility = httpRequestUtility;
            _authState = authState;
        }

        public async Task<string?> GetTokenAsync()
        {
            return await _localStorageService.GetItemAsStringAsync("AuthToken");
        }

        public async Task<bool> LoginAsync(LoginViewModel loginBindingViewModel)
        {
            var loginDto = _mapper.Map<LoginRequestDto>(loginBindingViewModel);

            HttpResponseMessage response;
            try
            {
                response = await _httpClient.PostAsJsonAsync("users/login", loginDto);
            }
            catch (System.Exception)
            {
                ShowErrorMessage("Unknown error occured");
                return false;
            }

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadFromJsonAsync<LoginResponseDto>()
                    ?? throw new ArgumentNullException("Error with auth response.");

                await _localStorageService.SetItemAsStringAsync("AuthToken", responseBody.AuthToken);
                await _localStorageService.SetItemAsStringAsync("RefreshToken", responseBody.RefreshToken);
                await SetCurrentUserNameAsync(responseBody.UserId);
                _authState.SetLoginState(true);

                return true;
            }
            else
            {
                await HandleHttpError(response);
            }

            return false;
        }

        public async Task LogoutAsync()
        {
            try
            {
                await _httpRequestUtility.ExecutePostHttpRequestAsync("users/logout");
            }
            catch (HttpRequestException) { }

            var keys = new List<string>() { "AuthToken", "RefreshToken", "UserName" };
            await _localStorageService.RemoveItemsAsync(keys);

            _authState.SetLoginState(false);
        }

        public async Task<bool> TryAutoLoginAsync()
        {
            if (!(await _localStorageService.ContainKeyAsync("RefreshToken")))
                return false;

            try
            {
                await _httpRequestUtility.RedeemTokenAsync();
            }
            catch (HttpRequestErrorException)
            {
                var keys = new List<string>() { "AuthToken", "RefreshToken", "UserName" };
                await _localStorageService.RemoveItemsAsync(keys);
                return false;
            }
            return true;
        }

        public async Task<string?> GetCurrentlyLoggedInUserAsync()
        {
            return await _localStorageService.GetItemAsStringAsync("UserName");
        }

        private async Task SetCurrentUserNameAsync(string currentUserId)
        {
            var response = await _httpRequestUtility.ExecuteGetHttpRequestAsync<UserResponseDto>($"users/{currentUserId}");

            var userName = response?.Response?.Name;

            if (!string.IsNullOrWhiteSpace(userName))
            {
                await _localStorageService.SetItemAsStringAsync("UserName", userName);
            }
            else
            {
                ShowErrorMessage("A felhasználónév nem található az API válaszban.");
            }
        }

    }
}
