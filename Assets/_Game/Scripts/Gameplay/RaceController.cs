using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HorseRace.GamePlay
{
    public class RaceController : MonoBehaviour
    {
        private class Period
        {
            public float StartPosition;

            public float StartSpeed;
            public float EndSpeed;

            public float StartTime;
            public float EndTime;
        }

        public delegate void OnRaceActions(bool isPlayerWon);
        public static event OnRaceActions OnRaceFinished;

        private const float FinishZPosition = 250f;

        private HorseController _playerHorse, _opponentHorse;
        private List<Period> _playerHorsePeriods = new List<Period>();
        private List<Period> _otherHorsePeriods = new List<Period>();
        private Transform _cameraTransform;
        private Vector3 _cameraOffsetToPlayer;
        private float _camSpeed = 3f;
        private float _timeElapsedSinceRaceStart = 0;
        private bool _isPlayerWon;
        private bool _isRaceStarted;

        private void OnEnable()
        {
            GameMotor.Instance.OnPlayerEnteredRoom += OnPlayerEnteredRoom;
        }

        private void OnDisable()
        {
            GameMotor.Instance.OnPlayerEnteredRoom -= OnPlayerEnteredRoom;
        }

        private void OnPlayerEnteredRoom()
        {
            _cameraTransform.position = new Vector3(0, 4.75f, -13.25f);
            _cameraTransform.eulerAngles = new Vector3(35, 0, 0);

            _playerHorse.gameObject.SetActive(false);
            _playerHorse.transform.position = new Vector3(0f, 1.22f, -8.6f);
            _playerHorse.transform.eulerAngles = new Vector3(0f, -233.02f, 0f);
            _playerHorse.DisableText();
            _opponentHorse.gameObject.SetActive(false);
            Destroy(this);
        }

        private void Update()
        {
            if (_isRaceStarted)
            {
                _timeElapsedSinceRaceStart += Time.deltaTime;

                _playerHorse.transform.position = new Vector3(_playerHorse.transform.position.x, _playerHorse.transform.position.y, GetCurrentZPos(_playerHorsePeriods));
                _opponentHorse.transform.position = new Vector3(_opponentHorse.transform.position.x, _opponentHorse.transform.position.y, GetCurrentZPos(_otherHorsePeriods));

                UpdateCameraPosition();
            }
        }

        public void SetHorsesAndWinner(HorseController playerHorse, HorseController opponentHorse, bool isPlayerWon)
        {
            _playerHorse = playerHorse;
            _opponentHorse = opponentHorse;
            _isPlayerWon = isPlayerWon;

            _playerHorse.transform.position = new Vector3(2.2f, 0.48f, 0f);
            _playerHorse.transform.rotation = Quaternion.identity;
            _opponentHorse.transform.position = new Vector3(-2.2f, 0.48f, 0f);
            _opponentHorse.transform.rotation = Quaternion.identity;

            _cameraTransform = Camera.main.transform;
            _cameraTransform.position = new Vector3(17, 15, 15);
            _cameraTransform.eulerAngles = new Vector3(33, 245, 0);
            _cameraOffsetToPlayer = _cameraTransform.position - _playerHorse.transform.position;

            float playerFinishTime = _isPlayerWon ? Random.Range(13, 13.99f) : Random.Range(14.001f, 16f);
            float oponentFinishTime = _isPlayerWon ? Random.Range(14.001f, 16f) : Random.Range(13, 13.99f);
            GetPeriods(playerFinishTime, _playerHorsePeriods);
            GetPeriods(oponentFinishTime, _otherHorsePeriods);

            _playerHorse.StartRacing();
            _opponentHorse.StartRacing();
            _isRaceStarted = true;

            GameMotor.Instance.IsRacePreparing = false;
            GameMotor.Instance.ChangeState(GameState.Racing);
        }

        private void GetPeriods(float finishTime, List<Period> periodListToFill)
        {
            float remainingTime = finishTime;
            float periodTime = 0;
            float totalTimeElapsed = 0;
            float totalDistanceTraveled = 0;

            //starting Period
            float accelerateToSpeedAtStart = Random.Range(18f, 22f);
            float accelerationTime = Random.Range(1f, 1.2f);
            Period currentPeriod = new Period { StartPosition = 0, StartSpeed = 0, EndSpeed = accelerateToSpeedAtStart, StartTime = 0, EndTime = accelerationTime };
            AddPeriodToListAndCalculateOtherVariables(ref remainingTime, ref periodTime, ref totalTimeElapsed, ref totalDistanceTraveled, currentPeriod, periodListToFill);

            //constant speed Period
            float timeToContinueWithStartingSpeed = Random.Range(2.5f, 4.5f);
            currentPeriod = new Period { StartPosition = totalDistanceTraveled, StartSpeed = periodListToFill.Last().EndSpeed, EndSpeed = periodListToFill.Last().EndSpeed, StartTime = totalTimeElapsed, EndTime = totalTimeElapsed + timeToContinueWithStartingSpeed };
            AddPeriodToListAndCalculateOtherVariables(ref remainingTime, ref periodTime, ref totalTimeElapsed, ref totalDistanceTraveled, currentPeriod, periodListToFill);

            //accelerate or decelerate Period
            float timeToNextIteration = Random.Range(1f, 2.5f);
            float speedDifference = Random.Range(-8f, 10f);
            currentPeriod = new Period { StartPosition = totalDistanceTraveled, StartSpeed = periodListToFill.Last().EndSpeed, EndSpeed = periodListToFill.Last().EndSpeed + speedDifference, StartTime = totalTimeElapsed, EndTime = totalTimeElapsed + timeToNextIteration };
            AddPeriodToListAndCalculateOtherVariables(ref remainingTime, ref periodTime, ref totalTimeElapsed, ref totalDistanceTraveled, currentPeriod, periodListToFill);

            //lastPeriod for arriving finish Line on time
            float startSpeed = currentPeriod.EndSpeed;
            float distanceLeft = FinishZPosition - totalDistanceTraveled;
            float endSpeed = (distanceLeft - startSpeed * remainingTime) / remainingTime * 2f + startSpeed;
            currentPeriod = new Period { StartPosition = totalDistanceTraveled, StartSpeed = startSpeed, EndSpeed = endSpeed, StartTime = totalTimeElapsed, EndTime = totalTimeElapsed + remainingTime };
            AddPeriodToListAndCalculateOtherVariables(ref remainingTime, ref periodTime, ref totalTimeElapsed, ref totalDistanceTraveled, currentPeriod, periodListToFill);
        }

        private void AddPeriodToListAndCalculateOtherVariables(ref float remainingTime, ref float periodTime, ref float totalTimeElapsed, ref float totalDistanceTraveled, Period currentPeriod, List<Period> periodListToFill)
        {
            periodListToFill.Add(currentPeriod);
            periodTime = currentPeriod.EndTime - currentPeriod.StartTime;
            remainingTime -= periodTime;
            totalTimeElapsed += periodTime;
            totalDistanceTraveled = currentPeriod.StartPosition + (currentPeriod.EndSpeed - currentPeriod.StartSpeed) / 2f * periodTime + currentPeriod.StartSpeed * periodTime;
        }

        private float GetCurrentZPos(List<Period> periodList)
        {
            Period currentPeriod = periodList.Find(i => _timeElapsedSinceRaceStart > i.StartTime & _timeElapsedSinceRaceStart < i.EndTime);

            if (currentPeriod == null)
            {
                _isRaceStarted = false;

                _playerHorse.StopRacing();
                _opponentHorse.StopRacing();

                OnRaceFinished?.Invoke(_isPlayerWon);
                return FinishZPosition;
            }

            float elapsedTimeOnCurrentPeriod = _timeElapsedSinceRaceStart - currentPeriod.StartTime;
            float currentPos = currentPeriod.StartPosition + (_timeElapsedSinceRaceStart.Remap(currentPeriod.StartTime, currentPeriod.EndTime, currentPeriod.StartSpeed, currentPeriod.EndSpeed) - currentPeriod.StartSpeed) / 2f * elapsedTimeOnCurrentPeriod + currentPeriod.StartSpeed * elapsedTimeOnCurrentPeriod;

            return currentPos;
        }

        private void UpdateCameraPosition()
        {
            _cameraTransform.position = Vector3.Lerp(_cameraTransform.position, _playerHorse.transform.position + _cameraOffsetToPlayer, Time.deltaTime * _camSpeed);
        }
    }
}
