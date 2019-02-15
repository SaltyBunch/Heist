using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game
{
    public class QuickTimeEvent : MonoBehaviour
    {
        public delegate void QuickTimeEventHandler(Object sender, QuickTimeEventArgs e);

        public enum Type
        {
            GoldPile,
            Door,
            MiniVault
        }
        
        public class QuickTimeEventArgs : EventArgs
        {
            public LockQuickTimeEvent.Type Type;
            public int State;
            public bool Result;
            public bool Complete;
        }

        public enum Button
        {
            A,
            B,
            X,
            Y
        }
    }
}