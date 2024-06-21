using UnityEngine;
using UnityEngine.AI;
using FPTemplate.Utilities;

namespace FPTemplate.Actors
{
    public class NPCSimpleMovementController : SlowUpdater, IMovementController
    {
        public Rigidbody Rigidbody => GetComponent<Rigidbody>();
        public NavMeshAgent Navmesh => GetComponent<NavMeshAgent>();
        public Actor Actor => GetComponent<Actor>();
        public Vector3 MoveDirection { get; set; }
        public bool IsGrounded => Navmesh.isOnNavMesh;
        public Vector3 LookPosition => transform.position + (Navmesh.velocity.sqrMagnitude > 0 ? Navmesh.velocity.normalized : transform.forward);
        public Vector3 CurrentGravity => Vector3.down;

        public Vector3 ExtraRotation;
        public float TurnSpeed = 30;
        public float AnimationExpressiveness = 1;
        public NavMeshQueryFilter NavmeshFilter;

        private Quaternion m_lastRotation;
        private NavMeshPath m_currentPath;

        private SmoothPositionVector3 m_smoothPosition;

        private void Start()
        {
            m_smoothPosition = new SmoothPositionVector3(10, transform.position);
        }

        protected override int TickOnThread(float dt)
        {
            if (Actor.Animator)
            {
                var localVelocity = transform.worldToLocalMatrix.MultiplyVector(transform.position - m_smoothPosition.SmoothPosition) * AnimationExpressiveness;
                //Debug.DrawLine(transform.position, transform.position + transform.localToWorldMatrix.MultiplyVector(localVelocity), Color.cyan);
                Actor.Animator.SetFloat("VelocityX", localVelocity.z);
                Actor.Animator.SetFloat("VelocityY", localVelocity.y);
                Actor.Animator.SetFloat("VelocityZ", localVelocity.x);
            }
            m_smoothPosition.Push(transform.position);
            return 1;
        }

        public void MoveToPosition(Vector3 worldPos)
        {
            m_currentPath = new NavMeshPath();
            if (!NavMesh.CalculatePath(transform.position, worldPos, NavmeshFilter, m_currentPath))
            {
                m_currentPath = null;
                return;
            }
            Navmesh.SetPath(m_currentPath);
        }

        public void Jump()
        {
        }

        public void MoveInput(Vector2 dir)
        {
        }

        private void OnDrawGizmosSelected()
        {
            if (Navmesh.path != null)
            {
                for (int i = 0; i < Navmesh.path.corners.Length - 1; i++)
                {
                    Gizmos.DrawLine(Navmesh.path.corners[i], Navmesh.path.corners[i + 1]);
                }
                Gizmos.DrawWireCube(LookPosition, Vector3.one * .2f);
            }
        }
    }
}