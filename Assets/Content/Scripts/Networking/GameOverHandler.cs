using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class GameOverHandler : NetworkBehaviour
{
    //private List<UnitBase> bases = new List<UnitBase>();

    public static event Action ServerOnGameOver;
    public static event Action<string> ClientOnGameOver;

    #region Server

    public override void OnStartServer()
    {
        // UnitBase.ServerOnBaseSpawned += ServerHandleBaseSpawned;
        // UnitBase.ServerOnBaseDespawned += ServerHandleBaseDespawned;
    }

    public override void OnStopServer()
    {
        // UnitBase.ServerOnBaseSpawned -= ServerHandleBaseSpawned;
        // UnitBase.ServerOnBaseDespawned -= ServerHandleBaseDespawned;
    }

    [Server]
    private void ServerHandleBaseSpawned(/*UnitBase unitBase*/)
    {
        //bases.Add(unitBase);
    }

    [Server]
    private void ServerHandleBaseDespawned(/*UnitBase unitBase*/)
    {
        //bases.Remove(unitBase);

        //if (bases.Count != 1) return;

        //int playerID = bases[0].connectionToClient.connectionId;

        //RpcGameOver($"Player {playerID}");

        ServerOnGameOver?.Invoke();
    }

    #endregion

    #region Client

    [ClientRpc]
    private void RpcGameOver(string winner)
    {
        ClientOnGameOver?.Invoke(winner);
    }

    #endregion
}
