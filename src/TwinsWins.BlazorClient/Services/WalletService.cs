using Blazored.LocalStorage;

namespace TwinsWins.BlazorClient.Services;

public class WalletService : IWalletService
{
    private readonly ILocalStorageService _localStorage;
    private string? _connectedWallet;

    public event Action<string?>? OnWalletChanged;

    public WalletService(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public async Task<bool> ConnectWalletAsync()
    {
        // TODO: Implement actual TON wallet connection
        // 1. Check for TonConnect SDK
        // 2. Request wallet connection
        // 3. Get wallet address
        // 4. Store in local storage
        
        // Stub implementation for testing
        _connectedWallet = "EQTest_" + Guid.NewGuid().ToString()[..8];
        await _localStorage.SetItemAsync("wallet_address", _connectedWallet);
        OnWalletChanged?.Invoke(_connectedWallet);
        return true;
    }

    public async Task DisconnectWalletAsync()
    {
        _connectedWallet = null;
        await _localStorage.RemoveItemAsync("wallet_address");
        OnWalletChanged?.Invoke(null);
    }

    public async Task<string?> GetConnectedWalletAsync()
    {
        if (_connectedWallet != null)
            return _connectedWallet;

        _connectedWallet = await _localStorage.GetItemAsync<string>("wallet_address");
        return _connectedWallet;
    }

    public async Task<string> SignMessageAsync(string message)
    {
        // TODO: Implement actual message signing with TON wallet
        // 1. Request signature from connected wallet
        // 2. Return signature
        
        // Stub implementation
        await Task.Delay(100); // Simulate async operation
        return "stub_signature_" + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(message));
    }
}
