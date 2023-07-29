using System.Collections.Generic;

using UnityEngine;

using static GameFramework.System.MessageRouter;

namespace GameFramework.System
{
    public static class MessageRouter
    {
        static Dictionary<string, MessageDelegate> Listeners = new Dictionary<string, MessageDelegate>();
        public delegate void MessageDelegate(string InChannel, object Data);

        public static ListenerHandle RegisterListener(string InChannel, MessageDelegate InDelegate)
        {
            if (!Listeners.ContainsKey(InChannel))
            {
                Listeners.Add(InChannel, InDelegate);
                return new ListenerHandle(InChannel, InDelegate);
            }

            Listeners[InChannel] += InDelegate;
            return new ListenerHandle(InChannel, InDelegate );
        }

        public static void UnregisterListener(ListenerHandle Handle)
        {
            if (!Listeners.ContainsKey(Handle.Channel)) return;
            Listeners[Handle.Channel] -= Handle.Delegate;
        }

        public static void BroadcastMessage(string InChannel, object Data)
        {
            if (!Listeners.ContainsKey(InChannel)) return;
            if (Listeners[InChannel] == null) return;

            List<MessageDelegate> InvalidListenerHandles = null;
            foreach(MessageDelegate Delegate in Listeners[InChannel].GetInvocationList())
            {
                if (Delegate.Target is MonoBehaviour)
                {
                    if ((MonoBehaviour)Delegate.Target == null)
                    {
                        if (InvalidListenerHandles == null) InvalidListenerHandles = new List<MessageDelegate>();
                        InvalidListenerHandles.Add(Delegate);
                        continue;
                    }
                }

                Delegate.Invoke(InChannel, Data);
            }

            if (InvalidListenerHandles == null) return;
            foreach (MessageDelegate InvalidListenerHandle in InvalidListenerHandles)
            {
                Listeners[InChannel] -= InvalidListenerHandle;
            }
        }
    }

    public class ListenerHandle
    {
        public ListenerHandle(string InChannel, MessageDelegate InDelegate)
        {
            Channel = InChannel;
            Delegate = InDelegate;
        }

        public readonly string Channel;
        public readonly MessageDelegate Delegate;
    }

    //test
    public struct MessageData
    {
        public string Name;
    }
}