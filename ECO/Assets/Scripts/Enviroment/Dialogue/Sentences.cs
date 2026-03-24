using UnityEngine;

[System.Serializable]
public class Sentences
{
    public AudioClip voice;
    [TextArea(3, 10)]
    public string sentence;


    public override string ToString()
    {
        return sentence;
    }

    public AudioClip GetVoice()
    {
        return voice;
    }
}
