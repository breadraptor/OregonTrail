using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour
{

	public Font mainFont;
	private int maxChoicesCount = 8;
	public GameObject defaultText;
	public GameObject defaultButton;
	public GameObject choicesMenu;
	public GameObject locationTimeDisplay;
	public GameObject currentStatusDisplay;
	public GameObject alertDisplay;
	public GameObject alertAsset;
	public GameObject mainMenuButton;
	public string eventText;

	private GameController mainGameController;

	ArrayList paceMenuButtonConfigs = new ArrayList ();
	ArrayList rationMenuButtonConfigs = new ArrayList ();
	ArrayList restMenuButtonConfigs = new ArrayList ();
	ArrayList eventBaseConfigs = new ArrayList ();

	private ButtonConfig continueJourneyButtonConfig;
	private ButtonConfig setOutButtonConfig;
	private ButtonConfig checkSuppliesButtonConfig;
	private ButtonConfig setPaceButtonConfig;
	private ButtonConfig setPortionsButtonConfig;
	private ButtonConfig restButtonConfig;
	private ButtonConfig talkToPeopleButtonConfig;
	private ButtonConfig tradeButtonConfig;
	private ButtonConfig checkScoreButtonConfig;

	void Start ()
	{

		mainGameController = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameController> ();

		buildAllButtonConfigs ();

		mainGameController.EnsureInitialDestinationSet ();

		configureUIWithEvent (GUIEvents.GoToMenu);

	}


	public void configureUIWithEvent (GUIEvents uiEvent)
	{
		deactivateAllDisplays ();

		switch (uiEvent) {
		case GUIEvents.GoToMenu:
			mainGameController.StopWorldCoroutine ();
			displayMainMenu ();
			break;
		case GUIEvents.ContinueJourney:
			displayContinueJourneyMenu ();
			break;
		case GUIEvents.DisplaySupplies:
			displaySuppliesMenu ();
			break;
		case GUIEvents.ChangePace:
			displayPaceMenu ();
			break;
		case GUIEvents.ChangeRations:
			displayRationsMenu ();
			break;
		case GUIEvents.StopToRest:
			displayRestMenu ();
			break;
		case GUIEvents.WorldEvent:
			displayEventMenu ();
			break;
		default:
			Debug.Log ("UNRECOGNIZED EVENT: " + uiEvent.ToString ());
			break;
		}
	}

	private void deactivateAllDisplays ()
	{
		
		locationTimeDisplay.SetActive (false);
		currentStatusDisplay.SetActive (false);
		choicesMenu.SetActive (false);
		mainMenuButton.SetActive (false);
		alertDisplay.SetActive (false);
		alertAsset.SetActive (false);

	}

	private ArrayList getMainMenuButtonConfigs() {
		ArrayList mainMenuConfigs = new ArrayList ();

		if (mainGameController.AtFinalDestination ()) {
			mainMenuConfigs.Add (checkScoreButtonConfig);
		} else if (mainGameController.AtCurrentDestination ()) {
			mainMenuConfigs.Add (setOutButtonConfig);
			mainMenuConfigs.Add (talkToPeopleButtonConfig);
			if (mainGameController.CanTrade ()) {
				mainMenuConfigs.Add (tradeButtonConfig);
			}
		} else {
			mainMenuConfigs.Add (continueJourneyButtonConfig);
		}

		mainMenuConfigs.Add (checkSuppliesButtonConfig);

		if (!mainGameController.AtFinalDestination ()) {
			mainMenuConfigs.Add (setPaceButtonConfig);
			mainMenuConfigs.Add (setPortionsButtonConfig);
			mainMenuConfigs.Add (restButtonConfig);
		}

		return mainMenuConfigs;
	}

	private void displayMainMenu ()
	{
		Debug.Log ("Going to menu");
		updateLocationTimeDisplay (mainGameController.GetDestinationDescription ());
		locationTimeDisplay.SetActive (true);

		if (mainGameController.AtCurrentDestination ()) {
			updateCurrentStatusDisplay (mainGameController.GetLocationDescription ());
		} else {
			updateCurrentStatusDisplay (mainGameController.GetStatusText ());
		}
			
		currentStatusDisplay.SetActive (true);

		setChoicesMenuWithOptions (getMainMenuButtonConfigs());
		choicesMenu.SetActive (true);
	}

	private void displayContinueJourneyMenu ()
	{
		if (mainGameController.AtCurrentDestination ()) {

			updateCurrentStatusDisplay ("Where would you like to go?");

			ArrayList nextDestinations = mainGameController.GetNextDestinations ();

			ArrayList destinationOptions = new ArrayList ();
			ButtonConfig stayButton = new ButtonConfig (
				                          "Stay here",
				                          delegate {
					setNewDestination (null, 0);
				}
			                          );
			destinationOptions.Add (stayButton);

			for (int i = 0; i < nextDestinations.Count; ++i) {
				Destination destination = (Destination)nextDestinations [i];
				Location location = Locations.getLocationWithId (destination.id);
				ButtonConfig destinationButton = new ButtonConfig (
					                                 location.name + " <- " + destination.distance + "mi.",
					                                 delegate {
						setNewDestination (location, destination.distance);
					}
				                                 );
				destinationOptions.Add (destinationButton);
			}

			setChoicesMenuWithOptions (destinationOptions);
			choicesMenu.SetActive (true);

		} else {
			mainGameController.StartWorldCoroutine ();
			mainMenuButton.SetActive (true);
		}

		locationTimeDisplay.SetActive (true);
		currentStatusDisplay.SetActive (true);
	}

	private void setNewDestination (Location newLocation, int distance)
	{
		if (distance == 0) {
			configureUIWithEvent (GUIEvents.GoToMenu);
		} else {
			deactivateAllDisplays ();
			mainGameController.SetNewDestination (newLocation, distance);

			updateLocationTimeDisplay (mainGameController.GetDestinationDescription ());

			mainGameController.StartWorldCoroutine ();
			locationTimeDisplay.SetActive (true);
			currentStatusDisplay.SetActive (true);
			mainMenuButton.SetActive (true);
		}
	}

	private void displaySuppliesMenu ()
	{
		string suppliesText = mainGameController.GetStatusText ();
		updateCurrentStatusDisplay (suppliesText);
		currentStatusDisplay.SetActive (true);

		mainMenuButton.SetActive (true);
	}

	private void displayPaceMenu ()
	{
		updateCurrentStatusDisplay ("What travel pace would you like to set?...");
		currentStatusDisplay.SetActive (true);

		setChoicesMenuWithOptions (paceMenuButtonConfigs);
		choicesMenu.SetActive (true);
	}

	private void displayRationsMenu ()
	{
		updateCurrentStatusDisplay ("What portion size would you like to consume?...");
		currentStatusDisplay.SetActive (true);

		setChoicesMenuWithOptions (rationMenuButtonConfigs);
		choicesMenu.SetActive (true);
	}

	private void displayRestMenu ()
	{
		updateCurrentStatusDisplay ("How long would you like to rest?...");
		currentStatusDisplay.SetActive (true);

		setChoicesMenuWithOptions (restMenuButtonConfigs);
		choicesMenu.SetActive (true);
	}

	private void displayEventMenu ()
	{
		updateAlertDisplay (eventText);
		alertDisplay.SetActive (true);
		alertAsset.SetActive (true);
		setChoicesMenuWithOptions (eventBaseConfigs);
		choicesMenu.SetActive (true);
		currentStatusDisplay.SetActive (true);
	}

	public void updateLocationTimeDisplay (string text)
	{
		locationTimeDisplay.GetComponent<Text> ().text = text;
	}

	public void updateCurrentStatusDisplay (string text)
	{
		currentStatusDisplay.GetComponent<Text> ().text = text;
	}

	public void updateAlertDisplay (string text)
	{
		alertDisplay.GetComponent<Text> ().text = text;
	}

	private void setPace (Pace pace)
	{
		mainGameController.SetPlayerPace (pace);
		configureUIWithEvent (GUIEvents.GoToMenu);
	}

	private void setPortions (Portion portion)
	{
		mainGameController.SetPlayerPortion (portion);
		configureUIWithEvent (GUIEvents.GoToMenu);
	}

	private void setRestTime (int restTime)
	{
		if (restTime > 0) {
			mainGameController.RestForDays (restTime);
			deactivateAllDisplays ();
			updateCurrentStatusDisplay ("Resting for " + restTime + " days...");
			currentStatusDisplay.SetActive (true);
			mainGameController.StartWorldCoroutine ();
		} else {
			configureUIWithEvent (GUIEvents.GoToMenu);
		}
	}

	void setChoicesMenuWithOptions (ArrayList buttonConfigs)
	{

		Button[] currentButtons = choicesMenu.GetComponentsInChildren<Button> ();
		for (int i = 0; i < currentButtons.Length; ++i) {
			DestroyImmediate (currentButtons [i].gameObject);
		}

		float buttonSize = 1.0f / (float)maxChoicesCount;
		float buttonsTop = buttonSize * buttonConfigs.Count;

		for (int i = 0; i < buttonConfigs.Count && i < maxChoicesCount; ++i) {
			ButtonConfig config = (ButtonConfig)buttonConfigs [i];
			GameObject newButtonObj = Instantiate (defaultButton, choicesMenu.transform);
			newButtonObj.name = "Choice Button " + i;
			RectTransform buttonTransform = newButtonObj.GetComponent<RectTransform> ();
			float anchorPos = buttonsTop - (buttonSize * i);
			buttonTransform.anchorMax = new Vector2 (1.0f, anchorPos);
			buttonTransform.anchorMin = new Vector2 (0.0f, anchorPos - buttonSize);
			buttonTransform.offsetMax = Vector2.zero;
			buttonTransform.offsetMin = Vector2.zero;

			configureButton (config, newButtonObj);

			newButtonObj.SetActive (true);
		}
	}

	private void configureButton (ButtonConfig config, GameObject button)
	{
		button.GetComponentInChildren<Text> ().text = config.displayText;
		button.GetComponent<Button> ().onClick.AddListener (config.action);
	}

	private void buildAllButtonConfigs ()
	{
		///// ###### CONFIGURE MAIN MENU BUTTONS ###### /////
		continueJourneyButtonConfig = new ButtonConfig (
			"Continue on your journey",
			delegate { configureUIWithEvent (GUIEvents.ContinueJourney); }
		);

		setOutButtonConfig = new ButtonConfig (
			"Set out",
			delegate { configureUIWithEvent (GUIEvents.ContinueJourney); }
		);

		checkSuppliesButtonConfig = new ButtonConfig (
			"Check supplies",
			delegate { configureUIWithEvent (GUIEvents.DisplaySupplies); }
		);

		setPaceButtonConfig = new ButtonConfig (
			"Set travel pace",
			delegate { configureUIWithEvent (GUIEvents.ChangePace); }
		);

		setPortionsButtonConfig = new ButtonConfig (
			"Set portion size",
			delegate { configureUIWithEvent (GUIEvents.ChangeRations); }
		);

		restButtonConfig = new ButtonConfig (
			"Stop to rest",
			delegate { configureUIWithEvent (GUIEvents.StopToRest); }
		);

		talkToPeopleButtonConfig = new ButtonConfig (
			"Talk to somebody",
			delegate { Debug.Log("TODO: Talk to people..."); }
		);

		tradeButtonConfig = new ButtonConfig (
			"Trade with somebody",
			delegate { Debug.Log("TODO: Trade with people..."); }
		);

		checkScoreButtonConfig = new ButtonConfig (
			"Final score",
			delegate { Debug.Log("TODO: Check your final score..."); }
		);

		///// ###### CONFIGURE JOURNEY SETTINGS BUTTONS ###### /////
		ButtonConfig paceCrawlingButton = new ButtonConfig (
			"Crawling",
			delegate { setPace (Pace.Crawling); }
		);

		ButtonConfig paceSlowButton = new ButtonConfig (
			"Slow",
			delegate { setPace (Pace.Slow); }
		);

		ButtonConfig paceNormalButton = new ButtonConfig (
			"Normal",
			delegate { setPace (Pace.Normal); }
		);

		ButtonConfig paceQuickButton = new ButtonConfig (
			"Quick",
			delegate { setPace (Pace.Quick); }
		);

		ButtonConfig paceStrenuousButton = new ButtonConfig (
			"Strenuous",
			delegate { setPace (Pace.Strenuous); }
		);

		paceMenuButtonConfigs.Add (paceCrawlingButton);
		paceMenuButtonConfigs.Add (paceSlowButton);
		paceMenuButtonConfigs.Add (paceNormalButton);
		paceMenuButtonConfigs.Add (paceQuickButton);
		paceMenuButtonConfigs.Add (paceStrenuousButton);

		ButtonConfig rationsStarvingButton = new ButtonConfig (
			"Starving",
			delegate { setPortions (Portion.Starving); }
		);

		ButtonConfig rationsMeagerButton = new ButtonConfig (
			"Meager",
			delegate { setPortions (Portion.Meager); }
		);

		ButtonConfig rationsModerateButton = new ButtonConfig (
			"Moderate",
			delegate { setPortions (Portion.Moderate); }
		);

		ButtonConfig rationsNormalButton = new ButtonConfig (
			"Normal",
			delegate { setPortions (Portion.Normal); }
		);

		ButtonConfig rationsPlentifulButton = new ButtonConfig (
			"Plentiful",
			delegate { setPortions (Portion.Plentiful); }
		);

		rationMenuButtonConfigs.Add (rationsStarvingButton);
		rationMenuButtonConfigs.Add (rationsMeagerButton);
		rationMenuButtonConfigs.Add (rationsModerateButton);
		rationMenuButtonConfigs.Add (rationsNormalButton);
		rationMenuButtonConfigs.Add (rationsPlentifulButton);

		ButtonConfig restNoneButton = new ButtonConfig (
			"Nevermind",
			delegate { setRestTime (0); }
		);

		ButtonConfig restOneDayButton = new ButtonConfig (
			"One day",
			delegate { setRestTime (1); }
		);

		ButtonConfig restThreeDaysButton = new ButtonConfig (
			"Three days",
			delegate { setRestTime (3); }
		);

		ButtonConfig restFiveDaysButton = new ButtonConfig (
			"Five days",
			delegate { setRestTime (5); }
		);

		restMenuButtonConfigs.Add (restNoneButton);
		restMenuButtonConfigs.Add (restOneDayButton);
		restMenuButtonConfigs.Add (restThreeDaysButton);
		restMenuButtonConfigs.Add (restFiveDaysButton);

		ButtonConfig eventContinueButton = new ButtonConfig (
			"Continue",
			delegate { configureUIWithEvent (GUIEvents.ContinueJourney); }
		);
		eventBaseConfigs.Add (eventContinueButton);

		ButtonConfig mainMenuButtonConfig = new ButtonConfig (
			"Take a break",
			delegate { configureUIWithEvent (GUIEvents.GoToMenu); }
		);

		configureButton (mainMenuButtonConfig, mainMenuButton);
	}
}
