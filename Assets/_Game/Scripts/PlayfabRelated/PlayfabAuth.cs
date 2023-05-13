using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System;
using HorseRace.UI;

namespace HorseRace.Playfab
{
    public class PlayfabAuth : MonoBehaviour
    {
        public static event Action OnPlayerLoggedIn;
        public static event Action<string> OnUpdateLoginInfo;

        private LoginWithPlayFabRequest _loginRequest;
        private string _userName;
        private string _password;

        private void OnEnable()
        {
            UIManager.OnPlayerClickedLogin += OnPlayerClickedLogin;
        }

        private void OnDisable()
        {
            UIManager.OnPlayerClickedLogin -= OnPlayerClickedLogin;
        }

        private void OnPlayerClickedLogin(string userName, string passWord)
        {
            _userName = userName;
            _password = passWord;
            Login();
        }

        public void Login()
        {
            _loginRequest = new LoginWithPlayFabRequest();
            _loginRequest.Username = _userName;
            _loginRequest.Password = _password;

            PlayFabClientAPI.LoginWithPlayFab(_loginRequest, resultCallback => //If the account found
            {
                OnUpdateLoginInfo?.Invoke("Welcome " + _userName + "\nConnecting...");
                GameMotor.Instance.PlayerLoggedIn(_userName);
                OnPlayerLoggedIn?.Invoke();
            }, errorCallback =>  //If the account not found
            {
                OnUpdateLoginInfo?.Invoke("Failed to login your account [" + errorCallback.ErrorMessage + "]");

                if (errorCallback.ErrorMessage.Equals("User not found"))
                {
                    OnUpdateLoginInfo?.Invoke("User not found Creating New One");
                    Register();
                }
                Debug.LogError(errorCallback.ErrorMessage);
            }, null);
        }

        private void Register()
        {
            RegisterPlayFabUserRequest registerRequest = new RegisterPlayFabUserRequest();
            registerRequest.Username = _userName;
            registerRequest.Password = _password;
            registerRequest.RequireBothUsernameAndEmail = false;
            registerRequest.DisplayName = _userName;

            PlayFabClientAPI.RegisterPlayFabUser(registerRequest, resultCallback =>
            {
                OnUpdateLoginInfo?.Invoke("New Account Has Created");
                Login();
            }, errorCallback =>
            {
                OnUpdateLoginInfo?.Invoke("Failed to Create your account [" + errorCallback.ErrorMessage + "]");
                Debug.Log(errorCallback.ErrorMessage);
            });
        }
    }
}
