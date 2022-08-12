using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using SolanaWalletAdapter.Scripts;
using UnityEngine;

public class JsEventDispatcher: MonoBehaviour
{
    private static readonly List<IJsEvent> _events = new List<IJsEvent>(10);

    public static T GetEvent<T>() where T: class, IJsEvent
    {
        var e = _events.FirstOrDefault(x => x.GetType() == typeof(T));
        
        if (e == null)
        {
            e = Activator.CreateInstance<T>();
            _events.Add(e);
        }
            
        return e as T;
    }
    

    public void OnHandleEvent(string argsStr)
    {
        // Debug.Log($"Received event: {argsStr}");
        
        var argsArr = JArray.Parse(argsStr);
        var eventName = argsArr[0].Value<string>();

        var eventObj = _events.FirstOrDefault(x => x.EventName == eventName);
        if (eventObj == null)
        {
            Debug.LogWarning($"JsEventDispatcher: OnHandleEvent(): event with name {eventName} not registered");
            return;
        }

        eventObj.HandleData(argsArr);
    }
}
