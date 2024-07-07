using System.Linq;
using UnityEngine;
using FPTemplate.Utilities;
using FPTemplate.Utilities.Extensions;
using FPTemplate.Actors;
using vSplines;
using FPTemplate.Utilities.Maths;
using System.Collections.Generic;
using FPTemplate.Utilities.Helpers;
using System.Reflection;

namespace FPTemplate.World.Portals
{

    public class PortalRenderer : TrackedObject<PortalRenderer>
    {
        public static RenderTexture Output { get; private set; }
        public static Camera RootCamera { get; set; }
        public Matrix4x4 PortalMatrix => PortalUtilities.GetPortalMatrix(transform, Destination.transform, PortalConfiguration);
        public RotationalBounds Bounds => new RotationalBounds(transform.position, PortalConfiguration.Size, Quaternion.LookRotation(transform.localToWorldMatrix.MultiplyVector(PortalConfiguration.Normal)).normalized);
        public Mesh Mesh { get; set; }
        public Camera PortalCamera { get; private set; }
        public PortalRenderer Destination;
        public PortalRenderer[] Neighbours;
        public Material PortalMaterial;
        public PortalConfiguration PortalConfiguration;

        public PortalCluster Cluster;

        private void Start()
        {
            if (!Output)
            {
                SetupSharedResources();
            }
            PortalCamera = new GameObject("PortalCamera").AddComponent<Camera>();
            PortalCamera.transform.SetParent(transform);
            PortalCamera.CopyFrom(RootCamera);
            PortalCamera.enabled = false;
            PortalCamera.clearFlags = CameraClearFlags.Nothing;
            PortalCamera.targetTexture = Output;
            Neighbours.Add(this);
            Mesh = MeshExtensions.CreateQuadMesh(PortalConfiguration.Size, -PortalConfiguration.Normal);
            var mf = gameObject.AddComponent<MeshFilter>();
            mf.sharedMesh = Mesh;
            var mr = gameObject.AddComponent<MeshRenderer>();
            mr.sharedMaterial = PortalMaterial;
        }

        private static void SetupSharedResources()
        {
            RootCamera = CameraController.Instance.GetComponent<Camera>();
            Output = new RenderTexture(Screen.width, Screen.height, 8);
            Output.enableRandomWrite = true;
            Output.Create();
        }

        private static bool TestLine(Camera cam, Vector3 p1, Vector3 p2)
        {
            if (!cam.IsLineInFrustum(p1, p2))
            {
                Debug.DrawLine(p1, p2, Color.red, 1);
                return false;
            }
            Debug.DrawLine(p1, p2, Color.green, 0);
            return true;
        }

        public void Render(PortalState state)
        {
            PortalCamera.transform.position = state.TransformMatrix.MultiplyPoint3x4(RootCamera.transform.position);
            PortalCamera.transform.forward = state.TransformMatrix.MultiplyVector(RootCamera.transform.forward);
            CameraScissorRectUtility.SetScissorRect(PortalCamera, state.ViewportRect);
            
            //Shader.SetGlobalMatrix("_PortalMatrix", lookBackMatrix);
            Shader.SetGlobalTexture("_PortalTexture", Output);
            Shader.SetGlobalVector("_WorldClipPos", transform.position - Bounds.center);
            Shader.SetGlobalVector("_WorldClipNormal", Destination.transform.localToWorldMatrix.MultiplyVector(Destination.PortalConfiguration.Normal));

            var canvasHackField = typeof(Canvas).GetField("willRenderCanvases", BindingFlags.NonPublic | BindingFlags.Static);
            var canvasHackObject = canvasHackField.GetValue(null);
            canvasHackField.SetValue(null, null);

            DebugHelper.DrawCube(PortalCamera.transform.position, new Vector3(.25f, .25f, .5f) / 2f, PortalCamera.transform.rotation, Color.blue, 0);
            PortalCamera.gameObject.SetActive(true);
            PortalCamera.RenderDontRestore();

            canvasHackField.SetValue(null, canvasHackObject);
            CameraController.Instance.SetGlobalVariables();

            /*if (!gameObject.activeInHierarchy || renderChain.Count > 1)
            {
                return;
            }
            PortalCamera.gameObject.SetActive(false);
            var parent = renderChain.LastOrDefault();
            //var portalMatrix = PortalUtilities.GetPortalMatrix(transform, Destination.transform, PortalConfiguration);
            var lookBackMatrix = Matrix4x4.identity;
            for (int i = renderChain.Count - 1; i >= 0; i--)
            {
                var portal = renderChain[i];
                var parentLookback = renderChain.ElementAtOrDefault(i - 1)?.transform ?? parent.transform;
                lookBackMatrix = PortalUtilities.GetPortalMatrix(parentLookback, portal.transform, portal.PortalConfiguration).inverse * lookBackMatrix;
            }
            var bounds = lookBackMatrix * Bounds;
            //PortalUtilities.DebugScreenRect(this, parent, bounds);
            DebugHelper.DrawCube(bounds.center, bounds.extents, bounds.rotation, Color.yellow, 0);
            if (!ShouldRender(bounds, out var screenRect) || RenderedThisFrame)
            {
                return;
            }

            var portalMatrix = lookBackMatrix * PortalUtilities.GetPortalMatrix(transform, Destination.transform, PortalConfiguration);

            // Setup camera
            var camera = parent?.PortalCamera ? parent.PortalCamera : RootCamera;
            PortalCamera.transform.position = portalMatrix.MultiplyPoint3x4(camera.transform.position);
            PortalCamera.transform.forward = portalMatrix.MultiplyVector(camera.transform.forward);

            // Render children
            var nextChain = renderChain?.ToList() ?? new List<PortalRenderer>();
            nextChain.Add(this);
            foreach (var neighbour in Neighbours)
            {
                if (!neighbour)
                {
                    continue;
                }
                neighbour.Render(nextChain);
            }

            PortalCamera.transform.position = portalMatrix.MultiplyPoint3x4(camera.transform.position);
            PortalCamera.transform.forward = portalMatrix.MultiplyVector(camera.transform.forward);
            var viewportRect = screenRect.ScreenRectToViewportRect();
            if (viewportRect.width <= 0 || viewportRect.height <= 0)
            {
                return;
            }

            CameraScissorRectUtility.SetScissorRect(PortalCamera, viewportRect);

            if (parent)
            {
                portalMatrix = PortalUtilities.GetPortalMatrix(parent.transform, transform, PortalConfiguration);
            }
            PortalCamera.transform.position = portalMatrix.MultiplyPoint3x4(camera.transform.position);
            PortalCamera.transform.forward = portalMatrix.MultiplyVector(camera.transform.forward);
            Shader.SetGlobalMatrix("_PortalMatrix", lookBackMatrix);
            Shader.SetGlobalTexture("_PortalTexture", Output);
            Shader.SetGlobalVector("_WorldClipPos", transform.position - Bounds.center);
            Shader.SetGlobalVector("_WorldClipNormal", Destination.transform.localToWorldMatrix.MultiplyVector(Destination.PortalConfiguration.Normal));
            PortalCamera.gameObject.SetActive(true);
            PortalCamera.RenderDontRestore();

            CameraController.Instance.SetGlobalVariables();*/
        }

        private void OnDrawGizmos()
        {
            if (!Destination)
            {
                return;
            }
            var connector = new SplineSegment(new SplineSegment.ControlPoint
            {
                Position = Bounds.center,
                Control = transform.localToWorldMatrix.MultiplyVector(PortalConfiguration.Normal) * 10,
            }, new SplineSegment.ControlPoint
            {
                Position = Destination.Bounds.center,
                Control = Destination.transform.localToWorldMatrix.MultiplyVector(Destination.PortalConfiguration.Normal) * 10,
            }, 12);
            connector.Recalculate(true);
            connector.DrawGizmos(Color.green);

            Gizmos.color = Color.yellow;
            foreach (var neighbour in Neighbours)
            {
                if (!neighbour)
                {
                    continue;
                }
                Gizmos.DrawLine(transform.position, neighbour.transform.position);
            }

            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(Vector3.zero, PortalConfiguration.Normal * 3);
            Gizmos.DrawWireCube(transform.worldToLocalMatrix.MultiplyPoint3x4(Bounds.center), PortalConfiguration.Size);
        }
    }
}