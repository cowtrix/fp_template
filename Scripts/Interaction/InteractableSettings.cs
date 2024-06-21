using System;
using System.Collections.Generic;
using UnityEngine;

namespace FPTemplate.Interaction
{
    [Serializable]
    public class InteractableSettings
    {
        [Serializable]
        public class InteractionRenderer
        {
            public MeshRenderer Renderer;
            public Mesh Mesh;
        }

        public ActorEvent OnFocusEnter;
        public ActorEvent OnFocusExit;
        public ActorActionEvent OnUsed;
        public ActorEvent OnEnterAttention;
        public ActorEvent OnExitAttention;

        public Func<Sprite> Icon;

        public List<Collider> Colliders;
        public List<InteractionRenderer> Renderers;

        public float MaxFocusDistance = 5;
        public float MaxUseDistance = 2;
    }
}