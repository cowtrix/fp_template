using FPTemplate.Utilities;
using FPTemplate.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace FPTemplate.Actors
{
    [RequireComponent(typeof(Actor))]
	public abstract class StateContainer : TrackedObject<StateContainer>
	{
		private Dictionary<string, PropertyInfo> m_fieldLookup;
		public Actor Actor => GetComponent<Actor>();
		public FloatStateUpdateEvent OnStateUpdate = new FloatStateUpdateEvent();

		public IStateProvider[] StateProviders { get; private set; }

		protected virtual void Awake()
		{
			StateProviders = gameObject.GetComponentsByInterfaceInChildren<IStateProvider>(true);

			m_fieldLookup = GetType()
				.GetProperties()
				.Where(p => p.CanWrite && p.CanRead && p.GetCustomAttribute<NonSerializedAttribute>() == null && !p.DeclaringType.Assembly.FullName.Contains("UnityEngine.CoreModule"))
				.ToDictionary(f => f.Name, f => f);

			foreach (var field in m_fieldLookup
				.Where(f => f.Value.PropertyType == typeof(float) || f.Value.PropertyType == typeof(int)))
			{
				var currentValue = (float)Convert.ChangeType(field.Value.GetValue(this), typeof(float));
				OnStateUpdate.Invoke(Actor, new StateUpdate<float>(field.Key, null, currentValue, 0, true));
			}
		}

		public Vector2 GetMinMax(string key)
        {
			var field = GetType().GetProperty(key.ToString());
			var attr = field.GetCustomAttribute<StateMinMaxAttribute>();
			if(attr != null)
            {
				return new Vector2(attr.Min, attr.Max);
            }
			return new Vector2(float.MinValue, float.MaxValue);
        }

		public bool TryGetValue<T>(string key, out T result)
		{
			if (!m_fieldLookup.TryGetValue(key.ToString(), out var fieldInfo))
			{
				result = default;
				return false;
			}
			var rawObject = fieldInfo.GetValue(this);
			result = (T)Convert.ChangeType(rawObject, typeof(T));
			return true;
		}

		public bool TryGetValueNormalized(string key, out float result)
		{
			if (!m_fieldLookup.TryGetValue(key.ToString(), out var fieldInfo))
			{
				result = default;
				return false;
			}
			var rawObject = fieldInfo.GetValue(this);
			result = (float)Convert.ChangeType(rawObject, typeof(float));
			var range = GetMinMax(key);
			result = (result - range.x) / (range.y - range.x);
			return true;
		}

		public bool TryAdd(string field, float delta, string description)
		{
			if (!m_fieldLookup.TryGetValue(field.ToString(), out var fieldInfo))
			{
				Debug.LogWarning($"Tried to delta {field} but it did not exist on actor {Actor}", this);
				return false;
			}
			
			var val = (float)fieldInfo.GetValue(this);
			var newVal = val + delta;
			var range = GetMinMax(field);
			if (newVal < range.x || newVal > range.y)
			{
				OnStateUpdate.Invoke(Actor, new StateUpdate<float>(field, description, delta, val, false));
				return false;
			}

			newVal = Mathf.Clamp(newVal, range.x, range.y);
			fieldInfo.SetValue(this, newVal);
			OnStateUpdate.Invoke(Actor, new StateUpdate<float>(field, description, newVal, delta, true));
			//Debug.Log($"State update: {field}: {newVal} ({delta})");
			return true;
		}

		public bool TryAdd(string key, int delta, string desc)
		{
			if (!m_fieldLookup.TryGetValue(key.ToString(), out var fieldInfo))
			{
				Debug.LogWarning($"Tried to delta {key} but it did not exist on actor {Actor}", this);
				return false;
			}
			/*if (fieldInfo.PropertyType != typeof(int))
			{
				throw new Exception($"State delta type mismatch for {field}, expected int but got {fieldInfo.PropertyType}");
			}*/
			var val = (int)fieldInfo.GetValue(this);
			var newVal = val + delta;
			var range = GetMinMax(key);
			
			if (newVal < range.x || newVal > range.y)
			{
				OnStateUpdate.Invoke(Actor, new StateUpdate<float>(key, desc, delta, val, false));
				return false;
			}

			newVal = Mathf.RoundToInt(Mathf.Clamp(newVal, range.x, range.y));
			fieldInfo.SetValue(this, newVal);
			OnStateUpdate.Invoke(Actor, new StateUpdate<float>(key, desc, newVal, delta, true));
			//Debug.Log($"State update: {field}: {newVal} ({delta})");
			return true;
		}

        /*public JObject GetSaveData()
		{
			var data = new JObject();
			data[nameof(GUID)] = GUID;
			foreach (var f in m_fieldLookup)
			{
				data[f.Key] = JsonUtility.ToJson(f.Value.GetValue(this));
			}
			foreach (var provider in StateProviders)
			{
				var providerSaveData = provider.GetSaveData();
				data[provider.GUID] = providerSaveData;
				Debug.Log($"Provider {provider} loaded save data {providerSaveData}");
			}
			Debug.Log($"State container {this} saved data {data}");
			return data;
		}

		public void LoadSaveData(JObject data)
		{
			Debug.Log($"State container {this} loaded save data {data}");
			foreach (var token in data.Children())
			{
				if (!(token is JObject jObj))
				{
					Debug.LogError($"Unexpected token in save data: {token}", this);
					return;
				}

				if (Enum.TryParse(typeof(eStateKey), jObj.Path, out var enumObj) 
					&& enumObj is eStateKey key
					&& m_fieldLookup.TryGetValue(key.ToString(), out var prop))
				{
					var obj = JsonUtility.FromJson(jObj.ToString(), prop.PropertyType);
					prop.SetValue(this, obj);
					return;
				}

				var provider = StateProviders.FirstOrDefault(s => s.GUID == jObj.Path);
				if (provider != null)
				{
					Debug.Log($"Provider {provider} loaded save data {jObj}");
					provider.LoadSaveData(jObj.ToString());
				}

				Debug.LogError($"Failed to find property with name: {jObj.Path}", this);
			}
			OnSaveDataLoaded();
		}*/

        protected virtual void OnSaveDataLoaded() { }
	}
}