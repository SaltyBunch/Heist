using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PlayerUIManager : MonoBehaviour
    {
        [SerializeField] private TextMeshPro _gold;
        [SerializeField] private Image[] _health;
        [SerializeField] private Image _playerPortrait;

        [SerializeField] private Transform _quickTimePosition;

        [SerializeField] private RectTransform _uiLoction;

        public void SetPosition(Rect rect)
        {
            var pos = new Rect()
            {
                x = 0, y = 0
            };

            //todo
            //todo
            //todo
            if (Math.Abs(rect.y) > 0.01f) //put bottom
            {
                if (Math.Abs(rect.x) > 0.01f) //put left
                {
                    //bottom left
                    _uiLoction.anchorMax = Vector2.zero;
                    _uiLoction.anchorMin = Vector2.zero;
                }
                else
                {
                    //bottom right
                    _uiLoction.anchorMax = Vector2.right;
                    _uiLoction.anchorMin = Vector2.right;
                }
            }
            else //top
            {
                if (Math.Abs(rect.x) > 0.01f) //put left
                {
                    //top left
                    _uiLoction.anchorMax = Vector2.up;
                    _uiLoction.anchorMin = Vector2.up;
                }
                else //right
                {
                    //top right
                    _uiLoction.anchorMax = Vector2.one;
                    _uiLoction.anchorMin = Vector2.one;
                }
            }


            
        }
    }
}