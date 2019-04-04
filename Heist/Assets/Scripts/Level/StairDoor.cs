using Character;
using Game;
using UI;

namespace Level
{
    public class StairDoor : Door
    {
        private bool _opened;

        public bool Opened
        {
            get => _opened;
            set
            {
                if (value)
                    _light.color = _unlockedColor;
                else
                    _light.color = _lockedColor;

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
                    LevelManager.LevelManagerRef.NotifyPlayers(TextHelper.StairUnlocked);
                }
            }
            else if (!Opened)
            {
                player.BaseCharacter.PlayerUiManager.NeedKey(KeyType.BlueKey);
            }

            if (Opened)
                if (!_animating)
                {
                    if (!_open)
                    {
                        _animating = true;
                        _anim.SetTrigger("Open");
                        if (_openClip != null)
                        {
                            _audioSource.clip = _openClip;
                            _audioSource.Play();
                        }
                    }
                    else
                    {
                        _animating = true;
                        _anim.SetTrigger("Close");
                        if (_closeClip != null)
                        {
                            _audioSource.clip = _closeClip;
                            _audioSource.Play();
                        }
                    }
                }
        }

        private void SetOpen()
        {
            _animating = false;
            _open = true;
        }

        private void SetClose()
        {
            _animating = false;
            _open = false;
        }
    }
}