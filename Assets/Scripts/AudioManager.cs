using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class AudioManager : MonoBehaviour
{
    public AudioMixer mixer;
    public AudioMixerGroup audioChannel;
    public Slider slider;
    public TMP_Text volumeText;

    // Start is called before the first frame update
    void Start()
    {
        //pull values from last saved data
        slider.value = PlayerPrefs.GetFloat(audioChannel.name + "Volume", 0.75f);
        volumeText.SetText(PlayerPrefs.GetString(audioChannel.name + "VolumeTxt", "00"));
    }

    public void SetVolume(float sliderValue)
    {
        //set exposed parameter from slider value
        mixer.SetFloat(audioChannel.name + "Volume", Mathf.Log10(sliderValue) * 20);

        //set volume txt output to correct value (converted to 1-100)
        int wholeNumberVolume = (int)(sliderValue * 100);
        volumeText.SetText(wholeNumberVolume.ToString());

        //save slider and volume values 
        PlayerPrefs.SetFloat(audioChannel.name + "Volume", sliderValue);
        PlayerPrefs.SetString(audioChannel.name + "VolumeTxt", wholeNumberVolume.ToString());
    }


}
