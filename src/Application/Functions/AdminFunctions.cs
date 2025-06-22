using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Application.Functions;

public class AdminFunctions
{
    private readonly ILogger _logger;

    public AdminFunctions(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<AdminFunctions>();
    }

    [Function("AdminInitialize")]
    public HttpResponseData Initialize(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "admin/initialize")] HttpRequestData req)
    {
        var response = req.CreateResponse(HttpStatusCode.OK);
        return response;
    }

    [Function("AdminSeed")]
    public HttpResponseData Seed(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "admin/seed")] HttpRequestData req)
    {
        var response = req.CreateResponse(HttpStatusCode.OK);
        return response;
    }
}
