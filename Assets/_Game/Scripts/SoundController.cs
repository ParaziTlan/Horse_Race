using UnityEngine;
using HorseRace.UI;

namespace HorseRace
{
    public class SoundController : MonoBehaviour
    {
        [SerializeField]
        private AudioSource _audioSource;

        private const string SoundEnabledKey = "soundEnabled";

        private void OnEnable()
        {
            UIManager.OnSoundSettingsChanged += OnSoundSettingsChanged;
        }

        private void OnDisable()
        {
            UIManager.OnSoundSettingsChanged -= OnSoundSettingsChanged;
        }

        private void Start()
        {
            OnSoundSettingsChanged();
        }

        private void OnSoundSettingsChanged()
        {
            bool soundEnabled = PlayerPrefs.GetInt(SoundEnabledKey, 0) == 1;
            if (soundEnabled) _audioSource.Play();
            else _audioSource.Pause();
        }
    }
}
