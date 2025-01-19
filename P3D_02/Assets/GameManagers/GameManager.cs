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

    private UIManager uiManager => UIManager.Instance;

    private GameState currentGameState;
    private MapManager currentMap;

    internal int TotalRacer {  get; private set; }

    // Race data
    private float raceTimer;
    private float countdownTimer;
    private bool isRaceActive;
    private Dictionary<int, RacerData> racerDataDict = new Dictionary<int, RacerData>();

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
        racerDataDict.Clear();
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

        // Initialize player data
        RacerData playerData = new RacerData
        {
            racerName = "Player",
            racerHandleInput = playerCar,
            currentLap = 0,
            isFinished = false,
            isAI = false,
            checkpointIndex = 0
        };

        CameraFollow camera = Instantiate(cameraFollow, Vector3.zero, Quaternion.identity);
        camera.carTarget = playerCar.transform;
        racerDataDict.Add(0, playerData); // Player always has ID 0
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

            // Initialize AI data
            RacerData aiData = new RacerData
            {
                racerName = $"AI {i + 1}",
                racerHandleInput = aiCar,
                currentLap = 0,
                isFinished = false,
                isAI = true,
                checkpointIndex = 0
            };

            racerDataDict.Add(i + 1, aiData);
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
        foreach (var racer in racerDataDict.Values)
        {
            racer.racerHandleInput.EnableControl = true;
        }
    }

    // Update race progress
    private void UpdateRace()
    {
        if (!isRaceActive) return;

        raceTimer += Time.deltaTime;

        // Update lap times
        foreach (var racer in racerDataDict.Values)
        {
            if (!racer.isFinished)
            {
                racer.currentLapTime += Time.deltaTime;
            }
        }

        CalculateRacePositions();

        // Update UI
        //uiManager.UpdateRaceInfo(raceTimer, racerDataDict);
        uiManager.UpdateRacerPosition(racerDataDict);

        CheckRaceEndConditions();
    }

    private void CalculateRacePositions()
    {
        // Convert dictionary to list for sorting
        var racerList = new List<KeyValuePair<int, RacerData>>(racerDataDict);

        // Sort based on laps and checkpoints
        racerList.Sort((a, b) =>
        {
            // Compare laps
            if (a.Value.currentLap != b.Value.currentLap)
                return b.Value.currentLap.CompareTo(a.Value.currentLap);

            // Compare checkpoints if on same lap
            if (a.Value.checkpointIndex != b.Value.checkpointIndex)
                return b.Value.checkpointIndex.CompareTo(a.Value.checkpointIndex);

            // Compare lap times if at same checkpoint
            return a.Value.currentLapTime.CompareTo(b.Value.currentLapTime);
        });

        // Assign positions
        for (int i = 0; i < racerList.Count; i++)
        {
            racerList[i].Value.position = i + 1;
        }

    }

    // Handle checkpoint crossing
    public void OnCheckpointCrossed(int racerId, int checkpointIndex)
    {
        if (!racerDataDict.ContainsKey(racerId)) return;

        RacerData racer = racerDataDict[racerId];

        // Update checkpoint index
        racer.checkpointIndex = checkpointIndex;

        // Check if completed a lap
        if (checkpointIndex == 0) // Assuming 0 is finish line
        {
            CompleteLap(racerId);
        }
    }

    // Handle lap completion
    private void CompleteLap(int racerId)
    {
        RacerData racer = racerDataDict[racerId];
        racer.currentLap++;

        // Update best lap time
        if (racer.bestLapTime == 0 || racer.currentLapTime < racer.bestLapTime)
        {
            racer.bestLapTime = racer.currentLapTime;
        }

        // Reset current lap time
        racer.currentLapTime = 0;

        // Check if race is finished for this racer
        if (racer.currentLap >= totalLaps)
        {
            racer.isFinished = true;
        }
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
        bool allFinished = true;
        foreach (var racer in racerDataDict.Values)
        {
            if (!racer.isFinished)
            {
                allFinished = false;
                break;
            }
        }

        if (allFinished)
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
        foreach (var racer in racerDataDict.Values)
        {
            racer.racerHandleInput.EnableControl = false;
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

[System.Serializable]
public class RacerData
{
    public string racerName;
    public IHandleCarInput racerHandleInput;
    public bool isAI;

    public float currentLapTime;
    public float bestLapTime;

    public int position;
    public bool isFinished;

    public int currentLap;
    public int checkpointIndex;
    public float distanceFromNextCheckpoint;
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
