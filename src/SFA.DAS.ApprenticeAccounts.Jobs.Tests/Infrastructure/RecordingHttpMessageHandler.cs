using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeAccounts.Jobs.Tests.Infrastructure
{
    /// <summary>
    /// Stands in for the network: records every request the Outer API client sends
    /// and returns a canned response, so tests can check exactly what goes over the wire.
    /// </summary>
    public class RecordingHttpMessageHandler : HttpMessageHandler
    {
        public List<RecordedRequest> Requests { get; } = new();

        public HttpStatusCode ResponseStatusCode { get; set; } = HttpStatusCode.OK;

        public string ResponseBody { get; set; } = string.Empty;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var body = request.Content == null
                ? null
                : await request.Content.ReadAsStringAsync(cancellationToken);

            Requests.Add(new RecordedRequest(request, body));

            return new HttpResponseMessage(ResponseStatusCode)
            {
                RequestMessage = request,
                Content = new StringContent(ResponseBody, System.Text.Encoding.UTF8, "application/json"),
            };
        }
    }

    public record RecordedRequest(HttpRequestMessage Request, string? Body);
}
