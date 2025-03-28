using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LodingScript : MonoBehaviour
{
    public Sprite[] frames; // GIF 분할 프레임들
    public float frameRate = 0.1f;
    private Image image;
    private int currentFrame;

    void Start()
    {
        image = GetComponent<Image>();
        StartCoroutine(PlayGif());
    }

    IEnumerator PlayGif()
    {
        while (true)
        {
            image.sprite = frames[currentFrame];
            currentFrame = (currentFrame + 1) % frames.Length;
            yield return new WaitForSeconds(frameRate);
        }
    }

}