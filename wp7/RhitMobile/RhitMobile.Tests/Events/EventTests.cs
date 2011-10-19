using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Phone.Controls.Maps;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RhitMobile.Events;
using RhitMobile.Services;

namespace RhitMobile.Tests.Events {
    [TestClass]
    public class EventTests {
        private event DebugEventHandler DebugEvent;
        private event OutlineEventHandler OutlineEvent;
        private event PushpinEventHandler PushpinEvent;
        private event ServerEventHandler ServerEvent;

        [TestInitialize]
        public void SetUp() {
            if(Events == null) Events = new List<EventArgs>();
            DebugEvent += new DebugEventHandler(EventTests_handler);
            OutlineEvent += new OutlineEventHandler(EventTests_handler);
            PushpinEvent += new PushpinEventHandler(EventTests_handler);
            ServerEvent += new ServerEventHandler(EventTests_handler);
        }

        [TestCleanup]
        public void CleanUp() {
            foreach(Delegate d in DebugEvent.GetInvocationList())
                DebugEvent -= (DebugEventHandler) d;
            foreach(Delegate d in OutlineEvent.GetInvocationList())
                OutlineEvent -= (OutlineEventHandler) d;
            foreach(Delegate d in PushpinEvent.GetInvocationList())
                PushpinEvent -= (PushpinEventHandler) d;
            foreach(Delegate d in ServerEvent.GetInvocationList())
                ServerEvent -= (ServerEventHandler) d;
            Events.Clear();
        }

        void EventTests_handler(object sender, EventArgs e) {
            Events.Add(e);
        }

        public List<EventArgs> Events { get; set; }

        [TestMethod]
        [Description("Tests basic functionality of DebugEventArgs.")]
        public void DebugEventTest() {
            DebugEvent(this, new DebugEventArgs());
            Assert.IsTrue(Events.Count > 0, "No event was caught.");
            Assert.IsTrue(Events.Count == 1, "More than one event was caught.");
            Assert.IsTrue(Events[0] is DebugEventArgs, String.Format("An event with arggument of type {0} was caught when DebugEventArgs was expected.", Events[0].GetType().ToString()));
        }

        [TestMethod]
        [Description("Tests basic functionality of OutlineEventArgs.")]
        public void OutlineEventTest() {
            MapPolygon outline = new MapPolygon();
            OutlineEvent(this, new OutlineEventArgs() {
                Outline = outline,
            });

            Assert.IsTrue(Events.Count > 0, "No event was caught.");
            Assert.IsTrue(Events.Count == 1, "More than one event was caught.");
            Assert.IsTrue(Events[0] is OutlineEventArgs, String.Format("An event with arggument of type {0} was caught when OutlineEventArgs was expected.", Events[0].GetType().ToString()));
            Assert.IsTrue(((OutlineEventArgs) Events[0]).Outline == outline, "The outline received was not equivalent to the one sent.");
        }

        [TestMethod]
        [Description("Tests basic functionality of PushpinEventArgs.")]
        public void PushpinEventTest() {
            Pushpin pushpin = new Pushpin();
            PushpinEvent(this, new PushpinEventArgs() {
                SelectedPushpin = pushpin,
            });

            Assert.IsTrue(Events.Count > 0, "No event was caught.");
            Assert.IsTrue(Events.Count == 1, "More than one event was caught.");
            Assert.IsTrue(Events[0] is PushpinEventArgs, String.Format("An event with arggument of type {0} was caught when PushpinEventArgs was expected.", Events[0].GetType().ToString()));
            Assert.IsTrue(((PushpinEventArgs) Events[0]).SelectedPushpin == pushpin, "The selected pushpin received was not equivalent to the one sent.");
        }

        [TestMethod]
        [Description("Tests basic functionality of ServerEventArgs.")]
        public void ServerEventTest() {
            HttpStatusCode response = new HttpStatusCode();
            ServerObject serverObject = new ServerObject();
            ServerEvent(this, new ServerEventArgs() {
                ServerResponse = response,
                ResponseObject = serverObject,
            });

            Assert.IsTrue(Events.Count > 0, "No event was caught.");
            Assert.IsTrue(Events.Count == 1, "More than one event was caught.");
            Assert.IsTrue(Events[0] is ServerEventArgs, String.Format("An event with arggument of type {0} was caught when ServerEventArgs was expected.", Events[0].GetType().ToString()));
            Assert.IsTrue(((ServerEventArgs) Events[0]).ServerResponse == response, "The server response received was not equivalent to the one sent.");
            Assert.IsTrue(((ServerEventArgs) Events[0]).ResponseObject == serverObject, "The server object received was not equivalent to the one sent.");
        }
    }
}
