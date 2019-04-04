using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace UI
{
    public enum SelectionDirection
    {
        Horizontal,
        Vertical
    }


    public class SelectableMenu : MonoBehaviour
    {
        [SerializeField] private List<TextMeshPro> _buttons;
        [SerializeField] internal UnityEvent _cancel;
        [SerializeField] private int _selected;


        [SerializeField] internal SelectionDirection _selectionDirection;


        [SerializeField] private List<UnityEvent> _submit;

        public int Selected
        {
            get => _selected;
            set
            {
                if (_buttons != null && _buttons.Count > value)
                {
                    _buttons[_selected].color = Color.white;
                    _selected = value;
                    _buttons[_selected].color = Color.cyan / 2;
                }
                else
                {
                    _selected = value;
                }
            }
        }

        private void OnEnable()
        {
            Selected = 0;
        }

        public virtual void Right()
        {
            if (_selectionDirection == SelectionDirection.Horizontal && _buttons.Count > 0)
                Selected = ((Selected + 1) % _buttons.Count + _buttons.Count) % _buttons.Count;
        }

        public virtual void Left()
        {
            if (_selectionDirection == SelectionDirection.Horizontal && _buttons.Count > 0)
                Selected = ((Selected - 1) % _buttons.Count + _buttons.Count) % _buttons.Count;
        }

        public virtual void Up()
        {
            if (_selectionDirection == SelectionDirection.Vertical && _buttons.Count > 0)
                Selected = ((Selected - 1) % _buttons.Count + _buttons.Count) % _buttons.Count;
        }

        public virtual void Down()
        {
            if (_selectionDirection == SelectionDirection.Vertical && _buttons.Count > 0)
                Selected = ((Selected + 1) % _buttons.Count + _buttons.Count) % _buttons.Count;
        }

        public virtual void Submit()
        {
            _submit[Selected]?.Invoke();
        }

        public virtual void Cancel()
        {
            _cancel?.Invoke();
        }

        public virtual void Activate()
        {
            Selected = 0;
        }
    }
}