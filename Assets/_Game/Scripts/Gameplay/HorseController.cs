using TMPro;
using UnityEngine;

namespace HorseRace.GamePlay
{
    public class HorseController : MonoBehaviour
    {
        [SerializeField]
        private Animator _animator;

        [SerializeField]
        private SkinnedMeshRenderer _horseRenderer, _jockeyRenderer;

        [SerializeField]
        private TextMeshPro _userNameTMP; // It is 3D TMP, That is why it's here

        private const string StartToMoveTrigger = "StartToMove", StopMovingTrigger = "Stop", AnimationSpeedName = "Speed";

        private bool _isRacing;
        private float _lastPos;

        private void LateUpdate()
        {
            if (_isRacing)
            {
                float speed = (transform.position.z - _lastPos) / Time.deltaTime;
                _lastPos = transform.position.z;
                _animator.SetFloat(AnimationSpeedName, Mathf.Clamp01(speed.Remap(0f, 24f, 0f, 1f)));
            }
        }

        public void ChangeHorseTo(int index)
        {
            (int, int) horseAndJockeyIndices = HorseContainer.Instance.GetHorseAndJockeyIndex(index);

            _horseRenderer.sharedMaterial = HorsesManager.Instance._horseMaterials[horseAndJockeyIndices.Item1];
            _jockeyRenderer.sharedMaterial = HorsesManager.Instance._jockeyMaterials[horseAndJockeyIndices.Item2];
        }

        public void SetUserNameText(string userName)
        {
            _userNameTMP.text = userName;
            _userNameTMP.gameObject.SetActive(true);
        }

        public void DisableText()
        {
            _userNameTMP.gameObject.SetActive(false);
        }

        public void StartRacing()
        {
            _isRacing = true;
            _animator.SetTrigger(StartToMoveTrigger);
        }

        public void StopRacing()
        {
            _isRacing = false;
            _animator.SetTrigger(StopMovingTrigger);
        }
    }
}
