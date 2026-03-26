using Betsson.OnlineWallets.Data.Models;
using Betsson.OnlineWallets.Data.Repositories;
using Betsson.OnlineWallets.Exceptions;
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

        #region GetBalanceAsync

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

        #endregion

        #region DepositFundsAsync

        [Theory]
        [InlineData(0, 100, 100)]
        [InlineData(50, 25, 75)]
        [InlineData(0, 0, 0)]
        [InlineData(50, 0, 50)]
        public async Task DepositFundsAsync_ReturnsCurrentBalancePlusDepositAmount(
            decimal currentBalance, decimal depositAmount, decimal expectedBalance)
        {
            _repositoryMock
                .Setup(r => r.GetLastOnlineWalletEntryAsync())
                .ReturnsAsync(new OnlineWalletEntry { BalanceBefore = 0, Amount = currentBalance });
            _repositoryMock
                .Setup(r => r.InsertOnlineWalletEntryAsync(It.IsAny<OnlineWalletEntry>()))
                .Returns(Task.CompletedTask);

            Balance result = await _service.DepositFundsAsync(new Deposit { Amount = depositAmount });

            Assert.Equal(expectedBalance, result.Amount);
        }

        [Fact]
        public async Task DepositFundsAsync_InsertsEntryWithCorrectAmountAndBalanceBefore()
        {
            _repositoryMock
                .Setup(r => r.GetLastOnlineWalletEntryAsync())
                .ReturnsAsync(new OnlineWalletEntry { BalanceBefore = 0, Amount = 100m });
            _repositoryMock
                .Setup(r => r.InsertOnlineWalletEntryAsync(It.IsAny<OnlineWalletEntry>()))
                .Returns(Task.CompletedTask);

            await _service.DepositFundsAsync(new Deposit { Amount = 50m });

            _repositoryMock.Verify(r => r.InsertOnlineWalletEntryAsync(It.Is<OnlineWalletEntry>(e =>
                e.Amount == 50m &&
                e.BalanceBefore == 100m
            )), Times.Once);
        }

        #endregion

        #region WithdrawFundsAsync

        [Theory]
        [InlineData(100, 40, 60)]
        [InlineData(100, 100, 0)]
        [InlineData(100, 0, 100)]
        public async Task WithdrawFundsAsync_ReturnsCurrentBalanceMinusWithdrawalAmount(
            decimal currentBalance, decimal withdrawalAmount, decimal expectedBalance)
        {
            _repositoryMock
                .Setup(r => r.GetLastOnlineWalletEntryAsync())
                .ReturnsAsync(new OnlineWalletEntry { BalanceBefore = 0, Amount = currentBalance });
            _repositoryMock
                .Setup(r => r.InsertOnlineWalletEntryAsync(It.IsAny<OnlineWalletEntry>()))
                .Returns(Task.CompletedTask);

            Balance result = await _service.WithdrawFundsAsync(new Withdrawal { Amount = withdrawalAmount });

            Assert.Equal(expectedBalance, result.Amount);
        }

        [Fact]
        public async Task WithdrawFundsAsync_InsertsEntryWithNegativeAmountAndBalanceBefore()
        {
            _repositoryMock
                .Setup(r => r.GetLastOnlineWalletEntryAsync())
                .ReturnsAsync(new OnlineWalletEntry { BalanceBefore = 0, Amount = 100m });
            _repositoryMock
                .Setup(r => r.InsertOnlineWalletEntryAsync(It.IsAny<OnlineWalletEntry>()))
                .Returns(Task.CompletedTask);

            await _service.WithdrawFundsAsync(new Withdrawal { Amount = 40m });

            _repositoryMock.Verify(r => r.InsertOnlineWalletEntryAsync(It.Is<OnlineWalletEntry>(e =>
                e.Amount == -40m &&
                e.BalanceBefore == 100m
            )), Times.Once);
        }

        [Fact]
        public async Task WithdrawFundsAsync_WhenAmountExceedsBalance_DoesNotInsertEntry()
        {
            _repositoryMock
                .Setup(r => r.GetLastOnlineWalletEntryAsync())
                .ReturnsAsync(new OnlineWalletEntry { BalanceBefore = 0, Amount = 50m });

            await Assert.ThrowsAsync<InsufficientBalanceException>(() =>
                _service.WithdrawFundsAsync(new Withdrawal { Amount = 51m }));

            _repositoryMock.Verify(r => r.InsertOnlineWalletEntryAsync(It.IsAny<OnlineWalletEntry>()), Times.Never);
        }

        #endregion
    }
}
