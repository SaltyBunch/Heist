using System;
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


        [SerializeField] private SelectionDirection _selectionDirection;
        [SerializeField] private int _selected;


        [SerializeField] private List<UnityEvent> _submit;
        [SerializeField] private UnityEvent _cancel;

        public int Selected
        {
            get { return _selected; }
            set
            {
                _buttons[_selected].color = Color.white;
                _selected = value;
                _buttons[_selected].color = Color.cyan / 2;
            }
        }

        private void OnEnable()
        {
            Selected = 0;
        }

        public void Right()
        {
            if (_selectionDirection == SelectionDirection.Horizontal)
                Selected = (((Selected + 1) % _buttons.Count) + _buttons.Count) % _buttons.Count;
        }

        public void Left()
        {
            if (_selectionDirection == SelectionDirection.Horizontal)
                Selected = (((Selected - 1) % _buttons.Count) + _buttons.Count) % _buttons.Count;
        }

        public void Up()
        {
            if (_selectionDirection == SelectionDirection.Vertical)
                Selected = (((Selected - 1) % _buttons.Count) + _buttons.Count) % _buttons.Count;
        }

        public void Down()
        {
            if (_selectionDirection == SelectionDirection.Vertical)
                Selected = (((Selected + 1) % _buttons.Count) + _buttons.Count) % _buttons.Count;
        }

        public void Submit()
        {
            _submit[Selected].Invoke();
        }

        public void Cancel()
        {
            _cancel.Invoke();
        }
    }
}