using UnityEngine;

namespace FPTemplate.Actors
{
    public interface ILookAdapter
    {
        Transform transform { get; }
        bool CanSee(Vector3 worldPosition);
    }
}