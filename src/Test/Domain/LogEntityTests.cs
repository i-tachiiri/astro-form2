namespace Domain.Tests;

using Domain.Entities;

public class LogEntityTests
{
    [Fact]
    public void AccessLog_StoresValues()
    {
        var id = Guid.NewGuid();
        var session = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var log = new AccessLog { Id = id, SessionId = session, AccessedAt = now };
        Assert.Equal(id, log.Id);
        Assert.Equal(session, log.SessionId);
        Assert.Equal(now, log.AccessedAt);
    }

    [Fact]
    public void ActionLog_StoresValues()
    {
        var log = new ActionLog
        {
            Id = Guid.NewGuid(),
            SessionId = Guid.NewGuid(),
            ActionName = "test",
            ActionedAt = DateTime.UtcNow
        };
        Assert.Equal("test", log.ActionName);
    }

    [Fact]
    public void SearchResultLog_StoresValues()
    {
        var log = new SearchResultLog
        {
            Id = Guid.NewGuid(),
            SessionId = Guid.NewGuid(),
            PlaceId = "place",
            Query = "query",
            Lat = 1.23m,
            Lng = 4.56m,
            SearchedAt = DateTime.UtcNow
        };
        Assert.Equal("place", log.PlaceId);
        Assert.Equal(1.23m, log.Lat);
    }
}
