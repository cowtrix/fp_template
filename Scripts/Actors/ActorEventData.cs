using System;
using UnityEngine;

namespace FPTemplate.Actors
{
    [Serializable]
    public class ActorEventData
    {
        public GameObject ContextGameObject;
        public Actor SourceActor;
        public Actor TargetActor;
        public Vector3 WorldPosition;
        public bool RequireSight;
        public bool IsIllegal;
    }
}