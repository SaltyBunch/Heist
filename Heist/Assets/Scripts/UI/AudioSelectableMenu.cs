using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UI
{
    public class AudioSelectableMenu : SelectableMenu
    {
        [SerializeField] private UnityAction _actions;
        [SerializeField] private List<WorldSpaceSlider> _sliders;

        public override void Right()
        {
            _sliders[Selected].Value += _sliders[Selected].Step;
        }

        public override void Left()
        {
            _sliders[Selected].Value -= _sliders[Selected].Step;
        }

        public override void Up()
        {
            if (_sliders.Count > 0)
            {
                _sliders[Selected].Selected = false;
                Selected = ((Selected - 1) % _sliders.Count + _sliders.Count) % _sliders.Count;
                _sliders[Selected].Selected = true;
            }
        }

        public override void Down()
        {
            if (_sliders.Count > 0)
            {
                _sliders[Selected].Selected = false;
                Selected = ((Selected + 1) % _sliders.Count + _sliders.Count) % _sliders.Count;
                _sliders[Selected].Selected = true;
            }
        }

        public override void Cancel()
        {
            _cancel?.Invoke();
        }

        public override void Activate()
        {
            Selected = 0;
            _sliders[Selected].Selected = true;
        }
    }
}