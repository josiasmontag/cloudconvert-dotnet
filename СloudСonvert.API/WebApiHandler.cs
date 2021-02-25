using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using СloudСonvert.API.Extensions;

namespace СloudСonvert.API
{
  internal sealed class WebApiHandler : HttpClientHandler
  {

    private readonly bool _loggingEnabled;

    public WebApiHandler(bool loggingEnabled)
    {
      _loggingEnabled = loggingEnabled;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {

      bool writeLog = _loggingEnabled;

      try
      {
        if (writeLog)
        {
          var requestString = await request?.Content?.ReadAsStringAsync();
        }

        var response = await base.SendAsync(request, cancellationToken);

        if (writeLog)
        {
          string responseString = (await response.Content.ReadAsStringAsync()).TrimLengthWithEllipsis(20000);
        }

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
          || response.StatusCode == System.Net.HttpStatusCode.BadRequest)
        {
          throw new WebApiException((await response.Content.ReadAsStringAsync()).TrimLengthWithEllipsis(20000));
        }

        return response;
      }
      catch (Exception exc)
      {
        throw new WebApiException($"{request.Method} {request.RequestUri} failed: {exc.Message}", exc);
      }
    }
  }
}
