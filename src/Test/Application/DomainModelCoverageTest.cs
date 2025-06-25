using System;
using Domain.Models;
using System.Text.Json;
using Xunit;

public class DomainModelCoverageTest
{
    [Fact]
    public void AccessLog_Serialization_Coverage()
    {
        var log = new AccessLog("1", "sid", DateTime.UnixEpoch);
        var json = JsonSerializer.Serialize(log);
        Assert.Contains("\"session_id\"", json);
    }

    [Fact]
    public void ActionLog_Serialization_Coverage()
    {
        var log = new ActionLog("1", "sid", "act", DateTime.UnixEpoch);
        var json = JsonSerializer.Serialize(log);
        Assert.Contains("\"action_name\"", json);
    }

    [Fact]
    public void SearchResultLog_Serialization_Coverage()
    {
        var log = new SearchResultLog("1", "sid", "pid", "q", 1.2m, 3.4m, DateTime.UnixEpoch);
        var json = JsonSerializer.Serialize(log);
        Assert.Contains("\"place_id\"", json);
    }
}
