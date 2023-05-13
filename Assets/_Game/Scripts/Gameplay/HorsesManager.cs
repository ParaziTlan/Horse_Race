using UnityEngine;
using HorseRace.MultiPlayer;
using HorseRace.UI;

namespace HorseRace.GamePlay
{
    public class HorsesManager : MonoBehaviour
    {
        public static HorsesManager Instance;
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        public Material[] _horseMaterials, _jockeyMaterials;

        [SerializeField]
        private HorseController _horsePrefab;

        private HorseController _playerHorse;
        private HorseController _opponentsHorse;
        private Player _localPlayer;
        private string _localUserName;
        private string _otherUserName;
        private bool _isPlayerSelectedHorse;
        private bool _isOtherPlayerSelectedHorse;
        private int _horseIndex;
        private int _otherPlayerHorseIndex;

        private void OnEnable()
        {
            GameMotor.Instance.OnPlayerEnteredRoom += OnPlayerEnteredRoom;
            GameMotor.Instance.OnOtherPlayerEntered += OnOtherPlayerEnteredRoom;
            UIManager.OnPlayerSelectedHorse += OnPlayerSelectedHorse;
        }

        private void OnDisable()
        {
            GameMotor.Instance.OnPlayerEnteredRoom -= OnPlayerEnteredRoom;
            GameMotor.Instance.OnOtherPlayerEntered -= OnOtherPlayerEnteredRoom;
            UIManager.OnPlayerSelectedHorse -= OnPlayerSelectedHorse;
        }

        private void OnPlayerEnteredRoom()
        {
            _playerHorse.gameObject.SetActive(false);
        }

        private void OnOtherPlayerEnteredRoom()
        {
            _playerHorse.gameObject.SetActive(true);
            _isPlayerSelectedHorse = false;
            _isOtherPlayerSelectedHorse = false;
        }

        private void OnPlayerSelectedHorse()
        {
            _isPlayerSelectedHorse = true;
            _localPlayer.PlayerSelectedHorse(_horseIndex);

            if (_isOtherPlayerSelectedHorse)
            {
                StartRace();
            }
        }

        private void Start()
        {
            SpawnHorse();
        }

        public void SpawnHorse()
        {
            _playerHorse = Instantiate(_horsePrefab);
            _opponentsHorse = Instantiate(_horsePrefab);
            ChangeHorseToIndex(_playerHorse, _horseIndex);
            _playerHorse.gameObject.SetActive(false);
            _opponentsHorse.gameObject.SetActive(false);
        }

        private void ChangeHorseToIndex(HorseController horseToChange, int toIndex)
        {
            horseToChange.ChangeHorseTo(toIndex);
        }

        public int IncreaseIndexOfHorse(int increasingAmount)
        {
            _horseIndex += increasingAmount;

            if (_horseIndex > 11) _horseIndex -= 12;
            if (_horseIndex < 0) _horseIndex += 12;

            ChangeHorseToIndex(_playerHorse, _horseIndex);
            return _horseIndex;
        }

        public void SetLocalPlayer(Player localPlayer, string localUserName)
        {
            _localPlayer = localPlayer;
            _localUserName = localUserName;
        }

        public void SetOpponentName(string opponentName)
        {
            _otherUserName = opponentName;
        }

        public void OtherPlayerHorseSelected(int otherPlayerHorseIndex)
        {
            _otherPlayerHorseIndex = otherPlayerHorseIndex;
            _isOtherPlayerSelectedHorse = true;
            if (_isPlayerSelectedHorse)
            {
                StartRace();
            }
        }

        private void StartRace()
        {
            GameMotor.Instance.IsRacePreparing = true;
            PlayfabCloudCall.CallCloudScript(AfterCloudScriptCompleted, _horseIndex, _otherPlayerHorseIndex);
        }

        private void AfterCloudScriptCompleted(bool isPlayerWon)
        {
            _playerHorse.gameObject.SetActive(true);
            _playerHorse.SetUserNameText(_localUserName);
            _opponentsHorse.gameObject.SetActive(true);
            _opponentsHorse.SetUserNameText(_otherUserName);
            ChangeHorseToIndex(_opponentsHorse, _otherPlayerHorseIndex);

            gameObject.AddComponent<RaceController>().SetHorsesAndWinner(_playerHorse, _opponentsHorse, isPlayerWon);
        }

    }
}
