using FPTemplate.Actors;
using FPTemplate.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

namespace FPTemplate.World
{
    public class Jetstream : ExtendedMonoBehaviour
    {
        private Dictionary<MovementController, Coroutine> m_activeCoroutines = new Dictionary<MovementController, Coroutine>();
        public Collider Collider => GetComponent<Collider>();
        public Vector3 Offset;
        public float MoveSpeed;

        private void OnTriggerEnter(Collider other)
        {
            var movementController = other.GetComponent<MovementController>();
            if (movementController == null)
            {
                return;
            }
            if(m_activeCoroutines.TryGetValue(movementController, out var coroutine) && coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            m_activeCoroutines[movementController] = StartCoroutine(MoveController(movementController));
        }

        IEnumerator MoveController(MovementController controller)
        {
            controller.MoveDirection = default;
            var timeInJetStream = 0f;
            while (controller.MoveDirection == default || timeInJetStream < 2)
            {
                controller.IgnoreGravity = true;
                controller.Rigidbody.AddForce(transform.localToWorldMatrix.MultiplyVector(Offset) * Time.deltaTime * MoveSpeed);
                timeInJetStream += Time.deltaTime;
                yield return null;
            }
            controller.IgnoreGravity = false;
            m_activeCoroutines.Remove(controller);
        }

        private void OnTriggerExit(Collider other)
        {
            var movementController = other.GetComponent<MovementController>();
            if (movementController == null)
            {
                return;
            }
            if (m_activeCoroutines.TryGetValue(movementController, out var coroutine) && coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            movementController.IgnoreGravity = false;
            m_activeCoroutines.Remove(movementController);
        }

        private void OnDrawGizmos()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = Color.cyan;
            Gizmos.DrawCube(Offset, Vector3.one);
        }
    }
}