using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Jogador")]
    [SerializeField] TMPro.TMP_InputField nomeJogador;
    Player dadosJogador;

    public static GameManager Instance;
    Authenticator authenticator;

    [SerializeField] LobbyManager lobbyManager;
    [SerializeField] LobbyListController listController;
    [SerializeField] SalaPanelController salaController;
    Dictionary<string, Coroutine> heartbeats;
    [SerializeField] float heartbeatLobbyInterval = 20;
    [SerializeField] int updateLobbyInterval = 3;
    Coroutine atualizacaoSalas;

    public static LobbyManager Lobby { get { return Instance.lobbyManager; } }

    public static string PlayerId
    {
        get
        {
            if (Instance != null && AuthenticationService.Instance.IsSignedIn)
            {
                return AuthenticationService.Instance.PlayerId;
            }
            return string.Empty;
        }
    }

    public static Player Jogador
    {
        get
        {
            if (Instance.dadosJogador == null)
            {
                CriarDadosJogador();
            }
            return Instance.dadosJogador;
        }
    }

    public static void AtualizarSalas()
    {
        Instance.VerificarListaLobby();
    }

    public static SalaPanelController SalaController
    {
        get { return Instance.salaController; }
    }

    IEnumerator AtualizarListaSalas(float interval)
    {
        while (true)
        {
            VerificarListaLobby();
            yield return new WaitForSeconds(interval);
            Debug.Log("GameManager.AtualizarListaSalas - Executando");
        }
    }

    private void Awake()
    {
        if (GameManager.Instance == null)
        {
            GameManager.Instance = this;
            authenticator = new Authenticator();
        }
        else if (GameManager.Instance != this)
            Destroy(gameObject);

        heartbeats = new Dictionary<string, Coroutine>();
        salaController.HideSalaInfoPanel();
    }

    // Start is called before the first frame update
    void Start()
    {
        authenticator.SignIn(PlayerSignedInUnityService);
    }

    private void PlayerSignedInUnityService()
    {
        // Shows how to get a playerID
        Debug.Log($"GameManager.Instance.PlayerSignedInUnityService - PlayerID: {AuthenticationService.Instance.PlayerId}");
        // Shows how to get an access token
        Debug.Log($"GameManager.Instance.PlayerSignedInUnityService - Access Token: {AuthenticationService.Instance.AccessToken}");

        atualizacaoSalas = StartCoroutine(AtualizarListaSalas(updateLobbyInterval));
    }

    public static void CriarDadosJogador()
    {
        Player player = new Player(PlayerId);
        player.Data = new Dictionary<string, PlayerDataObject>();
        string nome = (string.IsNullOrEmpty(Instance.nomeJogador.text) ? string.Empty : Instance.nomeJogador.text);
        player.Data.Add("nome", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, nome));
        player.Data.Add("pronto", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, false.ToString()));
        Instance.dadosJogador = player;
    }

    //Chamado ao criar um lobby, atualizando a lista de lobbys na UI e iniciando o heartbeat desse lobby
    public static void CriarLobbyCallback(Lobby lobby)
    {
        Debug.Log($"GameManager.CriarLobbyCallback - LobbyId: {lobby.Id}");
        IniciarHeartbeatLobby(lobby.Id);
    }

    public static void ApagarLobbyCallback(string lobbyid)
    {
        if (Instance.heartbeats.ContainsKey(lobbyid))
        {
            //Parando a coroutine
            Instance.StopCoroutine(Instance.heartbeats[lobbyid]);
            //Removendo da lista
            Instance.heartbeats.Remove(lobbyid);
        }
    }

    static void IniciarHeartbeatLobby(string lobbyId)
    {
        if (Instance.heartbeats.ContainsKey(lobbyId))
            return;
        Coroutine cTemp = Instance.StartCoroutine(Instance.lobbyManager.Heartbeat(lobbyId, Instance.heartbeatLobbyInterval));
        Debug.Log($"GameManager.IniciarHeartbeatLobby: {lobbyId}");
        Instance.heartbeats.Add(lobbyId, cTemp);
    }

    public static void PararHeartbeat(string lobbyId)
    {
        if (Instance.heartbeats.TryGetValue(lobbyId, out Coroutine heartbeat))
        {
            Instance.StopCoroutine(heartbeat);
            Instance.heartbeats.Remove(lobbyId);
        }
    }

    public void VerificarListaLobby()
    {
        Debug.Log($"GameManager.VerificarListaLobby");
        listController.Listar();
    }

    private void OnDestroy()
    {
        //Parando as corotinas de heartbeats
        foreach (Coroutine c in heartbeats.Values)
        {
            Debug.Log($"Parando coroutine {c.GetHashCode()}");
            StopCoroutine(c);
        }
        //Parando a corotina de Atualização de sala
        StopCoroutine(atualizacaoSalas);
    }
}
