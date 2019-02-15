using System;
using Character;
using Game;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Level
{
    [RequireComponent(typeof(Collider))]
    public class GoldPile : MonoBehaviour
    {
        private PlayerControl _player;

        private BarQuickTimeEvent _quickTime;
        [SerializeField] private int _transferAmount;
        [SerializeField] private BarQuickTimeEvent barQuickTimeEvent;

        public void StartChanneling(PlayerControl player)
        {
            _quickTime = Instantiate(barQuickTimeEvent);
            _quickTime.QuickTimeType = QuickTimeEvent.Type.GoldPile;
            _quickTime.Events += QuickTimeEventMonitor;
            _player = player;
            _player.OnMoveCancel += CancelChannel;
        }

        public void CancelChannel(object sender, EventArgs e)
        {
            _quickTime.Events -= QuickTimeEventMonitor;
            Destroy(_quickTime.gameObject, 0.2f);
            _quickTime = null;
            _player.OnMoveCancel -= CancelChannel;
            _player = null;
        }

        private void QuickTimeEventMonitor(Object sender, QuickTimeEvent.QuickTimeEventArgs e)
        {
            if (e.Result) _player.BaseCharacter.Inventory.GoldAmount += _transferAmount;

            if (e.Complete)
            {
                _quickTime.Events -= QuickTimeEventMonitor;
                Destroy(_quickTime.gameObject, 0.2f);
                _quickTime = null;
                _player.OnMoveCancel -= CancelChannel;
                _player = null;
            }
        }
    }
}