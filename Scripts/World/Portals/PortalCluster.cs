using FPTemplate.Actors;
using FPTemplate.Utilities;
using FPTemplate.Utilities.Extensions;
using FPTemplate.Utilities.Helpers;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;

namespace FPTemplate.World.Portals
{
    public struct PortalState
    {
        public int Depth;
        public Rect ScreenRect;
        public Rect ViewportRect => ScreenRect.ScreenRectToViewportRect();
        public Matrix4x4 TransformMatrix;
    }

    public class PortalCluster : TrackedObject<PortalCluster>
    {
        public static CameraController CamController => CameraController.Instance;
        public PortalManager PortManager => PortalManager.Instance;
        public bool Active { get; private set; } = true;
        public IEnumerable<PortalRenderer> Portals => GetComponentsInChildren<PortalRenderer>(true);

        private void Update()
        {
            if (!Active)
            {
                return;
            }

            var activePortals = Portals;
            var rootCamera = CamController.Camera;

            var culledPortals = CullPortalsForRender(activePortals, 0, new Rect(0, 0, Screen.width, Screen.height), Matrix4x4.identity);

            foreach (var portalRenderState in culledPortals.Reverse())
            {
                portalRenderState.Item1.Render(portalRenderState.Item2);
            }
        }

        public static IEnumerable<(PortalRenderer, PortalState)> CullPortalsForRender(IEnumerable<PortalRenderer> activePortals, int depth, Rect parentRect, Matrix4x4 parentMatrix)
        {
            var portalStateMapping = new Dictionary<PortalRenderer, PortalState>();
            var orderedPortals = activePortals
                .OrderByDescending(p => Vector3.Distance(CamController.transform.position, p.transform.position)).ToList();
            // Collect screen rect data
            for (var i = 0; i < orderedPortals.Count; i++)
            {
                var portal = orderedPortals[i];
                // Back to front
                var success = PortalUtilities.TryGetScreenRect(portal.PortalConfiguration, parentMatrix * portal.Bounds, out var sr);
                if (!sr.Overlaps(parentRect))
                {
                    sr = Rect.zero;
                }
                sr = sr.CropToOuterRect(parentRect);
                portalStateMapping[portal] = new PortalState
                {
                    Depth = depth,
                    ScreenRect = sr,
                    TransformMatrix = parentMatrix.inverse * PortalUtilities.GetPortalMatrix(portal.transform, portal.Destination.transform, portal.PortalConfiguration),
                };
            }
            var screenRects = RectExtensions.OccludeRects(portalStateMapping.Select(kvp => kvp.Value.ScreenRect).ToList());
            for (var i = orderedPortals.Count - 1; i >= 0; i--)
            {
                var portal = orderedPortals[i];
                var state = portalStateMapping[portal];
                state.ScreenRect = screenRects[i];
                if (state.ScreenRect == Rect.zero || state.ScreenRect.width <= 0 || state.ScreenRect.height <= 0)
                {
                    orderedPortals.RemoveAt(i);
                    continue;
                }
                var rootCam = CameraController.Instance.Camera;
                var hash = portal.GetHashCode();
                UnityEngine.Random.InitState(hash);
                var col = UnityEngine.Random.ColorHSV();
                DebugHelper.DrawScreenRect(rootCam, state.ScreenRect, col);
                var str = $"{portal.name}\n{depth}";
                DebugUI.Instance.DebugLabel(str, str, state.ScreenRect.position, col);
                // Draw any sub portals
                var newMatrix = portal.PortalMatrix.inverse * parentMatrix;
                if (depth < PortalManager.Instance.MaxRecursionDepth)
                {
                    var destinationPortals = portal.Destination.Cluster.Portals;
                    var cull = CullPortalsForRender(destinationPortals, depth + 1, state.ScreenRect, newMatrix).ToList();
                    foreach (var subPortal in cull)
                    {
                        yield return subPortal;
                    }
                }

                yield return (portal, state);
            }
        }

    }
}