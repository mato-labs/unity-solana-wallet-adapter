using System;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace SolanaWalletAdapter.Scripts
{
    public interface IJsEvent
    {
        int ArgsCount { get; }
        
        string EventName { get; }
        
        void HandleData(JArray args);
    }

    public class JsEvent: IJsEvent
    {
        public Action OnEvent = () => { };
        
        public int ArgsCount => 0;
        public string EventName { get; }

        public JsEvent(string eventName)
        {
            EventName = eventName;
        }
        
        public void HandleData(JArray args)
        {
            OnEvent.Invoke();
        }
    }
    
    public class JsEvent<T>: IJsEvent
    {
        public Action<T> OnEvent = obj => { };
        
        public int ArgsCount => 1;
        
        public string EventName { get; }
        
        public JsEvent(string eventName)
        {
            EventName = eventName;
        }

        public void HandleData(JArray args)
        {
            object arg1;

            if (args[1] is JObject)
            {
                arg1 = args[1].ToObject<T>();
            }
            else
            {
                arg1 = args[1].Value<T>();
            }

            OnEvent.Invoke((T)arg1);
        }
    }
    
    public class JsEvent<T1, T2>: IJsEvent
    {
        public Action<T1, T2> OnEvent = (a1, a2) => { };
        
        public int ArgsCount => 2;
        
        public string EventName { get; }
        
        public JsEvent(string eventName)
        {
            EventName = eventName;
        }

        public void HandleData(JArray args)
        {
            object[] argsArr = new object[ArgsCount];

            for (int i = 0; i < argsArr.Length; i++)
            {
                var argIndex = i + 1;
                
                if (args[argIndex] is JObject)
                {
                    argsArr[i] = args[argIndex].ToObject<T1>();
                }
                else
                {
                    argsArr[i] = args[argIndex].Value<T1>();
                }
            }

            OnEvent.Invoke((T1)argsArr[0], (T2)argsArr[1]);
        }
    }
    
    public class JsEvent<T1, T2, T3>: IJsEvent
    {
        public Action<T1, T2, T3> OnEvent = (a1, a2, a3) => { };
        
        public int ArgsCount => 3;
        
        public string EventName { get; }
        
        public JsEvent(string eventName)
        {
            EventName = eventName;
        }

        public void HandleData(JArray args)
        {
            object[] argsArr = new object[ArgsCount];

            for (int i = 0; i < argsArr.Length; i++)
            {
                var argIndex = i + 1;
                
                if (args[argIndex] is JObject)
                {
                    argsArr[i] = args[argIndex].ToObject<T1>();
                }
                else
                {
                    argsArr[i] = args[argIndex].Value<T1>();
                }
            }

            OnEvent.Invoke((T1)argsArr[0], (T2)argsArr[1], (T3)argsArr[2]);
        }
    }
    
    public class JsEvent<T1, T2, T3, T4>: IJsEvent
    {
        public Action<T1, T2, T3, T4> OnEvent = (a1, a2, a3, a4) => { };
        
        public int ArgsCount => 4;
        
        public string EventName { get; }
        
        public JsEvent(string eventName)
        {
            EventName = eventName;
        }

        public void HandleData(JArray args)
        {
            object[] argsArr = new object[ArgsCount];

            for (int i = 0; i < argsArr.Length; i++)
            {
                var argIndex = i + 1;
                
                if (args[argIndex] is JObject)
                {
                    argsArr[i] = args[argIndex].ToObject<T1>();
                }
                else
                {
                    argsArr[i] = args[argIndex].Value<T1>();
                }
            }

            OnEvent.Invoke((T1)argsArr[0], (T2)argsArr[1], (T3)argsArr[2], (T4)argsArr[3]);
        }
    }
}