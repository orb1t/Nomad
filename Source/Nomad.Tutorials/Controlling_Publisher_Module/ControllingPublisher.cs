﻿using System;
using Nomad.Communication.EventAggregation;
using Nomad.Modules;

namespace Controlling_Publisher_Module
{
    public class ControllingPublisher : IModuleBootstraper
    {
        private readonly IEventAggregator _eventAggregator;


        public ControllingPublisher(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        #region IModuleBootstraper Members

        public void Initialize()
        {
            _eventAggregator.Subscribe<CounterMessageType>(CheckCounter);
        }

        #endregion

        private void CheckCounter(CounterMessageType obj)
        {
            Console.WriteLine("Received counter message with: {0}", obj.Counter);
            if (obj.Counter >= 5)
            {
                _eventAggregator.Unsubsribe<CounterMessageType>(CheckCounter);
                Console.WriteLine("Unsubscribing from counter Events");
                TerminateCounterModule();
            }
        }


        private void TerminateCounterModule()
        {
            _eventAggregator.Publish(new StopPublishingMessageType("Counter reached desired number."));
        }

        #region Nested type: CounterMessageType

        private class CounterMessageType
        {
            public CounterMessageType(int counter)
            {
                Counter = counter;
            }


            public int Counter { get; private set; }
        }

        #endregion

        #region Nested type: StopPublishingMessageType

        private class StopPublishingMessageType
        {
            public readonly string Message;


            public StopPublishingMessageType(string message)
            {
                Message = message;
            }
        }

        #endregion
    }
}