using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour {

	public Font mainFont;
	private int maxChoicesCount = 5;
	public GameObject defaultText;
	public GameObject defaultButton;
	public GameObject choicesMenu;
	public GameObject locationTimeDisplay;
	public GameObject currentStatusDisplay;
	public GameObject mainMenuButton;

	private GameController mainGameController;

	ArrayList mainMenuButtonConfigs = new ArrayList();
	ArrayList paceMenuButtonConfigs = new ArrayList();
	ArrayList rationMenuButtonConfigs = new ArrayList();
	ArrayList restMenuButtonConfigs = new ArrayList();

	void Start () {

		mainGameController = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController>();

		buildAllButtonConfigs ();

		configureUIWithEvent (GUIEvents.GoToMenu);

	}


	public void configureUIWithEvent(GUIEvents uiEvent) {
		deactivateAllDisplays ();

		switch(uiEvent) {
		case GUIEvents.GoToMenu:
			mainGameController.StopWorldCoroutine();
			displayMainMenu ();
			break;
		case GUIEvents.ContinueJourney:
			displayContinueJourneyMenu ();
			break;
		case GUIEvents.DisplaySupplies:
			displaySuppliesMenu ();
			break;
		default:
			Debug.Log ("UNRECOGNIZED EVENT: " + uiEvent.ToString ());
			break;
		}
	}

	private void deactivateAllDisplays() {
		
		locationTimeDisplay.SetActive (false);
		currentStatusDisplay.SetActive (false);
		choicesMenu.SetActive (false);
		mainMenuButton.SetActive (false);

	}

	private void displayMainMenu() {
		Debug.Log ("Going to menu");
		updateLocationTimeDisplay ("This displays location\nAND TIME!! WOO");
		locationTimeDisplay.SetActive (true);

		updateCurrentStatusDisplay ("Some status\nSome more status\nI hope this is working...");
		currentStatusDisplay.SetActive (true);

		setChoicesMenuWithOptions (mainMenuButtonConfigs);
		choicesMenu.SetActive (true);
	}

	private void displayContinueJourneyMenu() {
		updateCurrentStatusDisplay ("I SWEAR you're continuing your journey right now");
		currentStatusDisplay.SetActive (true);

		mainGameController.StartWorldCoroutine ();

		mainMenuButton.SetActive (true);
	}

	private void displaySuppliesMenu() {
		updateCurrentStatusDisplay ("This should actually be showing you your supplies...");
		currentStatusDisplay.SetActive (true);

		mainMenuButton.SetActive (true);
	}

	public void updateLocationTimeDisplay(string text) {
		locationTimeDisplay.GetComponent<Text> ().text = text;
	}

	public void updateCurrentStatusDisplay(string text) {
		currentStatusDisplay.GetComponent<Text> ().text = text;
	}

	private void configureButton(ButtonConfig config, GameObject button) {
		button.GetComponentInChildren<Text> ().text = config.displayText;
		button.GetComponent<Button> ().onClick.AddListener (config.action);
	}

	void setChoicesMenuWithOptions(ArrayList buttonConfigs) {

		Button[] currentButtons = choicesMenu.GetComponentsInChildren<Button> ();
		for (int i = 0; i < currentButtons.Length; ++i) {
			DestroyImmediate (currentButtons [i].gameObject);
		}

		float buttonSize = 1.0f / (float)maxChoicesCount;

		for (int i = 0; i < buttonConfigs.Count && i < maxChoicesCount; ++i) {
			ButtonConfig config = (ButtonConfig)buttonConfigs [i];
			GameObject newButtonObj = Instantiate (defaultButton, choicesMenu.transform);
			newButtonObj.name = "Choice Button " + i;
			RectTransform buttonTransform = newButtonObj.GetComponent<RectTransform> ();
			float anchorPos = 1.0f - (buttonSize * i);
			buttonTransform.anchorMax = new Vector2 (1.0f, anchorPos);
			buttonTransform.anchorMin = new Vector2 (0.0f, anchorPos - buttonSize);
			buttonTransform.offsetMax = Vector2.zero;
			buttonTransform.offsetMin = Vector2.zero;

			configureButton (config, newButtonObj);

			newButtonObj.SetActive (true);
		}
	}

	private void buildAllButtonConfigs() {
		///// ###### CONFIGURE MAIN MENU BUTTONS ###### /////
		ButtonConfig continueButton = new ButtonConfig (
			"Continue this crazy journey.",
			delegate { configureUIWithEvent(GUIEvents.ContinueJourney); }
		);

		ButtonConfig suppliesButton = new ButtonConfig (
			"Check supplies",
			delegate { configureUIWithEvent(GUIEvents.DisplaySupplies); }
		);

		ButtonConfig paceButton = new ButtonConfig (
			"Set pace",
			delegate { configureUIWithEvent(GUIEvents.ChangePace); }
		);

		ButtonConfig rationsButton = new ButtonConfig (
			"Set rationing",
			delegate { configureUIWithEvent(GUIEvents.ChangeRations); }
		);

		ButtonConfig restButton = new ButtonConfig (
			"Stop to rest",
			delegate { configureUIWithEvent(GUIEvents.StopToRest); }
		);


		mainMenuButtonConfigs.Add (continueButton);
		mainMenuButtonConfigs.Add (suppliesButton);
		mainMenuButtonConfigs.Add (paceButton);
		mainMenuButtonConfigs.Add (rationsButton);
		mainMenuButtonConfigs.Add (restButton);

		ButtonConfig mainMenuButtonConfig = new ButtonConfig (
			"Take a break",
			delegate { configureUIWithEvent (GUIEvents.GoToMenu); }
		);

		configureButton (mainMenuButtonConfig, mainMenuButton);
	}
}
