using Fluxor;
using TwinsWins.BlazorClient.Services;

namespace TwinsWins.BlazorClient.State.User;

/// <summary>
/// User Effects - handle wallet connection and authentication
/// </summary>
public class UserEffects
{
    private readonly IWalletService _walletService;
    private readonly IAuthService _authService;

    public UserEffects(IWalletService walletService, IAuthService authService)
    {
        _walletService = walletService;
        _authService = authService;
    }

    [EffectMethod]
    public async Task HandleConnectWalletAction(ConnectWalletAction action, IDispatcher dispatcher)
    {
        try
        {
            var connected = await _walletService.ConnectWalletAsync();
            
            if (connected)
            {
                var walletAddress = await _walletService.GetConnectedWalletAsync();
                
                if (walletAddress != null)
                {
                    // TODO: Authenticate with backend
                    // var signature = await _walletService.SignMessageAsync("Auth message");
                    // await _authService.LoginAsync(walletAddress, signature);
                    
                    dispatcher.Dispatch(new ConnectWalletSuccessAction(walletAddress));
                }
                else
                {
                    dispatcher.Dispatch(new ConnectWalletFailureAction("Failed to get wallet address"));
                }
            }
            else
            {
                dispatcher.Dispatch(new ConnectWalletFailureAction("Wallet connection failed"));
            }
        }
        catch (Exception ex)
        {
            dispatcher.Dispatch(new ConnectWalletFailureAction(ex.Message));
        }
    }

    [EffectMethod]
    public async Task HandleDisconnectWalletAction(DisconnectWalletAction action, IDispatcher dispatcher)
    {
        await _walletService.DisconnectWalletAsync();
        await _authService.LogoutAsync();
    }
}
