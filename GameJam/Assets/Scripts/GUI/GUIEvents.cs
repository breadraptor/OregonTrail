using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum GUIEvents {
	GoToMenu,
	DisplaySupplies,
	ContinueJourney,
	ChangePace,
	ChangeRations,
	StopToRest,
	HaveRandomConversation
}

public class ButtonConfig {
	public string displayText;
	public UnityAction action;

	public ButtonConfig(string t, UnityAction a) {
		displayText = t;
		action = a;
	}
}