using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using Unity.Netcode;

using Unity.Netcode.Transports.UTP;
using TMPro;
using System.Net;
using System.Net.Sockets;

using System.Threading.Tasks;

public class LobbyManager : NetworkBehaviour
{


    public static LobbyManager Instance { get; private set; }

    public const string KEY_PLAYER_NAME = "PlayerName";
    public const string KEY_PLAYER_CHARACTER = "Character";
    public const string KEY_GAME_MODE = "GameMode";
    public const string KEY_START_GAME = "StartGame";
    public const string KEY_IP_ADDRESS = "IPAddress";
    public const string KEY_PLAYER_READINESS = "PlayerReadiness";

    [SerializeField] private GameObject allLobbyUI;
    [SerializeField] private GameObject camera;

    [SerializeField] private Transform playerCanvas;
    private bool wasExecuted = false;

    public event EventHandler OnLeftLobby;

    public event EventHandler<LobbyEventArgs> OnJoinedLobby;
    public event EventHandler<LobbyEventArgs> OnJoinedLobbyUpdate;
    public event EventHandler<LobbyEventArgs> OnKickedFromLobby;
    public event EventHandler<LobbyEventArgs> OnLobbyGameModeChanged;
    public class LobbyEventArgs : EventArgs
    {
        public Lobby lobby;
    }

    public event EventHandler<OnLobbyListChangedEventArgs> OnLobbyListChanged;
    public class OnLobbyListChangedEventArgs : EventArgs
    {
        public List<Lobby> lobbyList;
    }


    public enum GameMode
    {
        CaptureTheFlag,
        Conquest
    }

    public enum PlayerCharacter
    {
        Marine,
        Ninja,
        Zombie
    }



    private float heartbeatTimer;
    private float lobbyPollTimer;
    private float refreshLobbyListTimer = 5f;
    private Lobby joinedLobby;
    private string playerName;


    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        //HandleRefreshLobbyList(); // Disabled Auto Refresh for testing with multiple builds
        HandleLobbyHeartbeat();
        HandleLobbyPolling();
    }


    public async void AuthenticateAsServer(string playerName)
    {
        this.playerName = playerName;
        InitializationOptions initializationOptions = new InitializationOptions();
        initializationOptions.SetProfile(playerName);

        await UnityServices.InitializeAsync(initializationOptions);

        AuthenticationService.Instance.SignedIn += () =>
        {
            // do nothing
            Debug.Log("Signed in! " + AuthenticationService.Instance.PlayerId + "   " + playerName);

            CreateLobby(
                "Lobby name",
                5,
               false
            );

            RefreshLobbyList();
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();

    }

    public async void Authenticate(string playerName)
    {
        this.playerName = playerName;
        InitializationOptions initializationOptions = new InitializationOptions();
        initializationOptions.SetProfile(playerName);

        await UnityServices.InitializeAsync(initializationOptions);

        AuthenticationService.Instance.SignedIn += () =>
        {
            // do nothing
            Debug.Log("Signed in! " + AuthenticationService.Instance.PlayerId + "   " + playerName);

            RefreshLobbyList();
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();

    }

    //public async void Authenticate()
    //{
    //    await UnityServices.InitializeAsync();

    //    Debug.Log(UnityServices.State);

    //    AuthenticationService.Instance.SignedIn += () =>
    //    {
    //        // do nothing
    //        Debug.Log("Signed in! " + AuthenticationService.Instance.PlayerId);

    //        //RefreshLobbyList();
    //    };

    //    await AuthenticationService.Instance.SignInAnonymouslyAsync();
    //}

    private void HandleRefreshLobbyList()
    {
        if (UnityServices.State == ServicesInitializationState.Initialized && AuthenticationService.Instance.IsSignedIn)
        {
            refreshLobbyListTimer -= Time.deltaTime;
            if (refreshLobbyListTimer < 0f)
            {
                float refreshLobbyListTimerMax = 5f;
                refreshLobbyListTimer = refreshLobbyListTimerMax;

                RefreshLobbyList();
            }
        }
    }

    private async void HandleLobbyHeartbeat()
    {
        if (IsLobbyHost())
        {
            heartbeatTimer -= Time.deltaTime;
            if (heartbeatTimer < 0f)
            {
                float heartbeatTimerMax = 15f;
                heartbeatTimer = heartbeatTimerMax;

                Debug.Log("Heartbeat");
                await LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
            }
        }
    }

    private async void HandleLobbyPolling()
    {
        if (joinedLobby != null)
        {

            lobbyPollTimer -= Time.deltaTime;
            if (lobbyPollTimer < 0f)
            {
                //Debug.Log("LOBBY"); //works

                float lobbyPollTimerMax = 1.1f;
                lobbyPollTimer = lobbyPollTimerMax;

                joinedLobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);

                OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });

                Debug.Log(joinedLobby.Data[KEY_START_GAME].Value);

                //Debug.Log("joinedLobby.Data[KEY_START_GAME].Value != 0");

                if (joinedLobby.Data[KEY_START_GAME].Value != "0" && !wasExecuted)
                {
                    Debug.Log("2nd if works");

                    if (!IsLobbyHost())
                    {
                        Debug.Log("Starting Client as a non-host player...");
                        try
                        {
                            Instantiate(playerCanvas);

                            NetworkManager.Singleton.StartClient();

                            allLobbyUI.SetActive(false);
                            wasExecuted = true;
                        }
                        catch (Exception e)
                        {
                            Debug.LogError($"Error starting client: {e.Message}");
                        }
                    }

                    //joinedLobby = null;
                }


                if (!IsPlayerInLobby())
                {
                    // Player was kicked out of this lobby
                    Debug.Log("Kicked from Lobby!");

                    OnKickedFromLobby?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });

                    joinedLobby = null;
                }

            }
        }
        else {
            Debug.Log("NO LOBBY JOINED");
        }
    }

    public Lobby GetJoinedLobby()
    {
        return joinedLobby;
    }

    public bool IsLobbyHost()
    {
        return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    public string GetLobbyCode()
    {
        if (joinedLobby != null)
        {
            return GetJoinedLobby().LobbyCode;
        }
        else
        {
            return null;
        }

    }

    private bool IsPlayerInLobby()
    {
        if (joinedLobby != null && joinedLobby.Players != null)
        {
            foreach (Player player in joinedLobby.Players)
            {
                if (player.Id == AuthenticationService.Instance.PlayerId)
                {
                    // This player is in this lobby
                    return true;
                }
            }
        }
        return false;
    }

    public string GetPlayerName()
    {
        return GetPlayer().Data[KEY_PLAYER_NAME].Value;
    }

    private Player GetPlayer()
    {
        return new Player(AuthenticationService.Instance.PlayerId, null, new Dictionary<string, PlayerDataObject> {
            { KEY_PLAYER_NAME, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, playerName) },
            { KEY_PLAYER_CHARACTER, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, PlayerCharacter.Marine.ToString()) },
            { KEY_PLAYER_READINESS, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, "Not ready") }
        });

    }

    private Player GetHostPlayer()
    {
        return new Player(AuthenticationService.Instance.PlayerId, null, new Dictionary<string, PlayerDataObject> {
            { KEY_PLAYER_NAME, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, playerName) },
            { KEY_PLAYER_CHARACTER, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, PlayerCharacter.Marine.ToString()) },
            { KEY_PLAYER_READINESS, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, "") }
        });

    }

    public void ChangeGameMode()
    {
        if (IsLobbyHost())
        {
            GameMode gameMode =
                Enum.Parse<GameMode>(joinedLobby.Data[KEY_GAME_MODE].Value);

            switch (gameMode)
            {
                default:
                case GameMode.CaptureTheFlag:
                    gameMode = GameMode.Conquest;
                    break;
                case GameMode.Conquest:
                    gameMode = GameMode.CaptureTheFlag;
                    break;
            }

            UpdateLobbyGameMode(gameMode);
        }
    }

    public async void StartTheGame()
    {
        if (IsLobbyHost())
        {
            try
            {
                //NetworkManager.Singleton.StartServer();

                Lobby lobby = await Lobbies.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
                {
                    Data = new Dictionary<string, DataObject> {
                    { KEY_START_GAME, new DataObject(DataObject.VisibilityOptions.Public, "1") }
                }
                });

                joinedLobby = lobby;

                NetworkManager.Singleton.StartServer();

                allLobbyUI.SetActive(false);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }


        }


    }

    private bool ArePlayersReady(Lobby lobby)
    {
        int readyPlayers = 0;

        foreach (Player player in lobby.Players)
        {
            if (player.Data[KEY_PLAYER_READINESS].Value == "Ready")
            {
                readyPlayers++;
            }
        }

        if (readyPlayers == lobby.Players.Count - 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    public async void CreateLobby(string lobbyName, int maxPlayers, bool isPrivate)
    {
        Player player = GetHostPlayer();

        CreateLobbyOptions options = new CreateLobbyOptions
        {
            Player = player,
            IsPrivate = isPrivate,
            Data = new Dictionary<string, DataObject> {
                { KEY_START_GAME, new DataObject(DataObject.VisibilityOptions.Public, "0") },
                { KEY_IP_ADDRESS, new DataObject(DataObject.VisibilityOptions.Member, "0.0.0.0") }
            }
        };

        Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);

        joinedLobby = lobby;

        OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });

        GetLocalIPAddress();

        //Debug.Log("Lobby code: " + GetJoinedLobby().LobbyCode);
    }

    //public async void GetLocalIPAddress()
    //{
    //    var host = Dns.GetHostEntry(Dns.GetHostName());
    //    foreach (var ip in host.AddressList)
    //    {
    //        if (ip.AddressFamily == AddressFamily.InterNetwork)
    //        {
    //            //ipAddressText.text = ip.ToString();
    //            //ipAddress = ip.ToString();
    //            //return ip.ToString();

    //            Lobby lobby = await Lobbies.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
    //            {
    //                Data = new Dictionary<string, DataObject> {
    //                { KEY_IP_ADDRESS, new DataObject(DataObject.VisibilityOptions.Member, ip.ToString()) }
    //            }
    //            });

    //            Debug.Log("Created Lobby " + lobby.Name + " With IP: " + lobby.Data[LobbyManager.KEY_IP_ADDRESS].Value);
    //        }
    //    }
    //    //throw new System.Exception("No network adapters with an IPv4 address in the system!");
    //}






    //public async void GetLocalIPAddress()
    //{
    //    var host = Dns.GetHostEntry(Dns.GetHostName());
    //    foreach (var ip in host.AddressList)
    //    {
    //        if (ip.AddressFamily == AddressFamily.InterNetwork)
    //        {

    //            Lobby lobby = await Lobbies.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
    //            {
    //                Data = new Dictionary<string, DataObject> {
    //                { KEY_IP_ADDRESS, new DataObject(DataObject.VisibilityOptions.Member, ip.ToString()) }
    //            }
    //            });

    //            Debug.Log("Created Lobby " + lobby.Name + " With IP: " + lobby.Data[LobbyManager.KEY_IP_ADDRESS].Value);
    //        }
    //    }
    //}

    //public void SetIpAddress(string ipAddress)
    //{
    //    if (!IsLobbyHost())
    //    {
    //        UnityTransport transport;
    //        transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
    //        transport.ConnectionData.Address = ipAddress;

    //        Debug.Log("IP is set. Value: " + ipAddress);
    //    }
    //}



    public async void GetLocalIPAddress()
    {
        string localIPAddress = GetLocalIPAddressAsString(); // Get local IP address
        Lobby lobby = await Lobbies.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
        {
            Data = new Dictionary<string, DataObject> {
            { KEY_IP_ADDRESS, new DataObject(DataObject.VisibilityOptions.Member, localIPAddress) }
        }
        });

        Debug.Log("Created Lobby " + lobby.Name + " With IP: " + localIPAddress);
    }

    private string GetLocalIPAddressAsString()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        return null;
    }

    // Client code
    public void SetIpAddress(string ipAddress)
    {
        if (!IsLobbyHost())
        {
            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.ConnectionData.Address = ipAddress;

            Debug.Log("IP is set. Value: " + ipAddress);
        }
    }


    public async void RefreshLobbyList()
    {
        try
        {
            QueryLobbiesOptions options = new QueryLobbiesOptions();
            options.Count = 25;

            // Filter for open lobbies only
            options.Filters = new List<QueryFilter> {
                new QueryFilter(
                    field: QueryFilter.FieldOptions.AvailableSlots,
                    op: QueryFilter.OpOptions.GT,
                    value: "0")
            };

            // Order by newest lobbies first
            options.Order = new List<QueryOrder> {
                new QueryOrder(
                    asc: false,
                    field: QueryOrder.FieldOptions.Created)
            };

            QueryResponse lobbyListQueryResponse = await Lobbies.Instance.QueryLobbiesAsync();

            OnLobbyListChanged?.Invoke(this, new OnLobbyListChangedEventArgs { lobbyList = lobbyListQueryResponse.Results });
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void JoinLobbyByCode(string lobbyCode)
    {
        Player player = GetPlayer();

        Lobby lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, new JoinLobbyByCodeOptions
        {
            Player = player
        });

        joinedLobby = lobby;

        OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });

        Debug.Log("Joined Lobby by code" + lobby.Name + " With IP: " + joinedLobby.Data[KEY_IP_ADDRESS].Value);

        SetIpAddress(joinedLobby.Data[KEY_IP_ADDRESS].Value);
    }

    public async void JoinLobby(Lobby lobby)
    {
        Player player = GetPlayer();

        joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id, new JoinLobbyByIdOptions
        {
            Player = player
        });



        OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });

        Debug.Log("Joined Lobby not by code" + lobby.Name + " With IP: " + joinedLobby.Data[KEY_IP_ADDRESS].Value);

        SetIpAddress(joinedLobby.Data[KEY_IP_ADDRESS].Value);

        //if (lobby.Data.TryGetValue(KEY_IP_ADDRESS, out var ipAddressDataObject))
        //{
        //    SetIpAddress(ipAddressDataObject.Value);
        //}
        //else
        //{
        //    Debug.LogError("Key 'IPAddress' not present in the lobby data");
        //    // Handle the absence of the 'IPAddress' key accordingly
        //}
    }

    public async void UpdatePlayerName(string playerName)
    {
        this.playerName = playerName;

        if (joinedLobby != null)
        {
            try
            {
                UpdatePlayerOptions options = new UpdatePlayerOptions();

                options.Data = new Dictionary<string, PlayerDataObject>() {
                    {
                        KEY_PLAYER_NAME, new PlayerDataObject(
                            visibility: PlayerDataObject.VisibilityOptions.Public,
                            value: playerName)
                    }
                };

                string playerId = AuthenticationService.Instance.PlayerId;

                Lobby lobby = await LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id, playerId, options);
                joinedLobby = lobby;

                OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    public async void UpdatePlayerCharacter(PlayerCharacter playerCharacter)
    {
        if (joinedLobby != null)
        {
            try
            {
                UpdatePlayerOptions options = new UpdatePlayerOptions();

                options.Data = new Dictionary<string, PlayerDataObject>() {
                    {
                        KEY_PLAYER_CHARACTER, new PlayerDataObject(
                            visibility: PlayerDataObject.VisibilityOptions.Public,
                            value: playerCharacter.ToString())
                    }
                };

                string playerId = AuthenticationService.Instance.PlayerId;

                Lobby lobby = await LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id, playerId, options);
                joinedLobby = lobby;

                OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    public async void UpdatePlayerReadiness(string playerReadiness)
    {

        if (joinedLobby != null)
        {

            try
            {

                UpdatePlayerOptions options = new UpdatePlayerOptions();

                options.Data = new Dictionary<string, PlayerDataObject>() {
                    {
                        KEY_PLAYER_READINESS, new PlayerDataObject(
                            visibility: PlayerDataObject.VisibilityOptions.Public,
                            value: playerReadiness.ToString())
                    }
                };

                string playerId = AuthenticationService.Instance.PlayerId;

                Lobby lobby = await LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id, playerId, options);
                joinedLobby = lobby;

                OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    public async void QuickJoinLobby()
    {
        try
        {
            QuickJoinLobbyOptions options = new QuickJoinLobbyOptions();

            Lobby lobby = await LobbyService.Instance.QuickJoinLobbyAsync(options);
            joinedLobby = lobby;

            OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void LeaveLobby()
    {
        if (joinedLobby != null)
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);

                joinedLobby = null;

                OnLeftLobby?.Invoke(this, EventArgs.Empty);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    public async void KickPlayer(string playerId)
    {
        if (IsLobbyHost())
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, playerId);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    public async void UpdateLobbyGameMode(GameMode gameMode)
    {
        try
        {
            Debug.Log("UpdateLobbyGameMode " + gameMode);

            Lobby lobby = await Lobbies.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject> {
                    { KEY_GAME_MODE, new DataObject(DataObject.VisibilityOptions.Public, gameMode.ToString()) }
                }
            });

            joinedLobby = lobby;

            OnLobbyGameModeChanged?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

}