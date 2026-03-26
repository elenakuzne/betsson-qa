using Betsson.OnlineWallets.Data.Models;
using Betsson.OnlineWallets.Data.Repositories;
using Betsson.OnlineWallets.Models;
using Betsson.OnlineWallets.Services;
using Moq;

namespace Betsson.OnlineWallets.UnitTests.Services
{
    public class OnlineWalletServiceTests
    {
        private readonly Mock<IOnlineWalletRepository> _repositoryMock;
        private readonly OnlineWalletService _service;

        public OnlineWalletServiceTests()
        {
            _repositoryMock = new Mock<IOnlineWalletRepository>();
            _service = new OnlineWalletService(_repositoryMock.Object);
        }

        [Fact]
        public async Task GetBalanceAsync_WhenNoTransactionsExist_ReturnsZeroBalance()
        {
            _repositoryMock
                .Setup(r => r.GetLastOnlineWalletEntryAsync())
                .ReturnsAsync((OnlineWalletEntry?)null);

            Balance result = await _service.GetBalanceAsync();

            Assert.Equal(0, result.Amount);
        }

        [Theory]
        [InlineData(100, 50, 150)]
        [InlineData(200, -75, 125)]
        [InlineData(0, 300, 300)]
        public async Task GetBalanceAsync_WhenLastEntryExists_ReturnsBalanceBeforePlusAmount(
            decimal balanceBefore, decimal amount, decimal expectedBalance)
        {
            _repositoryMock
                .Setup(r => r.GetLastOnlineWalletEntryAsync())
                .ReturnsAsync(new OnlineWalletEntry { BalanceBefore = balanceBefore, Amount = amount });

            Balance result = await _service.GetBalanceAsync();

            Assert.Equal(expectedBalance, result.Amount);
        }
    }
}
