using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [Header("Game Settings")]
    [SerializeField] private int totalLaps = 3;
    [SerializeField] private float countdownTime = 3f;
    [SerializeField] private float raceTimeLimit = 300f; // 5 minutes
    [SerializeField] private int numberOfAICars = 3;

    [Header("References")]
    [SerializeField] private PlayerCar playerCarPrefab;
    [SerializeField] private AICar aiCarPrefab;
    [SerializeField] private CameraFollow cameraFollow;

    [Header("Managers")]
    [SerializeField] private RaceProgressManager raceProgressManager;

    private UIManager uiManager => UIManager.Instance;

    private GameState currentGameState;
    private MapManager currentMap;

    internal int TotalRacer {  get; private set; }

    // Race data
    private float raceTimer;
    private float countdownTimer;
    private bool isRaceActive;
    private Dictionary<int, IHandleCarInput> racers = new Dictionary<int, IHandleCarInput>();
    #region Single Player Methods

    private void Update()
    {
        switch (currentGameState)
        {
            case GameState.Countdown:
                countdownTimer -= Time.deltaTime;
                uiManager.UpdateCountdown(countdownTimer);
                if (countdownTimer <= 0)
                {
                    StartRace();
                }
                break;

            case GameState.Racing:
                UpdateRace();
                break;
        }
    }

    public void LoadMapData(MapManager map)
    {
        this.currentMap = map;
    }

    // Initialize single player race
    public void StartSinglePlayerRace()
    {
        currentGameState = GameState.Loading;
        isRaceActive = false;
        TotalRacer = numberOfAICars + 1;

        SpawnPlayerCar();

        SpawnAICars();

        StartCountdown();
    }

    private void SpawnPlayerCar()
    {
        Vector3 playerSpawnPoint = currentMap.startpoints[0].position;
        PlayerCar playerCar = Instantiate(playerCarPrefab, playerSpawnPoint, currentMap.startRotation);
        playerCar.EnableControl = false;
        playerCar.InitRacerInfo(0, "Player");

        raceProgressManager.RegisterRacer(playerCar);

        CameraFollow camera = Instantiate(cameraFollow, Vector3.zero, Quaternion.identity);
        camera.carTarget = playerCar.transform;

        racers.Add(0, playerCar);
    }

    private void SpawnAICars()
    {
        for (int i = 0; i < numberOfAICars; i++)
        {
            if (i + 1 >= currentMap.startpoints.Count) break;

            Vector3 aiSpawnPoint = currentMap.startpoints[i + 1].position;
            AICar aiCar = Instantiate(aiCarPrefab, aiSpawnPoint, currentMap.startRotation);
            aiCar.LoadWaypointData(currentMap.waypoints);
            aiCar.EnableControl = false;

            aiCar.InitRacerInfo(i + 1, $"AI {i + 1}");
            raceProgressManager.RegisterRacer(aiCar);

            racers.Add(i + 1, aiCar);
        }
    }

    private void StartCountdown()
    {
        currentGameState = GameState.Countdown;
        countdownTimer = countdownTime;
        uiManager.ShowCountdown();
    }

    private void StartRace()
    {
        currentGameState = GameState.Racing;
        raceTimer = 0f;
        isRaceActive = true;
        uiManager.ShowRaceUI();

        // Enable car controls for all racers
        foreach (var racer in racers.Values)
        {
            racer.EnableControl = true;
        }
    }

    // Update race progress
    private void UpdateRace()
    {
        if (!isRaceActive) return;

        raceTimer += Time.deltaTime;

        // Update lap times
        raceProgressManager.UpdateRaceProgress();

        // Update UI
        //uiManager.UpdateRaceInfo(raceTimer, racerDataDict);
        uiManager.UpdateRacerPosition(raceProgressManager.GetCurrentRacerInfoList());

        CheckRaceEndConditions();
    }

    // Check if race should end
    private void CheckRaceEndConditions()
    {
        bool shouldEndRace = false;

        // Check time limit
        if (raceTimer >= raceTimeLimit)
        {
            shouldEndRace = true;
        }

        // Check if all racers finished

        if (raceProgressManager.IsAllRacerFinished())
        {
            shouldEndRace = true;
        }

        if (shouldEndRace)
        {
            EndRace();
        }
    }

    // End the race
    private void EndRace()
    {
        isRaceActive = false;
        currentGameState = GameState.RaceEnd;

        // Disable all car controls
        foreach (var racer in racers.Values)
        {
            racer.EnableControl = false;
        }

        uiManager.ShowRaceResults();
    }

    public void PauseGame()
    {
        if (currentGameState == GameState.Racing)
        {
            currentGameState = GameState.Paused;
            Time.timeScale = 0;
            uiManager.ShowPauseMenu();
        }
    }

    public void ResumeGame()
    {
        if (currentGameState == GameState.Paused)
        {
            currentGameState = GameState.Racing;
            Time.timeScale = 1;
            uiManager.HidePauseMenu();
        }
    }
    
    #endregion

    #region Multiplayer Methods (To Be Implemented)

    // Host/Client setup
    public void StartHost()
    {
        // TODO: Implement host initialization
    }

    public void StartClient()
    {
        // TODO: Implement client connection
    }

    public void DisconnectMultiplayer()
    {
        // TODO: Implement disconnection logic
    }

    // Race management
    private void OnPlayerJoined(ulong clientId)
    {
        // TODO: Handle new player joining
    }

    private void OnPlayerLeft(ulong clientId)
    {
        // TODO: Handle player disconnection
    }

    //[ServerRpc]
    private void StartMultiplayerRaceServerRpc()
    {
        // TODO: Implement multiplayer race start
    }

    //[ServerRpc]
    private void PlayerCheckpointCrossedServerRpc(ulong clientId, int checkpointIndex)
    {
        // TODO: Implement multiplayer checkpoint handling
    }

    //[ClientRpc]
    private void UpdateRacePositionsClientRpc()
    {
        // TODO: Implement position updates for clients
    }

    //[ClientRpc]
    private void EndMultiplayerRaceClientRpc()
    {
        // TODO: Implement multiplayer race end
    }

    #endregion
}

public enum GameState
{
    MainMenu,
    Loading,
    Countdown,
    Racing,
    Paused,
    RaceEnd
}

public enum GameMode
{
    SinglePlay,
    MultiPlay,
}
