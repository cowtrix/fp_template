using UnityEngine;

namespace FPTemplate.Utilities
{
    public class DisableChildrenEachFrame : ExtendedMonoBehaviour
    {
        private void LateUpdate()
        {
            foreach(Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
        }
    }
}