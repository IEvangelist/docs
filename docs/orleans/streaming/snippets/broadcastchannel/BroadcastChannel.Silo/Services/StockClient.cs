﻿using System.Net.Http.Json;
using BroadcastChannel.Grains;
using BroadcastChannel.Silo.Options;
using Microsoft.Extensions.Options;

namespace BroadcastChannel.Silo.Services;

public sealed class StockClient : IDisposable
{
    private readonly HttpClient _client;
    private readonly AlphaVantageOptions _options;

    public StockClient(HttpClient client, IOptions<AlphaVantageOptions> options) =>
        (_client, _options) = (client, options.Value);
    
    public Task<Stock> GetStockAsync(string symbol) =>
        _client.GetFromJsonAsync<Stock>(
            $"query?function=GLOBAL_QUOTE&symbol={symbol}&apikey={_options.ApiKey}")!;

    void IDisposable.Dispose() => _client.Dispose();
}
