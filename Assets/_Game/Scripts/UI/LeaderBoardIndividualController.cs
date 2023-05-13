using TMPro;
using UnityEngine;

namespace HorseRace.UI
{
    public class LeaderBoardIndividualController : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _userNameText, _scoreText;

        public void SetUserNameAndScore(string username, string score)
        {
            _userNameText.text = username;
            _scoreText.text = score;
        }
    }
}
