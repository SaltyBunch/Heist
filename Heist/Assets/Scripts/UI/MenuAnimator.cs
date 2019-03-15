using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class MenuAnimator : MonoBehaviour
    {
        [SerializeField] private Transform _mainView;

        [Header("Options")] [SerializeField] private Transform _optionsControls;
        [SerializeField] private Transform _optionsSound;
        [SerializeField] private Transform _optionsCredits;

        [SerializeField] private Transform _credits;


        private IEnumerator AnimateTo(List<Transform> newTransforms, float speed)
        {
            foreach (var newTransform in newTransforms)
            {
                do
                {
                    transform.position = Vector3.Lerp(transform.position, newTransform.position, speed);
                    transform.rotation = Quaternion.Slerp(transform.rotation, newTransform.rotation, speed);
                    yield return new WaitForFixedUpdate();
                }
            }
        }
    }
}