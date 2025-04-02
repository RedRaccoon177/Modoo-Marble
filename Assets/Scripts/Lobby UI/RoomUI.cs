using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomUI : MonoBehaviour
{
    GameSound gameSound;
    public GameObject optionUi;
    SoundManager soundManager;
    public GameObject masterSound;
    public GameObject bgmSound;
    public GameObject sfxSound;
    public TextMeshProUGUI roomName;

    private void Start()
    {
        gameSound = GameObject.Find("Click").GetComponent<GameSound>();
        soundManager = GameObject.Find("SoundMgr").GetComponent<SoundManager>();
        soundManager.audioSlider1 = masterSound.GetComponent<Slider>();
        soundManager.audioSlider2 = bgmSound.GetComponent<Slider>();
        soundManager.audioSlider3 = sfxSound.GetComponent<Slider>();

        masterSound.GetComponent<Slider>().onValueChanged.AddListener(soundManager.MasterAudioControl);
        bgmSound.GetComponent<Slider>().onValueChanged.AddListener(soundManager.BGMAudioControl);
        sfxSound.GetComponent<Slider>().onValueChanged.AddListener(soundManager.SFXAudioControl);

        soundManager.LoadData();
        masterSound.GetComponent<Slider>().value = soundManager.soundData.master;
        bgmSound.GetComponent<Slider>().value = soundManager.soundData.bgm;
        sfxSound.GetComponent<Slider>().value = soundManager.soundData.sfx;

        roomName.text = PhotonNetwork.CurrentRoom.Name;
    }

    public void onClickServerBtnSoundPlay()
    {
        gameSound.ClickSoundPlay();
    }

    public void onSoundOptionUI()
    {
        optionUi.SetActive(true);
    }

    public void offSoundOptionUI()
    {
        optionUi.SetActive(false);
    }
}
