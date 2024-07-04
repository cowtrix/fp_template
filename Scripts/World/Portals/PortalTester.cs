using FPTemplate.Utilities;
using FPTemplate.Utilities.Extensions;
using UnityEngine;

namespace FPTemplate.World.Portals
{

    public class PortalTester : ExtendedMonoBehaviour
    {
        public Transform In, Out;
        public PortalConfiguration Config;

        public Camera Camera;



        public void OnDrawGizmos()
        {
            Gizmos.matrix = In.transform.localToWorldMatrix;
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(Vector3.zero, Config.Normal * 3);
            Gizmos.DrawWireCube(Vector3.zero, Config.Size);


            Gizmos.matrix = Out.transform.localToWorldMatrix;
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(Vector3.zero, Config.Normal * 3);
            Gizmos.DrawWireCube(Vector3.zero, Config.Size);

            Gizmos.matrix = Camera.transform.localToWorldMatrix;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(.25f, .25f, .5f));

            Gizmos.matrix = PortalUtilities.GetPortalMatrix(In, Out, Config);
            GizmoExtensions.DrawWireCube(Camera.transform.position, new Vector3(.25f, .25f, .5f) / 2f, Camera.transform.rotation, Color.red);
            Gizmos.DrawLine(Camera.transform.position, Camera.transform.position + Camera.transform.forward);
        }
    }
}