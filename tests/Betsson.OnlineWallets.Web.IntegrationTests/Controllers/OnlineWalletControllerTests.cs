using System.Net;
using System.Net.Http.Json;
using Betsson.OnlineWallets.Web.Models;

namespace Betsson.OnlineWallets.Web.IntegrationTests.Controllers;

public class OnlineWalletControllerTests : ControllerTestBase
{

    [Fact]
    public async Task Balance_ReturnsOkStatusCode()
    {
        var response = await Client.GetAsync("/onlinewallet/balance");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Balance_WhenNoTransactions_ReturnsZeroAmount()
    {
        var response = await Client.GetAsync("/onlinewallet/balance");

        var balance = await response.Content.ReadFromJsonAsync<BalanceResponse>();
        Assert.NotNull(balance);
        Assert.Equal(0, balance.Amount);
    }

    public static IEnumerable<object[]> DepositScenarios =>
    [
        [new[] { 100m }, 100m],
        [new[] { 100m, 50m }, 150m],
    ];

    [Theory]
    [MemberData(nameof(DepositScenarios))]
    public async Task Balance_AfterDeposits_ReturnsExpectedAmount(decimal[] amounts, decimal expectedBalance)
    {
        foreach (var amount in amounts)
            await Client.PostAsJsonAsync("/onlinewallet/deposit", new DepositRequest { Amount = amount });

        var response = await Client.GetAsync("/onlinewallet/balance");

        var balance = await response.Content.ReadFromJsonAsync<BalanceResponse>();
        Assert.NotNull(balance);
        Assert.Equal(expectedBalance, balance.Amount);
    }
}
