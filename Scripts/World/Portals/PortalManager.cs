using FPTemplate.Utilities;
using UnityEngine;

namespace FPTemplate.World.Portals
{
    public class PortalManager : Singleton<PortalManager>
    {
        public Material PortalMaterial;
        public int MaxRecursionDepth = 2;
    }
}