using Photon.Pun;
using HorseRace.GamePlay;

namespace HorseRace.MultiPlayer
{
    public class Player : MonoBehaviourPun
    {
        private string _userName;

        public void SetUserName(string userName)
        {
            _userName = userName;
        }

        private void Start()
        {
            if (photonView.IsMine)
            {
                HorsesManager.Instance.SetLocalPlayer(this, _userName);
                photonView.RPC("UserNameRPC", RpcTarget.AllBuffered, _userName);
            }
        }

        [PunRPC]
        private void UserNameRPC(string userName)
        {
            if (!photonView.IsMine)
            {
                SetUserName(userName);
                HorsesManager.Instance.SetOpponentName(userName);
            }
        }

        public void PlayerSelectedHorse(int horseIndex)
        {
            if (photonView.IsMine)
            {
                photonView.RPC("PlayerSelectedHorseRPC", RpcTarget.All, horseIndex);
            }
        }

        [PunRPC]
        private void PlayerSelectedHorseRPC(int horseIndex)
        {
            if (!photonView.IsMine)
            {
                HorsesManager.Instance.OtherPlayerHorseSelected(horseIndex);
            }
        }
    }
}
