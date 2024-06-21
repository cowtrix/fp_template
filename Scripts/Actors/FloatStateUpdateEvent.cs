using System;
using UnityEngine.Events;

namespace FPTemplate.Actors
{
    [Serializable]
	public class FloatStateUpdateEvent : UnityEvent<Actor, StateUpdate<float>> { }
}