using TwinsWins.Core.Entities;

namespace TwinsWins.Core.Interfaces;

public interface IBlockchainService
{
    Task<string> CreateGameContractAsync(Guid gameId, decimal stakeAmount);
    Task<bool> ProcessPayoutAsync(Guid gameId, string winnerWallet, decimal amount);
    Task<bool> ProcessRefundAsync(Guid gameId, string walletAddress, decimal amount);
    Task<Transaction> VerifyTransactionAsync(string transactionHash);
    Task<decimal> GetWalletBalanceAsync(string walletAddress);
}
