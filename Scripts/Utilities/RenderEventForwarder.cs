using FPTemplate.Utilities;
using UnityEngine;

namespace FPTemplate.Utilities
{
    public class RenderEventForwarder : ExtendedMonoBehaviour
    {
        public MonoBehaviour Target;

        private void OnWillRenderObject()
        {
            Target.SendMessage("OnWillRenderObject");
        }

        private void OnRenderObject()
        {
            Target.SendMessage("OnRenderObject");
        }
    }
}