using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverUIScript : MonoBehaviour
{
    AudioSource _audioSouce;
    SoundManager _soundMgr;

    void Start()
    {
        _audioSouce = GameObject.Find("SoundMgr").transform.GetChild(1).gameObject.GetComponent<AudioSource>();
        _soundMgr = GameObject.Find("SoundMgr").gameObject.GetComponent<SoundManager>();
        _audioSouce.GetComponent<AudioSource>().clip = _soundMgr.soundList[3];
        _audioSouce.GetComponent<AudioSource>().Play();
    }
}
