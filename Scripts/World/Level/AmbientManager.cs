using FPTemplate.Utilities;
using FPTemplate.Utilities.Extensions;
using Muzak;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPTemplate.World
{
    public class AmbientManager : ExtendedMonoBehaviour
    {
        public AmbientLightingState CurrentLightingState { get; private set; }
        public AmbientZone CurrentZone { get; private set; }
        public MuzakPlayer Player => GetComponent<MuzakPlayer>();
        public List<MuzakTrack> AmbientTracks;

        private void Start()
        {
            Player.Track = AmbientTracks.Random();
            StartCoroutine(UpdateZone());
            StartCoroutine(PlayAmbientMusic());
            Player?.EventListener.AddListener(OnEvent);
        }

        private void OnEvent(MuzakPlayerEvent.MuzakEventInfo ev)
        {
            if (ev.EventType == MuzakPlayerEvent.eEventType.TrackLoopEnded)
            {
                Player.Track = AmbientTracks.Random();
            }
        }

        IEnumerator UpdateZone()
        {
            while (true)
            {
                AmbientZone bestZone = null;
                foreach (var zone in AmbientZone.Instances)
                {
                    var wBounds = new Bounds(zone.transform.localToWorldMatrix.MultiplyPoint3x4(zone.Bounds.center), zone.Bounds.size);
                    if (wBounds.Contains(transform.position) && (!bestZone || bestZone.Priority < zone.Priority))
                    {
                        bestZone = zone;
                    }
                }

                CurrentZone = bestZone;
                if (CurrentZone)
                {
                    CurrentZone.Activate();
                }
                yield return null;
            }
        }

        IEnumerator PlayAmbientMusic()
        {
            while (true)
            {
                if (CurrentZone)
                {
                    Player?.Stop();
                }
                if (!CurrentZone && Player.PlayState != MuzakPlayer.ePlayState.Playing)
                {
                    Player?.Play();
                }
                yield return null;
            }
        }
    }
}