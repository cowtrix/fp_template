using UnityEngine;

namespace FPTemplate.Actors
{
    public interface ICameraControllerProxy
	{
		public Quaternion? LookDirectionOverride { get; }
		public Vector3? LookPositionOverride { get; }
		void Look(Actor actor, Vector2 lastDelta);
	}
}