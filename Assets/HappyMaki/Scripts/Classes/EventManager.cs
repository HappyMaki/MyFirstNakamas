using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager
{
    public static OnServerDiscovery onServerDiscovery = new OnServerDiscovery();
    public static OnAccountCreation onAccountCreation = new OnAccountCreation();
    private EventManager() { }

    public static IEnumerator DelayInvoke(float delay, UnityEvent e)
    {
        yield return new WaitForSeconds(delay);
        e.Invoke();
    }

    public static IEnumerator DelayInvoke(float delay, OnAccountCreation e, AccountCreationResolution resolution)
    {
        yield return new WaitForSeconds(delay);
        e.Invoke(resolution);
    }
}
