using UnityEngine;

namespace FPTemplate.Actors
{
    public interface IMovementController
    {
        Vector3 CurrentGravity { get; }
        Vector3 MoveDirection { get; set; }
        bool IsGrounded { get; }
        void MoveInput(Vector2 dir);
        void Jump();
        Rigidbody Rigidbody { get; }
    }
}