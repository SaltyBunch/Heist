using Character;
using Game;

namespace Level
{
    public class StairDoor : Door
    {
        private bool _opened;

        public bool Opened
        {
            get { return _opened; }
            set
            {
                if (value)
                {
                    _light.color = _unlockedColor;
                }
                else
                {
                    _light.color = _lockedColor;
                }

                _opened = value;
            }
        }

        private void Start()
        {
            Opened = false;
        }

        public override void Open(PlayerControl player)
        {
            if (player.BaseCharacter.Inventory.keys[KeyType.BlueKey])
            {
                if (!Opened)
                {
                    LevelManager.LevelManagerRef.OpenDoor();
                    Opened = true;
                }
            }

            if (Opened)
            {
                if (!_animating)
                {
                    if (!_open)
                    {
                        _animating = true;
                        _anim.SetTrigger("Open");
                    }
                    else
                    {
                        _animating = true;
                        _anim.SetTrigger("Close");
                    }
                }
            }
        }
        
        void SetOpen()
        {
            _animating = false;
            _open = true;
        }

        void SetClose()
        {
            _animating = false;
            _open = false;
        }
    }
}