using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class GameSound : MonoBehaviour
{
    SoundManager _soundMgr;

    private void Start()
    {
        _soundMgr = GameObject.Find("SoundMgr").gameObject.GetComponent<SoundManager>();
        gameObject.GetComponent<AudioSource>().clip = _soundMgr.soundList[2];
    }

    public void ClickSoundPlay()
    {
        gameObject.GetComponent<AudioSource>().Play();
    }
}
