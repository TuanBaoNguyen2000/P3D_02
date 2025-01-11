using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class AnimationEventMN : MonoBehaviour
{
    public Action OnAnimationBegin;
    public Action OnAnimationFinish;
    public Action<string> OnCustomEvent;

    private void OnTriggerBeginAnim()
    {
        OnAnimationBegin?.Invoke();
    }

    private void OnTriggerFinishAnim()
    {
        OnAnimationFinish?.Invoke();
    }

    private void OnTriggerCustomEvent(string eventName)
    {
        OnCustomEvent?.Invoke(eventName);
    }
}
