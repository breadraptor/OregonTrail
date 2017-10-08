using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventsManager {

	private static EventConfig eventConfig;
	public static ArrayList basicResourceTypes;

	static EventsManager() {
		TextAsset eventConfigAsset = (TextAsset)Resources.Load ("Configs/Events");
		eventConfig = JsonUtility.FromJson<EventConfig> (eventConfigAsset.text);
		basicResourceTypes = new ArrayList ();
		basicResourceTypes.Add (ResourceTypes.Rations);
		basicResourceTypes.Add (ResourceTypes.Ammo);
		basicResourceTypes.Add (ResourceTypes.Scrap);
	}

	public static EventStep getRandomTravelEvent() {
		int randomCount = UnityEngine.Random.Range(0, eventConfig.randomTravelEvents.Count);
		string randomEventId = eventConfig.randomTravelEvents [randomCount];
		return getEvent (randomEventId);
	}

	public static EventStep getEvent(string eventId) {
		for (int e = 0; e < eventConfig.eventSteps.Count; ++e) {
			EventStep step = eventConfig.eventSteps [e];
			if (step.id == eventId) {
				return step;
			}
		}
		return null;
	}

	public static EventOption getEventOption(string eventOptionId) {
		for (int e = 0; e < eventConfig.eventOptions.Count; ++e) {
			EventOption option = eventConfig.eventOptions [e];
			if (option.id == eventOptionId) {
				return option;
			}
		}
		return null;
	}

	public static EventValue getEventValue(string eventValueId) {
		Debug.Log ("Trying to get event: " + eventValueId);
		Debug.Log ("Event values count: " + eventConfig.eventValues.Count);
		for (int e = 0; e < eventConfig.eventValues.Count; ++e) {
			EventValue value = eventConfig.eventValues [e];
			Debug.Log ("Value id: " + value.id);
			if (value.id == eventValueId) {
				return value;
			}
		}
		return null;
	}
}

[Serializable]
public class EventConfig {
	public List<string> randomTravelEvents;
	public List<string> randomCityEvents;
	public List<EventStep> eventSteps;
	public List<EventOption> eventOptions;
	public List<EventValue> eventValues;
}

[Serializable]
public class EventStep {
	public string id;
	public string displayText;
	public List<string> options;
}

[Serializable]
public class EventOption {
	public string id = "";
	public string nextStepId = "";
	public string buttonText = "";
	public string cantAffordButtonText = "";
	public List<string> rewardIds = new List<string>();
	public List<string> costIds = new List<string>();
	public List<string> eventFlags = new List<string>();
	[SerializeField]
	private string healthEffect = "";

	private HealthEffect _diseaseEffect = HealthEffect.TYPE_COUNT;
	public HealthEffect diseaseEffect {
		get {
			if (healthEffect == null || healthEffect == "") {
				_diseaseEffect = HealthEffect.None;
			} else if (_diseaseEffect == HealthEffect.TYPE_COUNT) {
				_diseaseEffect = (HealthEffect)Enum.Parse (typeof(HealthEffect), healthEffect);
			}
			return _diseaseEffect;
		}
	}
}

[Serializable]
public class EventValue {
	public string id;
	[SerializeField]
	private string type;
	[SerializeField]
	private int minValue;
	[SerializeField]
	private int maxValue;

	private ResourceTypes _resourceType = ResourceTypes.TYPE_COUNT;
	public ResourceTypes resourceType {
		get {
			if (_resourceType == ResourceTypes.TYPE_COUNT) {
				_resourceType = (ResourceTypes)Enum.Parse (typeof(ResourceTypes), type);
				if (_resourceType == ResourceTypes.Basic) {
					int randomBasicIndex = UnityEngine.Random.Range (0, EventsManager.basicResourceTypes.Count);
					_resourceType = (ResourceTypes)EventsManager.basicResourceTypes[randomBasicIndex];
				}
			}
			return _resourceType;
		}
	}

	private int _resourceValue = int.MinValue;
	public int resourceValue {
		get {
			if (_resourceValue == int.MinValue) {
				_resourceValue = UnityEngine.Random.Range (minValue, maxValue);
			}
			return _resourceValue;
		}
	}
}