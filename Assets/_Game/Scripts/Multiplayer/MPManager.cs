using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using HorseRace.Playfab;
using HorseRace.UI;

namespace HorseRace.MultiPlayer
{
    public class MPManager : MonoBehaviourPunCallbacks
    {
        [SerializeField] private TextMeshProUGUI _connectionStateInfoTMP;

        private const int MaxPlayersCount = 2;

        private int playerCount = 0;

        public override void OnEnable()
        {
            base.OnEnable();
            GameMotor.Instance.OnRacingStarted += OnRaceStarted;

            PlayfabAuth.OnPlayerLoggedIn += OnPlayerLoggedIn;
            UIManager.OnPlayerClickedPlay += OnPlayerClickedPlay;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            GameMotor.Instance.OnRacingStarted -= OnRaceStarted;

            PlayfabAuth.OnPlayerLoggedIn -= OnPlayerLoggedIn;
            UIManager.OnPlayerClickedPlay -= OnPlayerClickedPlay;
        }

        private void FixedUpdate()
        {
            _connectionStateInfoTMP.text = PhotonNetwork.NetworkClientState.ToString(); // For the info
        }

        private void OnPlayerLoggedIn()
        {
            PhotonNetwork.ConnectUsingSettings();
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            RoomOptions roomOptions = new RoomOptions
            {
                MaxPlayers = MaxPlayersCount,
                IsVisible = true,
                IsOpen = true
            };
            int rndID = Random.Range(int.MinValue, int.MaxValue);
            PhotonNetwork.CreateRoom("Default: " + rndID, roomOptions, TypedLobby.Default);

        }

        public override void OnConnectedToMaster()
        {
            GameMotor.Instance.OnConnectedToMaster();
        }

        public override void OnJoinedRoom()
        {
            GameMotor.Instance.ChangeState(GameState.WaitingForOtherPlayer);

            photonView.RPC("AddPlayerCount", RpcTarget.AllBuffered);

            GameObject player = PhotonNetwork.Instantiate("Player", Vector3.zero + Vector3.right * Random.Range(-3, 2), Quaternion.identity, 0);
            player.GetComponent<Player>().SetUserName(GameMotor.Instance.GetPlayerName);
        }

        [PunRPC]
        private void AddPlayerCount()
        {
            playerCount++;
            if (playerCount == MaxPlayersCount)
            {
                GameMotor.Instance.ChangeState(GameState.HorseSelection);
            }
        }

        public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
        {
            if (!GameMotor.Instance.IsRacePreparing)
            {
                playerCount = 1;
                GameMotor.Instance.OtherPlayerLeft();
            }
        }

        private void OnRaceStarted()
        {
            if (PhotonNetwork.InRoom)
            {
                PhotonNetwork.LeaveRoom();
            }
            playerCount = 0;
        }

        private void OnPlayerClickedPlay()
        {
            PhotonNetwork.JoinRandomRoom();
        }
    }
}
