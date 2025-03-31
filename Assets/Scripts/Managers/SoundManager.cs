using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.IO;

[System.Serializable]
public class SoundData
{
    public float master;
    public float bgm;
    public float sfx;
}

public class SoundManager : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider audioSlider1;
    public Slider audioSlider2;
    public Slider audioSlider3;
    public List<AudioClip> soundList;

    //json
    SoundManager instance;

    public SoundData soundData;

    private string path;
    private string fileName = "/save";

    private void Awake()
    {
        //È¤½Ã¸ô¶ó¼­
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    private void Start()
    {
        path = Application.persistentDataPath + fileName;
        Debug.Log(path);
        LoadData();
        audioSlider1.value = soundData.master;
        audioSlider2.value = soundData.bgm;
        audioSlider3.value = soundData.sfx;

        gameObject.GetComponent<AudioSource>().clip = soundList[0];
        gameObject.GetComponent<AudioSource>().Play();
    }

    void SaveData()
    {
        string data = JsonUtility.ToJson(soundData);
        File.WriteAllText(path, data);
    }
    void LoadData()
    {
        if (File.Exists(path) == false)
        {
            SaveData();
        }
        string data = File.ReadAllText(path);
        soundData = JsonUtility.FromJson<SoundData>(data);
    }

    public void MasterAudioControl()
    {
        float volum = audioSlider1.value;
        soundData.master = volum;
        SaveData();
        if (volum == -40)
        {
            audioMixer.SetFloat("Master", -80f);
        }
        else
        {
            audioMixer.SetFloat("Master", volum);
        }
    }

    public void BGMAudioControl()
    {
        float volum = audioSlider2.value;
        soundData.bgm = volum;
        SaveData();
        if (volum == -40)
        {
            audioMixer.SetFloat("BGM", -80f);
        }
        else
        {
            audioMixer.SetFloat("BGM", volum);
        }
    }

    public void SFXAudioControl()
    {
        float volum = audioSlider3.value;
        soundData.sfx = volum;
        SaveData();
        if (volum == -40)
        {
            audioMixer.SetFloat("SFX", -80f);
        }
        else
        {
            audioMixer.SetFloat("SFX", volum);
        }
    }
}
