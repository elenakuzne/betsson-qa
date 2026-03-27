namespace Betsson.OnlineWallets.Web.IntegrationTests;

public abstract class ControllerTestBase : IAsyncDisposable
{
    private readonly CustomWebApplicationFactory _factory;
    protected readonly HttpClient Client;

    protected ControllerTestBase()
    {
        _factory = new CustomWebApplicationFactory();
        _factory.ResetDatabase();
        Client = _factory.CreateClient();
    }

    public async ValueTask DisposeAsync()
    {
        await _factory.DisposeAsync();
    }
}
