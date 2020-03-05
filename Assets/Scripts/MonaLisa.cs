using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MonaLisa : MonoBehaviour
{
    private static MonaLisa Instance;
    public Button Button;
    public Animation Animation;
    public List<TutorialMessage> TutorialMessages;
    public Image GuidingCharacterImage;
    //public ImageFade BackgroundTintImage;
    public TextMeshProUGUI Text;
    public GameObject Holder;
    public ParticleSystem PuffParticles;
    public float DisplayTime = 4f;
    private EventManager.GameEvent CurrentEvent;
    public RectTransform CharacterPositionHolder, LeftAnchorPos, RightAnchorPos, MiddleAnchorPos;

    private Queue<TutorialMessage> MessageQueue = new Queue<TutorialMessage>();

    public enum GuideAnchorPos { Left, Middle, Right }

    private void Start()
    {
        if (!Instance) Instance = this;
        else
        {
            Destroy(this.gameObject);
            return;
        }

        EventManager.OnGameEvent.AddListener(HandleGameEvent);
    }

    private void HandleGameEvent(EventManager.GameEvent gameEvent)
    {
        if (TutorialMessages.Any(m => m.EventTrigger == gameEvent))
        {
            var m = TutorialMessages.First(mes => mes.EventTrigger == gameEvent);

            if (Holder.activeInHierarchy)
                MessageQueue.Enqueue(m);
            else
                StartCoroutine(DisplayMessageRoutine(m));
        } //slight hack to control that clickable advice stays and closes when the dilemma screen does
        else if (gameEvent == EventManager.GameEvent.CloseDilemma)
        {
            if (CurrentEvent == EventManager.GameEvent.GuidingCharacterHelp)
            {
                if (MessageQueue.Any())
                    StartCoroutine(DisplayMessageRoutine(MessageQueue.Dequeue()));
                else
                    Close();
            }

            if (MessageQueue.Any(e => e.EventTrigger == EventManager.GameEvent.GuidingCharacterHelp))
            {
                MessageQueue = new Queue<TutorialMessage>(MessageQueue.Where(e => e.EventTrigger != EventManager.GameEvent.GuidingCharacterHelp));
            }
        }
    }

    private void Close()
    {
        Holder.SetActive(false);
        PuffParticles.Play();
        SoundController.PlaySound(SoundController.UISound.GuidePopup);
    }

    public void StopBlinking()
    {
        Animation.Stop();
        Animation.clip.SampleAnimation(Animation.gameObject, 0f);
    }

    private IEnumerator DisplayMessageRoutine(TutorialMessage tutorial)
    {
        if (!tutorial.Recurring)
            TutorialMessages.Remove(tutorial);

        CurrentEvent = tutorial.EventTrigger;

        if (tutorial.EventTrigger == EventManager.GameEvent.GuidingCharacterHelp)
        {
            Button.interactable = true;
            Animation.Play();
        }
        else
        {
            Button.interactable = false;
            StopBlinking();
        }

        //if (tutorial.LockOtherInteraction)
        //    BackgroundTintImage.FadeIn();

        if (tutorial.AnchorPositions.Any())
            StartCoroutine(MoveCharacterToAnchorPos(CharacterPositionHolder, tutorial.AnchorPositions[Random.Range(0, tutorial.AnchorPositions.Length)], !Holder.activeSelf));

        if (!Holder.activeSelf)
        {
            SoundController.PlaySound(SoundController.UISound.GuidePopup);
            PuffParticles.Play();
            Holder.SetActive(true);
        }

        if (tutorial.SelectOneAtRandom)
        {
            Text.text = "";
            yield return new WaitUntil(() => !SoundController.SpeechPlaying());
            yield return new WaitForSecondsRealtime(PlayLine(tutorial.Lines[Random.Range(0, tutorial.Lines.Length)].Line) + DisplayTime);
        }
        else
        {
            foreach (var t in tutorial.Lines)
            {
                Text.text = "";
                yield return new WaitUntil(() => !SoundController.SpeechPlaying());
                yield return new WaitForSecondsRealtime(PlayLine(t.Line) + DisplayTime);
            }
        }
        
        //if (tutorial.LockOtherInteraction)
        //    BackgroundTintImage.FadeOut();

        if (MessageQueue.Any())
            StartCoroutine(DisplayMessageRoutine(MessageQueue.Dequeue()));
        else if (tutorial.EventTrigger != EventManager.GameEvent.GuidingCharacterHelp)
        {
            Close();
        }
    }

    private IEnumerator MoveCharacterToAnchorPos(RectTransform rectTransform, GuideAnchorPos endPos, bool instantly = false)
    {
        var moveTime = instantly ? 0f : 3f;
        var endTime = Time.time + moveTime;

        Vector2 endposition = rectTransform.position;

        switch (endPos)
        {
            case GuideAnchorPos.Left:
                endposition = LeftAnchorPos.position;
                break;
            case GuideAnchorPos.Middle:
                endposition = MiddleAnchorPos.position;
                break;
            case GuideAnchorPos.Right:
                endposition = RightAnchorPos.position;
                break;
        }

        while (Time.time < endTime)
        {
            //lerp with current position to give fluid movement
            rectTransform.position = Vector2.Lerp(rectTransform.position, endposition, 1 - (endTime - Time.time) / moveTime);

            yield return null;
        }
        rectTransform.position = endposition;
    }

    private float PlayLine(TextWithSpeech textWithSpeech)
    {
        Text.text = textWithSpeech.GetText();

        SoundController.PlaySpeech(textWithSpeech);

        return textWithSpeech.GetAudioLength();
    }
}
