using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class TutorialMessage : ScriptableObject
{
    public EventManager.GameEvent EventTrigger;
    [Header("The lines the guiding character will say, in order")]
    public TutorialLine[] Lines;
    public bool SelectOneAtRandom;
    public bool Recurring;
    [Range(0f, 1f)]
    public float Chance = 1f;
    public bool LockOtherInteraction = true;
    public MonaLisa.GuideAnchorPos[] AnchorPositions;

    [Serializable]
    public struct TutorialLine
    {
        public TextWithSpeech Line;
        //TODO: should be enum or actual object
        public string UIObject;
    }
}
