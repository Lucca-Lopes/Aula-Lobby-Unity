using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyListController : MonoBehaviour
{
    [SerializeField] Transform lobbiesContainer;
    [SerializeField] LobbyUI lobbyPrefab;

    Dictionary<string, LobbyUI> lobbyList = new Dictionary<string, LobbyUI>();

    internal async void Listar()
    {
        //LimparLista(); - só serve pra limpar a lista antes de receber mais dados
        List<Lobby> listaTemp = await GameManager.Lobby.ListarLobby();

        if (listaTemp.Count >= lobbyList.Count)
        {
            //Tem sala nova
            AtualizarListaSalas(listaTemp);
        }
        else
        {
            //Tem menos salas
            RemoverSalas(listaTemp);
        }
    }

    private void AtualizarListaSalas(List<Lobby> listaTemp)
    {
        foreach (Lobby item in listaTemp)
        { //Percorrendo a lista de salas do serviço
            if (lobbyList.ContainsKey(item.Id))
            {
                LobbyUI lobby = lobbyList[item.Id];
                if (item.Players.Count > 0)
                {
                    if (VerificarPlayerLobby(item))
                    {
                        GameManager.SalaController.ShowSalaInfoPanel();
                        if (lobby.Data.Players.Count != item.Players.Count)
                        {
                            GameManager.SalaController.AtualizarListaJogadores(item.Players);
                        }
                    }
                    else
                    {
                        GameManager.SalaController.LimparSala();
                        GameManager.SalaController.HideSalaInfoPanel();
                    }
                }
                AtualizarSala(item, lobby);
            }
            else
            {
                CriarLobby(item);
            }
        }
    }

    public void AtualizarListaSalasAction()
    {
        GameManager.AtualizarSalas();
    }

    private void AtualizarSala(Lobby lobby, LobbyUI lobbyTemp)
    {
        bool isHost = IsHost(lobby);
        bool playerNoLobby = VerificarPlayerLobby(lobby);
        lobbyTemp.InitLobbyData(lobby, isHost, playerNoLobby);
    }

    private bool VerificarPlayerLobby(Lobby lobby)
    {
        foreach (Player item in lobby.Players)
        {
            if (item.Id.Equals(GameManager.PlayerId))
                return true;
        }
        return false;
    }

    private bool IsHost(Lobby lobby)
    {
        //Verificando se é o host ou não
        return GameManager.PlayerId.Equals(lobby.HostId);
    }

    private void RemoverSalas(List<Lobby> listaTemp)
    {
        IEnumerable<string> excluirLobbies =
       lobbyList.Keys.Except(listaTemp.Select(l => l.Id)).ToList();
        foreach (string item in excluirLobbies)
        {
            GameManager.SalaController.LimparSala();
            GameManager.SalaController.HideSalaInfoPanel();
            Destroy(lobbyList[item].gameObject);
            lobbyList.Remove(item);
        }
    }

    public void CriarLobby(Lobby lobby)
    {
        LobbyUI lobbyTemp = Instantiate(lobbyPrefab, lobbiesContainer);
        AtualizarSala(lobby, lobbyTemp);
        lobbyList.Add(lobby.Id, lobbyTemp);
        Debug.Log($"Lista atualizada ({lobbyList.Count})");
    }
}
