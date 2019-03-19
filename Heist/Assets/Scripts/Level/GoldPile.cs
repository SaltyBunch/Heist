using System;
using System.Collections.Generic;
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
        private int _intitialAmount;
        [SerializeField] private int _transferAmount;
        [SerializeField] private int _totalRemaining;
        [SerializeField] private BarQuickTimeEvent _barQuickTimeEvent;


        [SerializeField] private List<GameObject> _goldPieces;

        public int TotalRemaining
        {
            get { return _totalRemaining; }
            set
            {
                _totalRemaining = value;
                for (int i = 0; i < _goldPieces.Count; i++)
                {
                    if (i / _goldPieces.Count < _totalRemaining / _intitialAmount)
                    {
                        _goldPieces[i].SetActive(false);
                    }
                    else
                    {
                        _goldPieces[i].SetActive(true);
                    }
                }
            }
        }

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
                TotalRemaining -= _transferAmount;
            }

            if (e.Complete || TotalRemaining <= 0 || e.State == -1)
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