using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyMenu : MonoBehaviour
{
    [SerializeField] private GameObject lobbyUI = null;
    [SerializeField] private GameObject startGameButton = null;
    [SerializeField] private TextMeshProUGUI[] playerNamesTexts = new TextMeshProUGUI[4];

    private void OnEnable()
    {
        MyNetworkManager.ClientOnConnected += HandleClientConnected;
        Player.AuthorityOnPartyOwnerStateUpdated += AuthorityHandlePartyOwnerStateUpdated;
        Player.ClientOnInfoUpdated += ClientHandleInfoUpdated;
    }

    private void OnDisable()
    {
        MyNetworkManager.ClientOnConnected -= HandleClientConnected;
        Player.AuthorityOnPartyOwnerStateUpdated -= AuthorityHandlePartyOwnerStateUpdated;
        Player.ClientOnInfoUpdated -= ClientHandleInfoUpdated;
    }

    private void HandleClientConnected()
    {
        lobbyUI.SetActive(true);
    }

    public void LeaveLobby()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        }
        else
        {
            NetworkManager.singleton.OnStopClient();

            SceneManager.LoadScene(0);
        }

    }


    private void AuthorityHandlePartyOwnerStateUpdated(bool state)
    {
        startGameButton.SetActive(state);
    }

    public void StartGame()
    {
        NetworkClient.connection.identity.GetComponent<Player>().CmdStartGame();
    }

    private void ClientHandleInfoUpdated()
    {
        List<Player> players = ((MyNetworkManager)NetworkManager.singleton).Players;

        for (int i = 0; i < players.Count; i++)
        {
            playerNamesTexts[i].text = players[i].DisplayName;
        }

        for (int i = players.Count; i < playerNamesTexts.Length; i++)
        {
            playerNamesTexts[i].text = "Waiting for Player...";
        }
    }
}
