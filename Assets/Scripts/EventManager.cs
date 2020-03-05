using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    private static EventManager Instance;

    public enum GameEvent { SelectGuardian, NotClickedTerms, CharacterCreated, OpenMap, OpenDilemma, OpenCharacterCreation, ValueChange, DilemmaOpen, GuidingCharacterHelp, FamUnhappyDecision, FriUnhappyDecision, EduUnhappyDecision, GoodChoice, NotMovingEgg, RunningGameStart, QuizFam, QuizFriends, QuizEducation, CloseDilemma, FirstMapDrag }
    public class GameEventTrigger : UnityEvent<GameEvent> { }
    public static GameEventTrigger OnGameEvent = new GameEventTrigger();

    void Awake()
    {
        if (!Instance)
        {
            Instance = this;

            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }

    }

#if UNITY_EDITOR
    private static List<T> GetAssetsOfType<T>() where T : UnityEngine.ScriptableObject
    {
        var guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);  //FindAssets uses tags check documentation for more info
        return guids.Select(guid => AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid))).ToList();

    }
#endif
}
