using Fluxor;

namespace TwinsWins.BlazorClient.State.User;

/// <summary>
/// User State - holds authentication and user data
/// </summary>
[FeatureState]
public class UserState
{
    public string? WalletAddress { get; }
    public string? Username { get; }
    public bool IsAuthenticated { get; }
    public bool IsConnecting { get; }
    public string? ErrorMessage { get; }

    // Default constructor
    public UserState()
    {
        WalletAddress = null;
        Username = null;
        IsAuthenticated = false;
        IsConnecting = false;
        ErrorMessage = null;
    }

    // Constructor for updates
    public UserState(
        string? walletAddress,
        string? username,
        bool isAuthenticated,
        bool isConnecting,
        string? errorMessage)
    {
        WalletAddress = walletAddress;
        Username = username;
        IsAuthenticated = isAuthenticated;
        IsConnecting = isConnecting;
        ErrorMessage = errorMessage;
    }
}

// User Actions
public class ConnectWalletAction { }

public class ConnectWalletSuccessAction
{
    public string WalletAddress { get; }
    public ConnectWalletSuccessAction(string walletAddress) => WalletAddress = walletAddress;
}

public class ConnectWalletFailureAction
{
    public string ErrorMessage { get; }
    public ConnectWalletFailureAction(string errorMessage) => ErrorMessage = errorMessage;
}

public class DisconnectWalletAction { }

public class SetUsernameAction
{
    public string Username { get; }
    public SetUsernameAction(string username) => Username = username;
}

// User Reducers
public static class UserReducers
{
    [ReducerMethod]
    public static UserState ReduceConnectWalletAction(UserState state, ConnectWalletAction action)
    {
        return new UserState(
            walletAddress: null,
            username: state.Username,
            isAuthenticated: false,
            isConnecting: true,
            errorMessage: null
        );
    }

    [ReducerMethod]
    public static UserState ReduceConnectWalletSuccessAction(UserState state, ConnectWalletSuccessAction action)
    {
        return new UserState(
            walletAddress: action.WalletAddress,
            username: state.Username,
            isAuthenticated: true,
            isConnecting: false,
            errorMessage: null
        );
    }

    [ReducerMethod]
    public static UserState ReduceConnectWalletFailureAction(UserState state, ConnectWalletFailureAction action)
    {
        return new UserState(
            walletAddress: null,
            username: state.Username,
            isAuthenticated: false,
            isConnecting: false,
            errorMessage: action.ErrorMessage
        );
    }

    [ReducerMethod]
    public static UserState ReduceDisconnectWalletAction(UserState state, DisconnectWalletAction action)
    {
        return new UserState(); // Reset to initial state
    }

    [ReducerMethod]
    public static UserState ReduceSetUsernameAction(UserState state, SetUsernameAction action)
    {
        return new UserState(
            walletAddress: state.WalletAddress,
            username: action.Username,
            isAuthenticated: state.IsAuthenticated,
            isConnecting: state.IsConnecting,
            errorMessage: state.ErrorMessage
        );
    }
}
