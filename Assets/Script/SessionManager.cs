using UnityEngine;
using System;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Multiplayer;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using UnityUtils;

public class SessionManager : Singleton<SessionManager>
{
    #region SessionManager
    //ISession activeSession;
    //ISession ActiveSession
    //{
    //    get => activeSession;
    //    set
    //    {
    //        activeSession = value;
    //        Debug.Log($"Active Session: {activeSession}");
    //    }
    //}

    //const string playerNamePropertyKey = "playerName";

    //async void Start()
    //{
    //    try
    //    {
    //        await UnityServices.InitializeAsync();
    //        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    //        Debug.Log($"Sign in anonymously succeeded! Player ID: {AuthenticationService.Instance.PlayerId}");

    //        StartSessionHost();
    //    }
    //    catch (Exception e)
    //    {
    //        Debug.LogException(e);
    //    }
    //}

    //async UniTask<Dictionary<string, PlayerProperty>> GetPlayerProperties()
    //{
    //    var playerName = await AuthenticationService.Instance.GetPlayerNameAsync();
    //    var playerNameProperty = new PlayerProperty(playerName, VisibilityPropertyOptions.Member);
    //    return new Dictionary<string, PlayerProperty> { { playerNamePropertyKey, playerNameProperty } };
    //}

    //async void StartSessionHost()
    //{
    //    var playerProperties = await GetPlayerProperties();

    //    var options = new SessionOptions
    //    {
    //        MaxPlayers = 2,
    //        IsLocked = false,
    //        IsPrivate = false,
    //    }.WithRelayNetwork();

    //    activeSession = await MultiplayerService.Instance.CreateSessionAsync(options);
    //    Debug.Log($"Session  {ActiveSession.Id} created! Join Code: {ActiveSession.Code} ");
    //}
    //async UniTaskVoid JoinSessionById(string sessionId)
    //{
    //    ActiveSession = await MultiplayerService.Instance.JoinSessionByIdAsync(sessionId);
    //    Debug.Log($"Session {ActiveSession.Id} joined!");
    //}
    //async UniTaskVoid JoinSessionByCode(string sessionCode)
    //{
    //    ActiveSession = await MultiplayerService.Instance.JoinSessionByCodeAsync(sessionCode);
    //    Debug.Log($"Session {ActiveSession.Id} joined!");
    //}

    //async UniTaskVoid KickPlayer(string playerId)
    //{
    //    if (!ActiveSession.IsHost) return;
    //    await ActiveSession.AsHost().RemovePlayerAsync(playerId);
    //}
    //async UniTask<IList<ISessionInfo>> QuerySessions()
    //{
    //    var sessionQueryOptions = new QuerySessionsOptions();
    //    QuerySessionsResults results = await MultiplayerService.Instance.QuerySessionsAsync(sessionQueryOptions);
    //    return results.Sessions;
    //}

    //async UniTaskVoid LeaveSession()
    //{
    //    if (ActiveSession != null)
    //    {
    //        try
    //        {
    //            await ActiveSession.LeaveAsync();
    //        }
    //        catch
    //        {

    //        }
    //        finally
    //        {
    //            activeSession = null;
    //        }
    //    }
    //}
    #endregion
    #region Session Manager with Distributed Authority
    ISession activeSession;
    ISession ActiveSession
    {
        get => activeSession;
        set
        {
            activeSession = value;
            Debug.Log($"Active Session: {activeSession}");
        }
    }

    NetworkManager networkManager;

    string sessionName = "SessionKoIto";

    const string playerNamePropertyKey = "playerName";

    void OnSessionOwnerPromoted(ulong sessionOwnerPromoted)
    {
        if (networkManager.LocalClient.IsSessionOwner)
        {
            Debug.Log($"Client-{networkManager.LocalClientId} is the Session Owner!");
        }
    }

    void OnClientConnectedCallback(ulong clientId)
    {
        if (networkManager.LocalClientId == clientId)
        {
            Debug.Log($"Client-{clientId} is connected and can Spawn {nameof(NetworkObject)}s");
        }
    }

    async void Start()
    {
        try
        {
            networkManager = GetComponent<NetworkManager>();
            networkManager.OnClientConnectedCallback += OnClientConnectedCallback;
            networkManager.OnSessionOwnerPromoted += OnSessionOwnerPromoted;

            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log($"Sign in anonymously succeeded! Player ID: {AuthenticationService.Instance.PlayerId}");

            var option = new SessionOptions
            {
                Name = sessionName,
                MaxPlayers = 4
            }.WithDistributedAuthorityNetwork();

            ActiveSession = await MultiplayerService.Instance.CreateOrJoinSessionAsync(sessionName, option);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    async UniTask<Dictionary<string, PlayerProperty>> GetPlayerProperties()
    {
        var playerName = await AuthenticationService.Instance.GetPlayerNameAsync();
        var playerNameProperty = new PlayerProperty(playerName, VisibilityPropertyOptions.Member);
        return new Dictionary<string, PlayerProperty> { { playerNamePropertyKey, playerNameProperty } };
    }

    //async void StartSessionHost()
    //{
    //    var playerProperties = await GetPlayerProperties();

    //    var options = new SessionOptions
    //    {
    //        MaxPlayers = 2,
    //        IsLocked = false,
    //        IsPrivate = false,
    //    }.WithRelayNetwork();

    //    activeSession = await MultiplayerService.Instance.CreateSessionAsync(options);
    //    Debug.Log($"Session  {ActiveSession.Id} created! Join Code: {ActiveSession.Code} ");
    //}
    async UniTaskVoid JoinSessionById(string sessionId)
    {
        ActiveSession = await MultiplayerService.Instance.JoinSessionByIdAsync(sessionId);
        Debug.Log($"Session {ActiveSession.Id} joined!");
    }
    async UniTaskVoid JoinSessionByCode(string sessionCode)
    {
        ActiveSession = await MultiplayerService.Instance.JoinSessionByCodeAsync(sessionCode);
        Debug.Log($"Session {ActiveSession.Id} joined!");
    }

    async UniTaskVoid KickPlayer(string playerId)
    {
        if (!ActiveSession.IsHost) return;
        await ActiveSession.AsHost().RemovePlayerAsync(playerId);
    }
    async UniTask<IList<ISessionInfo>> QuerySessions()
    {
        var sessionQueryOptions = new QuerySessionsOptions();
        QuerySessionsResults results = await MultiplayerService.Instance.QuerySessionsAsync(sessionQueryOptions);
        return results.Sessions;
    }

    async UniTaskVoid LeaveSession()
    {
        if (ActiveSession != null)
        {
            try
            {
                await ActiveSession.LeaveAsync();
            }
            catch
            {

            }
            finally
            {
                activeSession = null;
            }
        }
    }
    #endregion
}