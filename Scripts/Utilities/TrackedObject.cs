using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FPTemplate.Utilities
{
    public abstract class TrackedObject<T> : ExtendedMonoBehaviour where T : TrackedObject<T>
    {
        public virtual bool AutoRegister => true;
        public virtual bool TrackDisabled => false;
        private static Dictionary<string, T> m_instances = new Dictionary<string, T>();

        protected virtual void OnEnable()
        {
            if (AutoRegister)
            {
                Register();
            }
        }

        protected virtual void OnDisable()
        {
            if (TrackDisabled)
            {
                return;
            }
            Unregister();
        }

        protected virtual void OnDestroy()
        {
            Unregister();
        }

        public virtual void Register()
        {
            GUID = null;
            var keys = m_instances.Keys.ToList();
            foreach (var key in keys)
            {
                var existing = m_instances[key];
                if (!existing || existing == this)
                {
                    m_instances.Remove(key);
                }
            }
            if (m_instances.ContainsKey(GUID))
            {
                var existing = m_instances[GUID];
                Debug.Break();
            }
            m_instances.Add(GUID, this as T);
        }

        public virtual void Unregister()
        {
            m_instances.Remove(GUID);
        }

        public static IEnumerable<T> Instances
        {
            get
            {
                if (UnityMainThreadDispatcher.IsOnMainThread && !Application.isPlaying)
                {
                    foreach (var i in FindObjectsByType<T>(FindObjectsSortMode.None))
                    {
                        yield return i;
                    }
                }
                else
                {
                    var keys = m_instances.Keys.ToList();
                    for (int i = 0; i < keys.Count; i++)
                    {
                        string key = keys[i];
                        if (!m_instances.TryGetValue(key, out var instance) || !instance)
                        {
                            continue;
                        }
                        yield return instance;
                    }
                }
            }
        }

        public static IEnumerable<T2> GetTyped<T2>() where T2 : T
        {
            if (!Application.isPlaying)
            {
                foreach (var i in FindObjectsOfType<T2>())
                {
                    yield return i;
                }
            }
            else
            {
                var keys = m_instances.Keys.ToList();
                for (int i = 0; i < keys.Count; i++)
                {
                    string key = keys[i];
                    var instance = m_instances[key];
                    if (!instance || !(instance is T2 cast))
                    {
                        continue;
                    }
                    yield return cast;
                }
            }
        }

        public static bool IsRegistered(T val) => m_instances.ContainsValue(val);

        public static bool TryGetValue(string guid, out T val)
        {
            if (!m_instances.TryGetValue(guid, out val))
            {
                return false;
            }
            return true;
        }

        [SerializeField]
        private string m_guid;

        public string GUID
        {
            get
            {
                if (string.IsNullOrEmpty(m_guid))
                {
                    do
                    {
                        m_guid = Guid.NewGuid().ToString();
                    }
                    while (m_instances.ContainsKey(m_guid));
                }
                return m_guid;
            }
            set
            {
                m_guid = value;
            }
        }
    }
}