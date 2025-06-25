using System;
using Domain.Models;
using System.Text.Json;
using Xunit;

public class DomainModelTests
{
    [Fact]
    public void AccessLog_StoresValues()
    {
        var now = DateTime.UtcNow;
        var log = new AccessLog("id", "sid", now);
        Assert.Equal("id", log.Id);
        Assert.Equal("sid", log.SessionId);
        Assert.Equal(now, log.AccessedAt);
    }

    [Fact]
    public void AccessLog_Serializes_With_SnakeCase()
    {
        var log = new AccessLog("1", "sid", DateTime.UnixEpoch);
        var json = JsonSerializer.Serialize(log);
        Assert.Contains("\"id\":\"1\"", json);
        Assert.Contains("\"session_id\":\"sid\"", json);
        Assert.Contains("\"accessed_at\":\"1970-01-01T00:00:00Z\"", json);
    }

    [Fact]
    public void ActionLog_StoresValues()
    {
        var now = DateTime.UtcNow;
        var log = new ActionLog("id", "sid", "act", now);
        Assert.Equal("id", log.Id);
        Assert.Equal("sid", log.SessionId);
        Assert.Equal("act", log.ActionName);
        Assert.Equal(now, log.ActionedAt);
    }

    [Fact]
    public void ActionLog_Serializes_With_SnakeCase()
    {
        var log = new ActionLog("1", "sid", "act", DateTime.UnixEpoch);
        var json = JsonSerializer.Serialize(log);
        Assert.Contains("\"id\":\"1\"", json);
        Assert.Contains("\"session_id\":\"sid\"", json);
        Assert.Contains("\"action_name\":\"act\"", json);
        Assert.Contains("\"actioned_at\":\"1970-01-01T00:00:00Z\"", json);
    }

    [Fact]
    public void SearchResultLog_StoresValues()
    {
        var now = DateTime.UtcNow;
        var log = new SearchResultLog("id", "sid", "pid", "query", 1.2m, 3.4m, now);
        Assert.Equal("id", log.Id);
        Assert.Equal("sid", log.SessionId);
        Assert.Equal("pid", log.PlaceId);
        Assert.Equal("query", log.Query);
        Assert.Equal(1.2m, log.Lat);
        Assert.Equal(3.4m, log.Lng);
        Assert.Equal(now, log.SearchedAt);
    }

    [Fact]
    public void SearchResultLog_Serializes_With_SnakeCase()
    {
        var log = new SearchResultLog("1", "sid", "pid", "q", 1.2m, 3.4m, DateTime.UnixEpoch);
        var json = JsonSerializer.Serialize(log);
        Assert.Contains("\"id\":\"1\"", json);
        Assert.Contains("\"session_id\":\"sid\"", json);
        Assert.Contains("\"place_id\":\"pid\"", json);
        Assert.Contains("\"query\":\"q\"", json);
        Assert.Contains("\"lat\":1.2", json);
        Assert.Contains("\"lng\":3.4", json);
        Assert.Contains("\"searched_at\":\"1970-01-01T00:00:00Z\"", json);
    }
}
