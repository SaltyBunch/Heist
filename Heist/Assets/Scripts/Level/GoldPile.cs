using System;
using Character;
using Game;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace Level
{
    [RequireComponent(typeof(Collider))]
    public class GoldPile : MonoBehaviour
    {
        private PlayerControl _player;

        private bool _interacting;
        
        private BarQuickTimeEvent _quickTime;
        [SerializeField] private int _transferAmount;
        [SerializeField] private int _totalRemaining;
        [SerializeField] private BarQuickTimeEvent _barQuickTimeEvent;

        public void StartChanneling(PlayerControl player)
        {
            if (_interacting) return;
            _interacting = true;
            _quickTime = player.BaseCharacter.InitializeQuickTime(_barQuickTimeEvent) as BarQuickTimeEvent;
            _quickTime.QuickTimeType = QuickTimeEvent.Type.GoldPile;
            _quickTime.Events += QuickTimeEventMonitor;
            _quickTime.SetDexterity(player.BaseCharacter.Stats.Dexterity);
            _quickTime.Generate(player);
            _player = player;
            _player.OnMoveCancel += CancelChannel;
        }

        public void CancelChannel(object sender, EventArgs e)
        {
            _interacting = false;
            _quickTime.Events -= QuickTimeEventMonitor;
            Destroy(_quickTime.gameObject, 0.2f);
            _quickTime = null;
            _player.OnMoveCancel -= CancelChannel;
            _player = null;
        }

        private void QuickTimeEventMonitor(Object sender, QuickTimeEvent.QuickTimeEventArgs e)
        {
            if (e.Result)
            {
                _player.BaseCharacter.Inventory.GoldAmount += _transferAmount;
                _totalRemaining -= _transferAmount;
            }

            if (e.Complete || _totalRemaining <= 0)
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