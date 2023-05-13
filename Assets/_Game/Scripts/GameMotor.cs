using UnityEngine;
using HorseRace.GamePlay;

namespace HorseRace
{
    public enum GameState
    {
        Login,
        JoinRoom,
        WaitingForOtherPlayer,
        HorseSelection,
        Racing
    }

    [DefaultExecutionOrder(-10)]
    public class GameMotor : MonoBehaviour
    {
        public static GameMotor Instance;
        private void Awake()
        {
            Application.targetFrameRate = 60;
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        public delegate void GameStateChangedDelegate();
        public event GameStateChangedDelegate OnPlayerLoggedIn;
        public event GameStateChangedDelegate OnPlayerEnteredRoom;
        public event GameStateChangedDelegate OnOtherPlayerEntered;
        public event GameStateChangedDelegate OnRacingStarted;

        public bool IsRacePreparing;
        public string GetPlayerName => _playerName;

        private string _playerName;
        private GameState _currentState = GameState.Login;

        private void OnEnable()
        {
            RaceController.OnRaceFinished += OnRaceFinished;
        }

        private void OnDisable()
        {
            RaceController.OnRaceFinished -= OnRaceFinished;
        }

        private void OnRaceFinished(bool obj)
        {
            _currentState = GameState.Login;
            ChangeState(GameState.JoinRoom);
        }

        public void ChangeState(GameState toState)
        {
            if ((int)toState == ((int)_currentState) + 1)
            {
                _currentState = toState;
            }
            else
            {
                Debug.LogError("GameFlow_Broken\nCurrent: " + _currentState + "  toState: " + toState);
                return;
            }

            if (_currentState == GameState.JoinRoom) OnPlayerLoggedIn?.Invoke();
            if (_currentState == GameState.WaitingForOtherPlayer) OnPlayerEnteredRoom?.Invoke();
            if (_currentState == GameState.HorseSelection) OnOtherPlayerEntered?.Invoke();
            if (_currentState == GameState.Racing) OnRacingStarted?.Invoke();
        }

        public void OtherPlayerLeft()
        {
            if (_currentState == GameState.HorseSelection)
            {
                _currentState = GameState.JoinRoom;
                ChangeState(GameState.WaitingForOtherPlayer);
            }
        }

        public void OnConnectedToMaster()
        {
            if (_currentState != GameState.Racing)
            {
                _currentState = GameState.Login;
                ChangeState(GameState.JoinRoom);
            }
        }

        public void PlayerLoggedIn(string playerName)
        {
            _playerName = playerName;
        }
    }
}
