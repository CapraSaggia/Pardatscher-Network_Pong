using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class MyNetworkManager : NetworkManager
{
    [SerializeField] private GameObject racketPrefab;
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private GameOverHandler gameOverHandlerPrefab;

    public static event Action ClientOnConnected;
    public static event Action ClientOnDisconnected;

    public List<Player> Players { get; } = new List<Player>();
    private bool isGameInProgress = false;


    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);
        Debug.Log(conn.address);
        Player player = conn.identity.GetComponent<Player>();
        Players.Add(player);

        player.SetDisplayName($"Player {Players.Count}");

        player.SetIsPartyOwner(Players.Count == 1);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        if (SceneManager.GetActiveScene().name.StartsWith("Map_"))
        {
            GameOverHandler gameOverHandlerInstance = Instantiate(gameOverHandlerPrefab);
            NetworkServer.Spawn(gameOverHandlerInstance.gameObject);


            foreach (Player player in Players)
            {
                GameObject racketInstance = Instantiate(
                     racketPrefab,
                    GetStartPosition().position,
                    Quaternion.identity);

                NetworkServer.Spawn(racketInstance, player.connectionToClient);

                GameObject ballInstance = Instantiate(
                    ballPrefab,
                    Vector3.zero,
                    Quaternion.identity
                );
                NetworkServer.Spawn(ballInstance);
            }
        }
    }

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        base.OnServerConnect(conn);

        if (!isGameInProgress) return;

        conn.Disconnect();
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {

        Players.Remove(conn.identity.GetComponent<Player>());

        base.OnServerDisconnect(conn);
    }

    public override void OnStopServer()
    {
        base.OnStopServer();

        Players.Clear();

        isGameInProgress = false;
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();

        ClientOnConnected?.Invoke();
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();

        ClientOnDisconnected?.Invoke();
    }

    public override void OnStopClient()
    {
        base.OnStopClient();

        Players.Clear();
    }

    public void StartGame()
    {
        //if (Players.Count != 2) return;

        isGameInProgress = true;

        ServerChangeScene("Map_NetworkPong");
    }
}
