using FPTemplate.Utilities;
using FPTemplate.Utilities.Extensions;
using FPTemplate.World;
using UnityEngine;

namespace FPTemplate.Actors
{

    public class MovementController : ExtendedMonoBehaviour, IMovementController
    {
        public bool IsGrounded { get; private set; }
        public float TimeUngrounded { get; private set; }
        public Vector3 MoveDirection { get; set; }
        public Vector3 CurrentGravity { get; private set; }
        public Rigidbody Rigidbody => GetComponent<Rigidbody>();
        public ActorState State => GetComponent<ActorState>();
        public Actor Actor => GetComponent<Actor>();

        [Header("Parameters")]
        public Transform LookTransform;
        public float MovementSpeed = 2000;
        public float JumpForce = 300;
        public LayerMask CollisionMask = 1 << 8;
        public float RotateTowardGravitySpeed = 10;
        public Transform GroundingPoint;
        public float PushOutSpeed = 10;

        protected bool m_inputJump;

        protected virtual void Start()
        {
            Rigidbody.useGravity = false;
        }

        protected virtual void FixedUpdate()
        {
            var dt = Time.fixedDeltaTime;
            CurrentGravity = GravityManager.Instance.GetGravityForce(transform.position);

            var groundingDistance = Vector3.Distance(GroundingPoint.position, transform.position);
            IsGrounded = Physics.Raycast(transform.position, CurrentGravity, out var groundHit, groundingDistance * 1.01f, CollisionMask, QueryTriggerInteraction.Ignore);
            var isGroundedForward = Physics.Raycast(transform.position + LookTransform.forward, CurrentGravity, out var forwardHit, groundingDistance, CollisionMask, QueryTriggerInteraction.Ignore);
            Debug.DrawLine(transform.position, transform.position + CurrentGravity * dt, Color.green);

            if (!IsGrounded)
            {
                Rigidbody.AddForce(CurrentGravity * dt, ForceMode.Acceleration);
                TimeUngrounded += dt;
            }
            else
            {
                TimeUngrounded = 0;
            }

            {
                // Straighten up
                var straightenQuat = Quaternion.Euler(Quaternion.FromToRotation(Vector3.up, -CurrentGravity.normalized)
                    .eulerAngles.xy().x0z(Rigidbody.rotation.eulerAngles.y));
                var straightenLerp = RotateTowardGravitySpeed * dt * (CurrentGravity.magnitude / 1000f);
                Rigidbody.rotation = Quaternion.Lerp(Rigidbody.rotation, straightenQuat, straightenLerp);
                //Rigidbody.rotation = straightenQuat;
                Debug.DrawLine(transform.position, transform.position + straightenQuat * transform.forward, Color.yellow);

                // Push out
                if (groundHit.distance < groundingDistance)
                {
                    Rigidbody.MovePosition(Rigidbody.position + groundHit.normal * (groundingDistance - groundHit.distance) * dt * PushOutSpeed);
                }
            }

            {
                // Move from input
                if (m_inputJump && IsGrounded)
                {
                    Rigidbody.AddForce(transform.localToWorldMatrix.MultiplyVector(JumpForce * transform.up) * Rigidbody.mass);
                    m_inputJump = false;
                }

                var worldVelocityDirection = LookTransform.localToWorldMatrix.MultiplyVector(new Vector3(MoveDirection.x, 0, MoveDirection.z));

                if (IsGrounded)
                {
                    var normal = isGroundedForward ? forwardHit.normal : groundHit.normal;
                    worldVelocityDirection -= normal * Vector3.Dot(worldVelocityDirection, normal);
                    worldVelocityDirection = worldVelocityDirection.normalized * MovementSpeed;
                }
                else
                {
                    worldVelocityDirection = MutateUngroundedVelocity(worldVelocityDirection);
                }

                Debug.DrawLine(transform.position, transform.position + worldVelocityDirection, Color.cyan, 5);
                Rigidbody.AddForce(worldVelocityDirection * dt);
            }
            if (Actor.Animator)
            {
                Actor.Animator.SetFloat("VelocityX", Rigidbody.linearVelocity.x);
                Actor.Animator.SetFloat("VelocityY", Rigidbody.linearVelocity.y);
                Actor.Animator.SetFloat("VelocityZ", Rigidbody.linearVelocity.z);
            }
        }

        public void Jump()
        {
            m_inputJump = true;
        }

        protected virtual Vector3 MutateUngroundedVelocity(Vector3 worldVelocityDirection)
        {
            worldVelocityDirection *= 0.01f;
            return worldVelocityDirection;
        }

        private void OnDrawGizmosSelected()
        {
            /*if (!GravityManager)
            {
                return;
            }
            var gravityVec = GravityManager.GetGravityForce(transform.position);*/
            Gizmos.DrawLine(transform.position, GroundingPoint.position);
            Gizmos.DrawWireCube(GroundingPoint.position, Vector3.one * .05f);
        }

        public void MoveInput(Vector2 dir)
        {
            //Debug.Log($"Move: {dir}");
            MoveDirection = new Vector3(dir.x, 0, dir.y);
        }
    }
}