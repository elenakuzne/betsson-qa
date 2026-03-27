using System.Net;
using System.Net.Http.Json;
using Betsson.OnlineWallets.Web.Models;

namespace Betsson.OnlineWallets.Web.IntegrationTests.Controllers;

public class OnlineWalletControllerTests : ControllerTestBase
{
    private const string BalanceUrl = "/onlinewallet/balance";
    private const string DepositUrl = "/onlinewallet/deposit";
    private const string WithdrawUrl = "/onlinewallet/withdraw";

    [Fact]
    public async Task Balance_ReturnsOkStatusCode()
    {
        var response = await Client.GetAsync(BalanceUrl);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Balance_WhenNoTransactions_ReturnsZeroAmount()
    {
        var response = await Client.GetAsync(BalanceUrl);

        var balance = await response.Content.ReadFromJsonAsync<BalanceResponse>();
        Assert.NotNull(balance);
        Assert.Equal(0, balance.Amount);
    }

    [Theory]
    [InlineData(-0.01)]
    [InlineData(-10)]
    public async Task Deposit_WithNegativeAmount_ReturnsBadRequest(decimal amount)
    {
        var response = await Client.PostAsJsonAsync(DepositUrl, new DepositRequest { Amount = amount });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    public static IEnumerable<object[]> DepositScenarios =>
    [
        [new[] { 0m }, 0m],
        [new[] { 100m }, 100m],
        [new[] { 0.01m, 0.02m }, 0.03m],
        [new[] { 100m, 50m }, 150m],
        [new[] { 200m, 50m }, 250m],
        [new[] { 100m, 200m, 300m, 400m, 500m }, 1500m],
    ];

    [Theory]
    [MemberData(nameof(DepositScenarios))]
    public async Task Deposit_WithValidAmount_ReturnsOkWithUpdatedBalance(decimal[] amounts, decimal expectedBalance)
    {
        HttpResponseMessage? depositResponse = null;
        foreach (var amount in amounts)
            depositResponse = await Client.PostAsJsonAsync(DepositUrl, new DepositRequest { Amount = amount });

        Assert.Equal(HttpStatusCode.OK, depositResponse!.StatusCode);
        var depositBalance = await depositResponse.Content.ReadFromJsonAsync<BalanceResponse>();
        Assert.NotNull(depositBalance);
        Assert.Equal(expectedBalance, depositBalance.Amount);

        var balanceResponse = await Client.GetAsync(BalanceUrl);
        var balance = await balanceResponse.Content.ReadFromJsonAsync<BalanceResponse>();
        Assert.NotNull(balance);
        Assert.Equal(expectedBalance, balance.Amount);
    }
}
