using PlayFab.ClientModels;
using PlayFab;
using System.Collections.Generic;
using UnityEngine;

namespace HorseRace.UI
{
    public class LeaderBoardsUIManager : MonoBehaviour
    {
        [SerializeField] private LeaderBoardIndividualController _leaderBoardIndividualPrefab;
        [SerializeField] private Transform _usersParent;

        private List<LeaderBoardIndividualController> _leaderBoardIndividualControllers;

        public void UpdateLeaderBoard()
        {
            SetLeaderBoard();
        }

        private void SetLeaderBoard()
        {
            SetLeaderBoardsUsersReadyForShowing();

            GetLeaderboardRequest leaderboardRequest = new GetLeaderboardRequest();
            leaderboardRequest.MaxResultsCount = 10;
            leaderboardRequest.StatisticName = "Wins";

            PlayFabClientAPI.GetLeaderboard(leaderboardRequest, resultCallback =>
            {
                for (int i = 0; i < resultCallback.Leaderboard.Count; i++)
                {
                    _leaderBoardIndividualControllers[i].gameObject.SetActive(true);
                    _leaderBoardIndividualControllers[i].SetUserNameAndScore(resultCallback.Leaderboard[i].DisplayName, resultCallback.Leaderboard[i].StatValue.ToString());
                }

            },
            errorCallback =>
            {
                Debug.LogError(errorCallback.ErrorMessage);
                _leaderBoardIndividualControllers[0].gameObject.SetActive(true);
                _leaderBoardIndividualControllers[0].SetUserNameAndScore("Error : " + errorCallback.ErrorMessage, "");
            });
        }

        private void SetLeaderBoardsUsersReadyForShowing()
        {
            if (_leaderBoardIndividualControllers == null)
            {
                int leaderboarUserMaxCount = 10;

                _leaderBoardIndividualControllers = new List<LeaderBoardIndividualController>();
                for (int i = 0; i < leaderboarUserMaxCount; i++)
                {
                    _leaderBoardIndividualControllers.Add(Instantiate(_leaderBoardIndividualPrefab, _usersParent));
                }
            }
            _leaderBoardIndividualControllers.ForEach(i => i.gameObject.SetActive(false));
        }
    }
}
