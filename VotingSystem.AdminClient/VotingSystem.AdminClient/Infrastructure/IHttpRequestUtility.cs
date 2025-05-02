﻿using VotingSystem.AdminClient.ViewModels;
using VotingSystem.Shared.Models;

namespace VotingSystem.AdminClient.Infrastructure
{
    public interface IHttpRequestUtility
    {
        Task<HttpResponseWrapper<T>> ExecuteGetHttpRequestAsync<T>(string uri);
        Task<TU?> ExecutePutHttpRequestAsync<T, TU>(string uri, T requestDto);
        Task<TU?> ExecutePostHttpRequestAsync<T, TU>(string uri, T requestDto);
        Task ExecutePostHttpRequestAsync(string uri);
        Task ExecuteDeleteHttpRequestAsync(string uri);
        Task<LoginResponseDto> RedeemTokenAsync(CancellationToken cancellationToken = default);
    }
}
