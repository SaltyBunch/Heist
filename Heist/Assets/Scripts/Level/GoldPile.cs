using System;
using System.Collections.Generic;
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

        private bool _interacting;

        private BarQuickTimeEvent _quickTime;
        private int _initialAmount;
        [SerializeField] private int _transferAmount;
        [SerializeField] private int _totalRemaining;
        [SerializeField] private BarQuickTimeEvent _barQuickTimeEvent;


        [SerializeField] private List<GameObject> _goldPieces;

        [SerializeField, Range(0, 1)] public float Percentage;

        public float PercentageRemaining
        {
            get { return Percentage; }
            set
            {
                Percentage = value;
                for (int i = 0; i < _goldPieces.Count; i++)
                {
                    if ((float)i / _goldPieces.Count >= Percentage)
                    {
                        _goldPieces[i].SetActive(false);
                        if (i == 0) // there are no gold left
                        {
                            if (_quickTime != null)
                            {
                                _quickTime.Events -= QuickTimeEventMonitor;
                                Destroy(_quickTime.gameObject, 0.2f);
                            }

                            _quickTime = null;
                            if (_player != null)
                            {
                                _player.OnMoveCancel -= CancelChannel;
                                _player.QTEInteracting = false;
                            }
                            _player = null;
                            Destroy(gameObject, 0.2f);
                        }
                    }
                    else
                    {
                        _goldPieces[i].SetActive(true);
                    }
                }
            }
        }

        void Start()
        {
            _initialAmount = _totalRemaining;
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
            _player.QTEInteracting = true;
            _player.OnMoveCancel += CancelChannel;
        }

        public void CancelChannel(object sender, EventArgs e)
        {
            _interacting = false;
            _quickTime.Events -= QuickTimeEventMonitor;
            Destroy(_quickTime.gameObject, 0.2f);
            _quickTime = null;
            _player.OnMoveCancel -= CancelChannel;
            _player.QTEInteracting = false;
            _player = null;
        }

        private void QuickTimeEventMonitor(Object sender, QuickTimeEvent.QuickTimeEventArgs e)
        {
            if (e.Result)
            {
                _player.BaseCharacter.Inventory.GoldAmount += _transferAmount;
                _player.PickupGold();
                _totalRemaining -= _transferAmount;
            }

            PercentageRemaining = _totalRemaining / (float)_initialAmount;
            if (e.Complete || _totalRemaining <= 0 || e.State == -1)
            {
                _interacting = false;
                if (_quickTime != null)
                {
                    _quickTime.Events -= QuickTimeEventMonitor;
                    Destroy(_quickTime.gameObject, 0.2f);
                }
                _quickTime = null;
                if (_player != null)
                {
                    _player.OnMoveCancel -= CancelChannel;
                    _player.QTEInteracting = false;
                }
                _player = null;
            }
        }
    }
}