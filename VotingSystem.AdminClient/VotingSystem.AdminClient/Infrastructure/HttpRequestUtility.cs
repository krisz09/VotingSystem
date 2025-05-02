﻿using Blazored.LocalStorage;
using VotingSystem.Shared.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using VotingSystem.AdminClient.ViewModels;

namespace VotingSystem.AdminClient.Infrastructure
{
    public class HttpRequestUtility : IHttpRequestUtility
    {
        private readonly ILocalStorageService _localStorageService;
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public HttpRequestUtility(ILocalStorageService localStorageService, HttpClient httpClient, JsonSerializerOptions jsonOptions)
        {
            _localStorageService = localStorageService;
            _httpClient = httpClient;
            _jsonOptions = jsonOptions;
        }

        public async Task<HttpResponseWrapper<T>> ExecuteGetHttpRequestAsync<T>(string uri)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            var response = await SendRequestAsync(request);

            var responseObject = await HandleResponseObjectAsync<T>(response);
            return new HttpResponseWrapper<T>(responseObject, response.Headers);
        }

        public async Task<TU?> ExecutePostHttpRequestAsync<T, TU>(string uri, T requestDto)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = CreateRequestBody(requestDto)
            };
            var response = await SendRequestAsync(request);
            return await HandleResponseObjectAsync<TU>(response);
        }

        public async Task ExecutePostHttpRequestAsync(string uri)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, uri);
            var response = await SendRequestAsync(request);
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestErrorException(response);
        }

        public async Task<TU?> ExecutePutHttpRequestAsync<T, TU>(string uri, T requestDto)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, uri)
            {
                Content = CreateRequestBody(requestDto)
            };
            var response = await SendRequestAsync(request);
            return await HandleResponseObjectAsync<TU>(response);
        }

        public async Task ExecuteDeleteHttpRequestAsync(string uri)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, uri);
            var response = await SendRequestAsync(request);
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestErrorException(response);
        }

        private async Task<T> HandleResponseObjectAsync<T>(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var responseObject = JsonSerializer.Deserialize<T>(content, _jsonOptions) ?? throw new HttpRequestException();
                return responseObject;

            }
            else
            {
                throw new HttpRequestErrorException(response);
            }
        }

        private async Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
        {
            var authToken = await _localStorageService.GetItemAsStringAsync("AuthToken", cancellationToken);
            if (!string.IsNullOrEmpty(authToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }

            var response = await _httpClient.SendAsync(request, cancellationToken);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                var loginResponseDto = await RedeemTokenAsync(cancellationToken);

                if (!string.IsNullOrEmpty(loginResponseDto.AuthToken))
                {
                    var newRequest = CloneRequest(request);
                    newRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", loginResponseDto.AuthToken);

                    response.Dispose();
                    response = await _httpClient.SendAsync(newRequest, cancellationToken);
                }
            }

            return response;
        }

        private HttpRequestMessage CloneRequest(HttpRequestMessage request)
        {
            var newRequest = new HttpRequestMessage(request.Method, request.RequestUri);

            foreach (var header in request.Headers)
            {
                newRequest.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            if (request.Content != null)
            {
                var content = request.Content.ReadAsByteArrayAsync().Result;
                newRequest.Content = new ByteArrayContent(content);

                foreach (var header in request.Content.Headers)
                {
                    newRequest.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }

            return newRequest;
        }


        public async Task<LoginResponseDto> RedeemTokenAsync(CancellationToken cancellationToken = default)
        {
            var refreshToken = await _localStorageService.GetItemAsStringAsync("RefreshToken", cancellationToken);
            if (string.IsNullOrEmpty(refreshToken))
                throw new ArgumentException(nameof(refreshToken));

            var content = new StringContent(JsonSerializer.Serialize(refreshToken, _jsonOptions), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("users/refresh", content, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync(cancellationToken);
                var loginResponseDto = JsonSerializer.Deserialize<LoginResponseDto>(responseData, _jsonOptions) ?? throw new HttpRequestException();

                if (!string.IsNullOrEmpty(loginResponseDto.RefreshToken))
                {
                    await _localStorageService.SetItemAsStringAsync("RefreshToken", loginResponseDto.RefreshToken, cancellationToken);
                }

                if (!string.IsNullOrEmpty(loginResponseDto.AuthToken))
                {
                    await _localStorageService.SetItemAsStringAsync("AuthToken", loginResponseDto.AuthToken, cancellationToken);
                }

                return loginResponseDto;
            }

            throw new HttpRequestErrorException(response);
        }

        private HttpContent CreateRequestBody<T>(T requestDto)
        {
            return new StringContent(JsonSerializer.Serialize(requestDto, _jsonOptions), Encoding.UTF8, "application/json");
        }
    }
}
