using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameSound : MonoBehaviour
{
    SoundManager _soundMgr;
    GameSound gameSound;


    void Start()
    {
        gameSound = GameObject.Find("Click").GetComponent<GameSound>();
        _soundMgr = GameObject.Find("SoundMgr").gameObject.GetComponent<SoundManager>();
        _soundMgr.GetComponent<AudioSource>().clip = _soundMgr.soundList[0];
        _soundMgr.GetComponent<AudioSource>().Play();
    }


    private void OnDisable()
    {
        _soundMgr.GetComponent<AudioSource>().clip = _soundMgr.soundList[1];
        _soundMgr.GetComponent<AudioSource>().Play();
    }

    public void onClickServerBtnSoundPlay()
    {
        gameSound.ClickSoundPlay();
    }

}
