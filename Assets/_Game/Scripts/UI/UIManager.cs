using TMPro;
using UnityEngine;
using HorseRace.GamePlay;
using HorseRace.Playfab;

namespace HorseRace.UI
{
    public class UIManager : MonoBehaviour
    {
        public delegate void OnButtonClicksActions();
        public static event OnButtonClicksActions OnSoundSettingsChanged;
        public static event OnButtonClicksActions OnPlayerSelectedHorse;
        public static event OnButtonClicksActions OnPlayerClickedPlay;

        public delegate void OnLoginButtonClickedDelegate(string userName, string password);
        public static event OnLoginButtonClickedDelegate OnPlayerClickedLogin;

        private const string SoundEnabledKey = "soundEnabled";

        [SerializeField] private TMP_InputField _userNameInputField, _passwordInputField;
        [SerializeField] private GameObject _loginPanelObj, _mainMenuObj, _waitingForPlayerObj, _leaderBoardsPanelObj, _horseSelectionObj;
        [SerializeField] private TextMeshProUGUI _messageLabelForLogin, _horsePropertyNameTMP, _winLoseInfoTMP, _soundTMP;
        [SerializeField] private LeaderBoardsUIManager _leaderBoardsController;

        private void OnEnable()
        {
            GameMotor.Instance.OnPlayerLoggedIn += OnPlayerLoggedIn;
            GameMotor.Instance.OnPlayerEnteredRoom += OnPlayerJoinedRoom;
            GameMotor.Instance.OnOtherPlayerEntered += OnOtherPlayerEnteredRoom;
            RaceController.OnRaceFinished += OnRaceFinished;
            PlayfabAuth.OnUpdateLoginInfo += OnUpdateLoginInfo;
        }

        private void OnDisable()
        {
            GameMotor.Instance.OnPlayerLoggedIn -= OnPlayerLoggedIn;
            GameMotor.Instance.OnPlayerEnteredRoom -= OnPlayerJoinedRoom;
            GameMotor.Instance.OnOtherPlayerEntered -= OnOtherPlayerEnteredRoom;
            RaceController.OnRaceFinished -= OnRaceFinished;
            PlayfabAuth.OnUpdateLoginInfo -= OnUpdateLoginInfo;
        }

        private void OnPlayerLoggedIn()
        {
            _loginPanelObj.SetActive(false);
            _mainMenuObj.SetActive(true);
        }

        private void OnPlayerJoinedRoom()
        {
            _mainMenuObj.SetActive(false);
            _waitingForPlayerObj.SetActive(true);
            _horseSelectionObj.SetActive(false);
        }

        private void OnOtherPlayerEnteredRoom()
        {
            _waitingForPlayerObj.SetActive(false);
            _horseSelectionObj.SetActive(true);
        }

        private void OnRaceFinished(bool isPlayerWon)
        {
            _winLoseInfoTMP.gameObject.SetActive(true);
            _winLoseInfoTMP.text = isPlayerWon ? "You Win!" : "You Lose!";
        }

        private void OnUpdateLoginInfo(string messageStr)
        {
            _messageLabelForLogin.text = messageStr;
        }

        private void Start()
        {
            UpdateHorseName(0);
            UpdateSoundText();
        }

        private void UpdateHorseName(int index)
        {
            _horsePropertyNameTMP.text = HorseContainer.Instance.GetHorseName(index);
        }

        private void UpdateSoundText()
        {
            bool soundEnabled = PlayerPrefs.GetInt(SoundEnabledKey, 0) == 1;
            _soundTMP.text = soundEnabled ? "Sound\nEnabled" : "Sound\nDisableb";
        }

        #region ButtonClicks
        public void OnNextButtonClicked()
        {
            int changedIndex = HorsesManager.Instance.IncreaseIndexOfHorse(1);
            UpdateHorseName(changedIndex);
        }

        public void OnPrevButtonClicked()
        {
            int changedIndex = HorsesManager.Instance.IncreaseIndexOfHorse(-1);
            UpdateHorseName(changedIndex);
        }

        public void OnSelectButtonClicked()
        {
            _horseSelectionObj.SetActive(false);
            OnPlayerSelectedHorse?.Invoke();
        }

        public void OnOpenLeaderBoardsPanelClicked()
        {
            _leaderBoardsPanelObj.gameObject.SetActive(true);
            _leaderBoardsController.UpdateLeaderBoard();
            _mainMenuObj.gameObject.SetActive(false);
        }

        public void OnCloseLeaderBoardsPanelClicked()
        {
            _leaderBoardsPanelObj.gameObject.SetActive(false);
            _mainMenuObj.gameObject.SetActive(true);
        }

        public void OnChangeSoundClicked()
        {
            bool soundEnabled = PlayerPrefs.GetInt(SoundEnabledKey, 0) == 1;
            soundEnabled = !soundEnabled;
            PlayerPrefs.SetInt(SoundEnabledKey, soundEnabled ? 1 : 0);
            UpdateSoundText();
            OnSoundSettingsChanged?.Invoke();
        }

        public void OnPlayButtonClicked()
        {
            OnPlayerClickedPlay?.Invoke();
        }

        public void OnLoginButtonClicked()
        {
            OnPlayerClickedLogin?.Invoke(_userNameInputField.text, _passwordInputField.text);
        }
        public void OnTestAccount1Clicked()
        {
            _userNameInputField.text = "test1";
            _passwordInputField.text = "testAccount1";
            OnLoginButtonClicked();
        }
        public void OnTestAccount2Clicked()
        {
            _userNameInputField.text = "test2";
            _passwordInputField.text = "testAccount2";
            OnLoginButtonClicked();
        }
        #endregion
    }
}