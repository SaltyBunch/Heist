using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game
{
    public class QuickTimeEvent : MonoBehaviour
    {
        public delegate void QuickTimeEventHandler(Object sender, QuickTimeEventArgs e);

        public enum Button
        {
            A,
            B,
            X,
            Y
        }

        public enum Type
        {
            GoldPile,
            Door,
            MiniVault
        }

        public class QuickTimeEventArgs : EventArgs
        {
            public bool Complete;
            public bool Result;
            public int State;
            public Type Type;
        }
    }
}