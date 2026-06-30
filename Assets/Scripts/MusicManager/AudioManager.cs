using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Header("Настройки микшера")]
    [SerializeField] private AudioMixer _audioMixer;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource _sfxSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip _backgroundMusic;
    [SerializeField] private AudioClip _playerHitSound;
    [SerializeField] private AudioClip _enemyDeathSound;
    [SerializeField] private AudioClip _playerDeathSound;

    private float _originalMusicVolume;
    void Start()
    {
        ApplySavedVolumes();
        _originalMusicVolume = _musicSource.volume;
        PlayMusic(_backgroundMusic);
    }
    private void ApplySavedVolumes()
    {
        if (_audioMixer == null) return;

        float savedMaster = PlayerPrefs.GetFloat("MasterVolumeKey", 1f);
        float savedMusic = PlayerPrefs.GetFloat("MusicVolumeKey", 1f);
        float savedSFX = PlayerPrefs.GetFloat("SfxVolumeKey", 1f);

        _audioMixer.SetFloat("MasterVol", Mathf.Log10(Mathf.Max(savedMaster, 0.0001f)) * 20f);
        _audioMixer.SetFloat("MusicVol", Mathf.Log10(Mathf.Max(savedMusic, 0.0001f)) * 20f);
        _audioMixer.SetFloat("SfxVol", Mathf.Log10(Mathf.Max(savedSFX, 0.0001f)) * 20f);
    }
    public void PlayMusic(AudioClip musicClip)
    {
        if (musicClip != null && _musicSource != null)
        {
            _musicSource.clip = musicClip;
            _musicSource.loop = true;
            _musicSource.volume = _originalMusicVolume;
            _musicSource.Play();
        }
    }
    
    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && _sfxSource != null)
        {
            _sfxSource.PlayOneShot(clip);
        }
    }
    public void PlayPlayerHit() => PlaySFX(_playerHitSound);
    public void PlayEnemyDeath() => PlaySFX(_enemyDeathSound);
    public void PlayPlayerDeath()  
    {
        StartCoroutine(FadeOutBackgroundMusic(1f));
    }
    private IEnumerator FadeOutBackgroundMusic(float duration)
    {
        if (_musicSource == null) yield break;

        float startVolume = _musicSource.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime; 
            _musicSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
            yield return null;
        }

        _musicSource.Stop();

        if (_playerDeathSound != null)
        {
            _musicSource.clip = _playerDeathSound;
            _musicSource.loop = true;
            _musicSource.volume = _originalMusicVolume;
            _musicSource.Play();
        }
    }
}
