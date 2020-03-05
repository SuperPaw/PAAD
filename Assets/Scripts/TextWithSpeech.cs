using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public class TextWithSpeech : ScriptableObject
{
    [SerializeField]
    [TextArea]
    private string Text;
    [SerializeField]
    private AudioClip Audio;

    public AudioClip GetAudioClip() => Audio;
    public float GetAudioLength() => GetAudioClip() ? GetAudioClip().length : 0f;

    public string GetText() => Text;


}