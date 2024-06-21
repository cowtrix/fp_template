using FPTemplate.Actors;
using System;
using UnityEngine.Events;

namespace FPTemplate.Interaction
{
    [Serializable]
    public class ActorActionEvent : UnityEvent<Actor, ActorAction>
    {
    }
}