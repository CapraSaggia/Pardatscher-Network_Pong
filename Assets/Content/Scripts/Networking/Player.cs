using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SyncVar(hook = nameof(AuthorityHandlePartyOwnerStateUpdated))]
    private bool isPartyOwner = false;

    [SyncVar(hook = nameof(ClientHandleDisplayNameUpdated))]
    private string displayName;

    public bool IsPartyOwner { get { return isPartyOwner; } }
    public string DisplayName { get { return displayName; } }

    [Server]
    public void SetIsPartyOwner(bool value) => isPartyOwner = value;
    [Server]
    public void SetDisplayName(string name) => displayName = name;

    public static event Action<bool> AuthorityOnPartyOwnerStateUpdated;
    public static event Action ClientOnInfoUpdated;


    #region Server

    private void AuthorityHandlePartyOwnerStateUpdated(bool oldState, bool newState)
    {
        if (!isOwned) return;

        AuthorityOnPartyOwnerStateUpdated?.Invoke(newState);
    }

    [Command]
    public void CmdStartGame()
    {
        if (!isPartyOwner) return;

        ((MyNetworkManager)NetworkManager.singleton).StartGame();
    }

    #endregion

    #region Client

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (NetworkServer.active) return;

        ((MyNetworkManager)NetworkManager.singleton).Players.Add(this);

        DontDestroyOnLoad(this);
    }

    public override void OnStopClient()
    {
        ClientOnInfoUpdated?.Invoke();

        if (!isClientOnly) return;

        ((MyNetworkManager)NetworkManager.singleton).Players.Remove(this);
    }

    private void ClientHandleDisplayNameUpdated(string oldDisplayName, string newDisplayName)
    {
        ClientOnInfoUpdated?.Invoke();
    }

    #endregion
}

