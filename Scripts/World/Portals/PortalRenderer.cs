using System.Linq;
using UnityEngine;
using FPTemplate.Utilities;
using FPTemplate.Utilities.Extensions;
using FPTemplate.Actors;
using vSplines;
using FPTemplate.Utilities.Maths;
using System.Collections.Generic;

namespace FPTemplate.World.Portals
{

    public class PortalRenderer : TrackedObject<PortalRenderer>
    {
        public static RenderTexture Output { get; private set; }
        public static Camera RootCamera { get; set; }
        private static MaterialPropertyBlock m_propertyBlock;
        public Renderer[] Renderers { get; private set; } = new Renderer[0];
        public RotationalBounds Bounds => new RotationalBounds(transform.position, PortalConfiguration.Size, Quaternion.LookRotation(transform.localToWorldMatrix.MultiplyVector(PortalConfiguration.Normal)).normalized);
        public bool RenderedThisFrame { get; set; }
        public Camera PortalCamera { get; private set; }
        public PortalRenderer Destination;
        public PortalRenderer[] Neighbours;
        public Material PortalMaterial;
        public PortalConfiguration PortalConfiguration;

        private void Start()
        {
            if (Neighbours.Contains(this))
            {
                throw new System.Exception("Neighbours can't contain self");
            }
            if (!Output)
            {
                SetupSharedResources();
            }
            PortalCamera = new GameObject("PortalCamera").AddComponent<Camera>();
            PortalCamera.transform.SetParent(transform);
            PortalCamera.CopyFrom(RootCamera);
            PortalCamera.enabled = false;
            PortalCamera.targetTexture = Output;
            Renderers = GetComponentsInChildren<Renderer>();
        }

        private static void SetupSharedResources()
        {
            RootCamera = CameraController.Instance.GetComponent<Camera>();
            Output = new RenderTexture(Screen.width, Screen.height, 8);
        }

        public bool ShouldRender(PortalRenderer parent, RotationalBounds relativeBounds, out Rect screenRect)
        {
            screenRect = default;
            if (!Destination || !gameObject.activeInHierarchy)
            {
                return false;
            }
            var camera = parent?.PortalCamera ? parent.PortalCamera : RootCamera;
            if (!camera.BoundsWithinFrustrum(relativeBounds))
            {
                return false;
            }
            screenRect = relativeBounds.WorldBoundsToScreenRect(camera);
            if (screenRect == default || !screenRect.ScreenRectIsOnScreen())
            {
                return false;
            }
            screenRect = screenRect.ClipToScreen();
            if (screenRect.width <= 0 || screenRect.height <= 0)
            {
                return false;
            }
            return true;
        }

        private void Update()
        {
            RenderedThisFrame = false;
        }

        public void Render(PortalRenderer parent, List<PortalRenderer> renderChain)
        {
            if (renderChain?.Contains(this) ?? false)
            {
                return;
            }
            var portalMatrix = PortalUtilities.GetPortalMatrix(transform, Destination.transform, PortalConfiguration);
            PortalUtilities.DebugScreenRect(this, parent, portalMatrix);
            var relativeBounds = portalMatrix * Bounds;
            if (!ShouldRender(parent, relativeBounds, out var screenRect) || RenderedThisFrame)
            {
                return;
            }

            // Setup camera
            var camera = parent?.PortalCamera ? parent.PortalCamera : RootCamera;
            PortalCamera.transform.position = portalMatrix.MultiplyPoint3x4(camera.transform.position);
            PortalCamera.transform.forward = portalMatrix.MultiplyVector(camera.transform.forward);
            var viewportRect = screenRect.ScreenRectToViewportRect();
            if (viewportRect.width <= 0 || viewportRect.height <= 0)
            {
                return;
            }
            CameraScissorRectUtility.SetScissorRect(PortalCamera, viewportRect);

            // Add to renderchain
            var nextChain = renderChain?.ToList() ?? new List<PortalRenderer>();
            nextChain.Add(this);
            foreach (var neighbour in Neighbours)
            {
                neighbour.Render(this, nextChain);
            }

            Shader.SetGlobalMatrix("_PortalMatrix", CameraExtensions.GetWorldToViewportMatrix(PortalCamera, Matrix4x4.identity));
            Shader.SetGlobalVector("_WorldClipPos", transform.position - Bounds.center);
            Shader.SetGlobalVector("_WorldClipNormal", Destination.transform.localToWorldMatrix.MultiplyVector(Destination.PortalConfiguration.Normal));

            PortalCamera.RenderDontRestore();

            CameraController.Instance.SetGlobalVariables();
        }

        private void LateUpdate()
        {
            if (m_propertyBlock == null)
            {
                m_propertyBlock = new MaterialPropertyBlock();
            }
            /*if (ShouldRender)
            {
                m_propertyBlock.SetFloat("_Mask", 0);
            }
            else
            {
                m_propertyBlock.SetFloat("_Mask", 1);
            }
            Shader.SetGlobalMatrix("_PortalMatrix", CameraExtensions.GetWorldToViewportMatrix(RootCamera, Matrix4x4.identity));
            Shader.SetGlobalInt("_IsRecursive", 0);*/
            m_propertyBlock.SetTexture("PortalTexture", Output);
            foreach (var r in Renderers)
            {
                r.SetPropertyBlock(m_propertyBlock);
            }
            Render(null, null);
        }

        protected override void OnDestroy()
        {
            Destroy(Output);
            base.OnDestroy();
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
                Gizmos.DrawLine(transform.position, neighbour.transform.position);
            }

            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(Vector3.zero, PortalConfiguration.Normal * 3);
            Gizmos.DrawWireCube(transform.worldToLocalMatrix.MultiplyPoint3x4(Bounds.center), PortalConfiguration.Size);
        }
    }
}