﻿using System;
using Engine.EventArgs;

namespace Engine.Services
{
    public class MessageBroker
    {
        private static readonly MessageBroker s_messageBroker = new MessageBroker();

        private MessageBroker()
        {
        }

        public event EventHandler<GameMessageEventArgs> OnMessageRaised;

        public static MessageBroker GetInstance() => s_messageBroker;

        internal void RaiseMessage(string message)
        {
            OnMessageRaised?.Invoke(this, new GameMessageEventArgs(message));
        }
    }
}