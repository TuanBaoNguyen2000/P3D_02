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
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private GameObject playerCarPrefab;
    [SerializeField] private GameObject aiCarPrefab;

    private UIManager uiManager => UIManager.Instance;

    private GameState currentGameState;

    // Race data
    private float raceTimer;
    private float countdownTimer;
    private bool isRaceActive;
    private Dictionary<int, RacerData> racerDataDict = new Dictionary<int, RacerData>();

    #region Single Player Methods

    // Initialize single player race
    public void StartSinglePlayerRace()
    {
        currentGameState = GameState.Loading;
        racerDataDict.Clear();
        isRaceActive = false;

        // Spawn player car
        SpawnPlayerCar();

        // Spawn AI cars
        SpawnAICars();

        // Start countdown
        StartCountdown();
    }

    // Spawn player car at first spawn point
    private void SpawnPlayerCar()
    {
        Vector3 playerSpawnPoint = spawnPoints[0].position;
        GameObject playerCar = Instantiate(playerCarPrefab, playerSpawnPoint, spawnPoints[0].rotation);

        // Initialize player data
        RacerData playerData = new RacerData
        {
            racerName = "Player",
            racerObject = playerCar,
            currentLap = 0,
            isFinished = false,
            isAI = false,
            checkpointIndex = 0
        };
        racerDataDict.Add(0, playerData); // Player always has ID 0
    }

    // Spawn AI cars at remaining spawn points
    private void SpawnAICars()
    {
        for (int i = 0; i < numberOfAICars; i++)
        {
            if (i + 1 >= spawnPoints.Length) break;

            Vector3 aiSpawnPoint = spawnPoints[i + 1].position;
            GameObject aiCar = Instantiate(aiCarPrefab, aiSpawnPoint, spawnPoints[i + 1].rotation);

            // Initialize AI data
            RacerData aiData = new RacerData
            {
                racerName = $"AI {i + 1}",
                racerObject = aiCar,
                currentLap = 0,
                isFinished = false,
                isAI = true,
                checkpointIndex = 0
            };
            racerDataDict.Add(i + 1, aiData);
        }
    }

    // Start race countdown
    private void StartCountdown()
    {
        currentGameState = GameState.Countdown;
        countdownTimer = countdownTime;
        uiManager.ShowCountdown();
    }

    // Start the race
    private void StartRace()
    {
        currentGameState = GameState.Racing;
        raceTimer = 0f;
        isRaceActive = true;
        uiManager.ShowRaceUI();

        // Enable car controls for all racers
        foreach (var racer in racerDataDict.Values)
        {
            if (!racer.isAI)
            {
                // Enable player controls
                //racer.racerObject.GetComponent<PlayerController>().EnableControls();
            }
            else
            {
                // Enable AI controls
                //racer.racerObject.GetComponent<AIController>().StartRacing();
            }
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

        // Calculate positions
        CalculateRacePositions();

        // Update UI
        //uiManager.UpdateRaceInfo(raceTimer, racerDataDict);

        // Check race end conditions
        CheckRaceEndConditions();
    }

    // Calculate current race positions
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
            if (!racer.isAI)
            {
                //racer.racerObject.GetComponent<PlayerController>().DisableControls();
            }
            else
            {
                //racer.racerObject.GetComponent<AIController>().StopRacing();
            }
        }

        // Show results
        uiManager.ShowRaceResults();
    }

    // Pause handling
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
    public GameObject racerObject;
    public int currentLap;
    public float currentLapTime;
    public float bestLapTime;
    public int position;
    public bool isFinished;
    public bool isAI;
    public int checkpointIndex;
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
