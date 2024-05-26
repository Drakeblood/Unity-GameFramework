using System;
using System.Collections.Generic;

using UnityEngine;

using static GameFramework.System.MessageRouter;

namespace GameFramework.System
{
    public static class MessageRouter
    {
        private static readonly Dictionary<string, MessageDelegate> listeners = new();
        public delegate void MessageDelegate(string channel, object data);

        public static ListenerHandle RegisterListener(string channel, MessageDelegate @delegate)
        {
            if (!listeners.ContainsKey(channel))
            {
                listeners.Add(channel, @delegate);
                return new ListenerHandle(channel, @delegate);
            }

            listeners[channel] += @delegate;
            return new ListenerHandle(channel, @delegate );
        }

        public static void UnregisterListener(ListenerHandle handle)
        {
            if (!listeners.ContainsKey(handle.Channel)) return;
            listeners[handle.Channel] -= handle.Delegate;
        }

        public static void BroadcastMessage(string channel, object data)
        {
            if (!listeners.ContainsKey(channel)) return;
            if (listeners[channel] == null) return;

            List<MessageDelegate> invalidListenerHandles = null;
            Delegate[] delegates = listeners[channel].GetInvocationList();

            for (int i = 0; i < delegates.Length; i++)
            {
                if (delegates[i] is not MessageDelegate @delegate) continue;

                if (@delegate.Target is MonoBehaviour behaviour)
                {
                    if (behaviour == null)
                    {
                        invalidListenerHandles ??= new List<MessageDelegate>();
                        invalidListenerHandles.Add(@delegate);
                        continue;
                    }
                }

                @delegate.Invoke(channel, data);
            }

            if (invalidListenerHandles == null) return;

            for (int i = 0; i < invalidListenerHandles.Count; i++)
            {
                listeners[channel] -= invalidListenerHandles[i];
            }
        }
    }

    public class ListenerHandle
    {
        public ListenerHandle(string channel, MessageDelegate @delegate)
        {
            Channel = channel;
            Delegate = @delegate;
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