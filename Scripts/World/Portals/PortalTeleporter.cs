using UnityEngine;
using FPTemplate.Utilities;
using FPTemplate.Actors;

namespace FPTemplate.World.Portals
{
    public class PortalTeleporter : ExtendedMonoBehaviour
    {
        public PortalRenderer Portal => GetComponent<PortalRenderer>();

        private void OnTriggerEnter(Collider other)
        {
            var player = other.GetComponent<PlayerMovementController>();
            if (!player)
            {
                return;
            }

            var camVel = player.Rigidbody.GetPointVelocity(CameraController.Instance.transform.position);
            if (Vector3.Dot(camVel, transform.forward) < 0)
            {
                return;
            }

            Debug.Log($"Teleporting player to {this}");
            var matrix = PortalUtilities.GetPortalMatrix(transform, Portal.Destination.transform, Portal.PortalConfiguration);
            player.transform.position = matrix.MultiplyPoint3x4(player.transform.position);
            player.transform.forward = matrix.MultiplyVector(player.transform.forward);
            player.Rigidbody.linearVelocity = matrix.MultiplyVector(player.Rigidbody.linearVelocity);
        }
    }
}