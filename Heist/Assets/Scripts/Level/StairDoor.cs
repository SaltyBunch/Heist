using Character;
using Game;

namespace Level
{
    public class StairDoor : Door
    {
        private bool _opened;

        public override void Open(PlayerControl player)
        {
            if (player.BaseCharacter.Inventory.keys[KeyType.BlueKey])
            {
                if (!_opened)
                {
                    LevelManager.LevelManagerRef.OpenDoor();
                    _opened = true;
                }
            }

            if (_opened)
            {
                _open = !_open;
                if (_open)
                    _anim.SetTrigger("Open");
                else
                    _anim.SetTrigger("Close");
            }
        }
    }
}