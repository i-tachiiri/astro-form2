using System;
using Domain.Models;
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
}
