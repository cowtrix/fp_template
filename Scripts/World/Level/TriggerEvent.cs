using System;
using UnityEngine.Events;

namespace FPTemplate.World
{
    [Serializable]
	public class TriggerEvent : UnityEvent<LevelTrigger> { }
}