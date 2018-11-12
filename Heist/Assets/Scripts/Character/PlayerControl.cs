using UnityEngine;

namespace Character
{

    internal struct Control
    {
        internal bool Dash;
        internal bool MeleeAttack;

        internal bool Interact;
    }

    [RequireComponent(typeof(Player))]
    public class PlayerControl : MonoBehaviour
    {

        [SerializeField] private Player _basePlayer;

        private void Start()
        {
            if (_basePlayer == null)
                _basePlayer = GetComponent<Player>();
        }

        private Control _control;

        internal Control Control
        {
            get { return _control; }
            set
            {
                if (!Equals(value, _control))
                {
                    if (value.Dash && !_control.Dash)
                        Dash();
                    if (value.MeleeAttack && !_control.MeleeAttack)
                        MeleeAttack();
                }
                _control = value;
            }
        }

        private void MeleeAttack()
        {
            throw new System.NotImplementedException();
        }

        private void Dash()
        {
            throw new System.NotImplementedException();
        }
    }
}