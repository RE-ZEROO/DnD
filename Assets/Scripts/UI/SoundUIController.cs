using UnityEngine;
using UnityEngine.UI;

public class SoundUIController : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("Images")]
    [SerializeField] private Image musicImage;
    [SerializeField] private Image sfxImage;

    [Header("Sprites")]
    [SerializeField] private Sprite musicSprite;
    [SerializeField] private Sprite musicSpriteOff;

    [SerializeField] private Sprite sfxSprite;
    [SerializeField] private Sprite sfxSpriteOff;


    public void ToggleMusic()
    {
        AudioManager.Instance.ToggleMusic();
        
        if (AudioManager.Instance.musicSource.mute)
            musicImage.sprite = musicSpriteOff;
        else
            musicImage.sprite = musicSprite;
    }


    public void ToggleSFX()
    {
        AudioManager.Instance.ToggleSFX();

        if (AudioManager.Instance.sfxSource.mute)
            sfxImage.sprite = sfxSpriteOff;
        else
            sfxImage.sprite = sfxSprite;
    }

    public void MusicVolume() => AudioManager.Instance.MusicVolume(musicSlider.value);
    public void SFXVolume() => AudioManager.Instance.SFXVolume(sfxSlider.value);
}