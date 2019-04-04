using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;

namespace UI
{
    public class AudioSelectableMenu : SelectableMenu
    {
        [SerializeField] private List<WorldSpaceSlider> _sliders;

        [SerializeField] private UnityAction _actions;

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
                Selected = (((Selected - 1) % _sliders.Count) + _sliders.Count) % _sliders.Count;
                _sliders[Selected].Selected = true;
            }
        }

        public override void Down()
        {
            if (_sliders.Count > 0)
            {
                _sliders[Selected].Selected = false;
                Selected = (((Selected + 1) % _sliders.Count) + _sliders.Count) % _sliders.Count;
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