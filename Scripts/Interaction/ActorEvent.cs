using FPTemplate.Actors;
using System;
using UnityEngine.Events;

namespace FPTemplate.Interaction
{
    [Serializable]
    public class ActorEvent : UnityEvent<Actor>
    {
    }
}