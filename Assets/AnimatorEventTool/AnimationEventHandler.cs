using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventHandler : MonoBehaviour
{
    enum AnimationEventType
    {
        None = 0,
        PlayEffect = 1,
        OpenObject = 2,
        CloseObject = 3,
    }
    void OnAnimationEvent(AnimationEvent animationEvent)
    {
        Debug.Log($"[OnAnimationEvent] {animationEvent.stringParameter}, {animationEvent.intParameter}, {animationEvent.floatParameter}, {animationEvent.functionName}");

        // animationEvent.stringParameter : event name to database
        // animationEvent.intParameter : database id

        switch ((AnimationEventType)animationEvent.intParameter)
        {
            case AnimationEventType.PlayEffect:
                playEffect(animationEvent.stringParameter);
                break;
        }
    }

    void playEffect(string info)
    {

    }
}
