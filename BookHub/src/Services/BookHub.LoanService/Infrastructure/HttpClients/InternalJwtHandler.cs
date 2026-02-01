using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using BookHub.LoanService.Infrastructure.Security;

namespace BookHub.LoanService.Infrastructure.HttpClients;

public class InternalJwtHandler : DelegatingHandler
{
    private readonly InternalJwtTokenGenerator _tokenGenerator;

    public InternalJwtHandler(InternalJwtTokenGenerator tokenGenerator)
    {
        _tokenGenerator = tokenGenerator;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = _tokenGenerator.Generate();
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return base.SendAsync(request, cancellationToken);
    }
}
