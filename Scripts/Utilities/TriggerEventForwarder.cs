using System;
using UnityEngine;
using UnityEngine.Events;

namespace FPTemplate.World
{
    [Serializable]
    public class ColliderEvent : UnityEvent<Collider> { }

    [RequireComponent(typeof(Collider))]
    public class TriggerEventForwarder : MonoBehaviour
    {
		public ColliderEvent TriggerEnter, TriggerStay, TriggerExit;

        private void OnTriggerEnter(Collider other)
        {
            TriggerEnter?.Invoke(other);
        }

        private void OnTriggerExit(Collider other)
        {
            TriggerExit?.Invoke(other);
        }

        private void OnTriggerStay(Collider other)
        {
            TriggerStay?.Invoke(other);
        }
    }

}