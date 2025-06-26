using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Application.Services;
using Domain.Models;
using Domain.Repositories;
using Microsoft.Extensions.Logging.Abstractions;
using Shared;
using System.Linq;
using System.Text.Json;
using Xunit;

class FakeLogRepository : ILogRepository
{
    public List<ActionLog> ActionLogs { get; } = new();
    public List<SearchResultLog> SearchLogs { get; } = new();
    public Task AddAccessLogAsync(AccessLog log) => Task.CompletedTask;
    public Task AddActionLogAsync(ActionLog log) { ActionLogs.Add(log); return Task.CompletedTask; }
    public Task AddSearchResultLogAsync(SearchResultLog log) { SearchLogs.Add(log); return Task.CompletedTask; }
}

class FakeHttpHandler : HttpMessageHandler
{
    private readonly Func<HttpRequestMessage, HttpResponseMessage> _handler;
    public FakeHttpHandler(Func<HttpRequestMessage, HttpResponseMessage> handler) => _handler = handler;
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) => Task.FromResult(_handler(request));
}

public class BirthplaceSearchServiceTest
{
    [Fact]
    public async Task SearchAsync_LogsAction()
    {
        var repo = new FakeLogRepository();
        var handler = new FakeHttpHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{\"predictions\":[{\"place_id\":\"1\",\"structured_formatting\":{\"main_text\":\"Tokyo\"},\"description\":\"Tokyo\"}]}")
        });
        var client = new HttpClient(handler);
        var service = new BirthplaceSearchService(client, repo, NullLogger<BirthplaceSearchService>.Instance);
        var results = await service.SearchAsync("tokyo", "sid");
        Assert.Single(repo.ActionLogs);
        Assert.Single(results.Results);
    }

    [Fact]
    public async Task GetPlaceDetailsAsync_LogsSearchResult()
    {
        var json = "{\"status\":\"OK\",\"result\":{\"place_id\":\"1\",\"name\":\"Tokyo\",\"formatted_address\":\"Tokyo\",\"geometry\":{\"location\":{\"lat\":1,\"lng\":2}},\"url\":\"u\"}}";
        var repo = new FakeLogRepository();
        var handler = new FakeHttpHandler(_ => new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(json) });
        var client = new HttpClient(handler);
        var service = new BirthplaceSearchService(client, repo, NullLogger<BirthplaceSearchService>.Instance);
        var detail = await service.GetPlaceDetailsAsync("1", "tokyo", "sid");
        Assert.Single(repo.SearchLogs);
        Assert.Equal("1", detail.PlaceId);
    }

    [Fact]
    public async Task SearchAsync_Throws_On_EmptyQuery()
    {
        var repo = new FakeLogRepository();
        var client = new HttpClient(new FakeHttpHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)));
        var service = new BirthplaceSearchService(client, repo, NullLogger<BirthplaceSearchService>.Instance);
        await Assert.ThrowsAsync<ArgumentException>(() => service.SearchAsync("", "sid"));
    }

    [Fact]
    public async Task SearchAsync_NoLog_When_SessionIdEmpty()
    {
        var handler = new FakeHttpHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{\"predictions\":[]}")
        });
        var repo = new FakeLogRepository();
        var client = new HttpClient(handler);
        var service = new BirthplaceSearchService(client, repo, NullLogger<BirthplaceSearchService>.Instance);
        await service.SearchAsync("tokyo", string.Empty);
        Assert.Empty(repo.ActionLogs);
    }

    [Fact]
    public async Task GetPlaceDetailsAsync_Throws_When_NotFound()
    {
        var json = "{\"status\":\"NOT_FOUND\"}";
        var handler = new FakeHttpHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json)
        });
        var repo = new FakeLogRepository();
        var client = new HttpClient(handler);
        var service = new BirthplaceSearchService(client, repo, NullLogger<BirthplaceSearchService>.Instance);
        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.GetPlaceDetailsAsync("1", "tokyo", "sid"));
    }

    [Fact]
    public async Task GetPlaceDetailsAsync_Throws_When_ErrorStatus()
    {
        var json = "{\"status\":\"ERROR\"}";
        var handler = new FakeHttpHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json)
        });
        var repo = new FakeLogRepository();
        var client = new HttpClient(handler);
        var service = new BirthplaceSearchService(client, repo, NullLogger<BirthplaceSearchService>.Instance);
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.GetPlaceDetailsAsync("1", "tokyo", "sid"));
    }

    [Fact]
    public async Task SearchAsync_Uses_Japanese_Language()
    {
        string? requestedUrl = null;
        var handler = new FakeHttpHandler(req =>
        {
            requestedUrl = req.RequestUri!.ToString();
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"predictions\":[]}")
            };
        });
        var repo = new FakeLogRepository();
        var client = new HttpClient(handler);
        var service = new BirthplaceSearchService(client, repo, NullLogger<BirthplaceSearchService>.Instance);
        await service.SearchAsync("tokyo", string.Empty);
        Assert.Contains("language=ja", requestedUrl);
    }

    [Fact]
    public async Task GetPlaceDetailsAsync_Uses_Japanese_Language()
    {
        string? requestedUrl = null;
        var json = "{\"status\":\"OK\",\"result\":{\"place_id\":\"1\",\"name\":\"Tokyo\",\"formatted_address\":\"Tokyo\",\"geometry\":{\"location\":{\"lat\":1,\"lng\":2}},\"url\":\"u\"}}";
        var handler = new FakeHttpHandler(req =>
        {
            requestedUrl = req.RequestUri!.ToString();
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json)
            };
        });
        var repo = new FakeLogRepository();
        var client = new HttpClient(handler);
        var service = new BirthplaceSearchService(client, repo, NullLogger<BirthplaceSearchService>.Instance);
        await service.GetPlaceDetailsAsync("1", "tokyo", string.Empty);
        Assert.Contains("language=ja", requestedUrl);
    }

    [Fact]
    public async Task SearchAsync_Returns_Up_To_Five_Results()
    {
        var predictions = Enumerable.Range(1, 6).Select(i => new
        {
            place_id = i.ToString(),
            structured_formatting = new { main_text = $"N{i}" },
            description = $"D{i}"
        });
        var json = JsonSerializer.Serialize(new { predictions });
        var handler = new FakeHttpHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json)
        });
        var repo = new FakeLogRepository();
        var client = new HttpClient(handler);
        var service = new BirthplaceSearchService(client, repo, NullLogger<BirthplaceSearchService>.Instance);
        var result = await service.SearchAsync("tokyo", string.Empty);
        Assert.Equal(5, result.Results.Count);
    }
}
