using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour
{
	private int maxChoicesCount = 8;

	public GUIEvents currentGuiState = GUIEvents.GoToMenu;

	public Font mainFont;
	public GameObject defaultText;
	public GameObject defaultButton;
	public GameObject choicesMenu;
	public GameObject locationTimeDisplay;
	public GameObject currentStatusDisplay;
	public GameObject alertDisplay;
	public GameObject alertAsset;
	public string eventText;

	private GameController mainGameController;

	ArrayList paceMenuButtonConfigs = new ArrayList ();
	ArrayList rationMenuButtonConfigs = new ArrayList ();
	ArrayList restMenuButtonConfigs = new ArrayList ();
	ArrayList eventBaseConfigs = new ArrayList ();

	private ButtonConfig takeBreakButtonConfig;
	private ButtonConfig continueJourneyButtonConfig;
	private ButtonConfig setOutButtonConfig;
	private ButtonConfig checkSuppliesButtonConfig;
	private ButtonConfig setPaceButtonConfig;
	private ButtonConfig setPortionsButtonConfig;
	private ButtonConfig restButtonConfig;
	private ButtonConfig talkToPeopleButtonConfig;
	private ButtonConfig tradeButtonConfig;
	private ButtonConfig checkScoreButtonConfig;
	private ButtonConfig startOverButtonConfig;
	private ButtonConfig backButtonConfig;

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
		case GUIEvents.PlayerDeath:
			displayGameOverMenu ();
			break;
		case GUIEvents.DoTrade:
			displayTradeMenu();
			break;
		case GUIEvents.HaveRandomConversation:
			displayTextBlurb ();
			break;
		default:
			Debug.Log ("UNRECOGNIZED EVENT: " + uiEvent.ToString ());
			break;
		}

		currentGuiState = uiEvent;
	}

	private void deactivateAllDisplays ()
	{
		
		locationTimeDisplay.SetActive (false);
		currentStatusDisplay.SetActive (false);
		choicesMenu.SetActive (false);
		alertDisplay.SetActive (false);
		alertAsset.SetActive (false);

	}

	private ArrayList getMainMenuButtonConfigs ()
	{
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
		updateLocationTimeDisplay (mainGameController.GetDestinationDescription ());
		locationTimeDisplay.SetActive (true);

		if (mainGameController.AtCurrentDestination ()) {
			updateCurrentStatusDisplay (mainGameController.GetLocationDescription ());
		} else {
			updateCurrentStatusDisplay (mainGameController.GetStatusText ());
		}
			
		currentStatusDisplay.SetActive (true);

		setChoicesMenuWithOptions (getMainMenuButtonConfigs());
	}

	private void displayContinueJourneyMenu ()
	{
		if (mainGameController.AtCurrentDestination ()) {

			updateCurrentStatusDisplay ("Where would you like to go?");

			List<Destination> nextDestinations = mainGameController.GetNextDestinations ();

			ArrayList destinationOptions = new ArrayList ();
			ButtonConfig stayButton = new ButtonConfig (
				"Stay here",
				delegate { setNewDestination (null, 0); }
			);
			destinationOptions.Add (stayButton);

			for (int i = 0; i < nextDestinations.Count; ++i) {
				Destination destination = (Destination)nextDestinations [i];
				Location location = LocationsManager.getLocationWithId (destination.id);
				ButtonConfig destinationButton = new ButtonConfig (
					location.name + " <- " + destination.distance + "mi.",
					delegate { setNewDestination (location, destination.distance); }
				);
				destinationOptions.Add (destinationButton);
			}

			setChoicesMenuWithOptions (destinationOptions);
		} else {
			mainGameController.StartWorldCoroutine ();
			ArrayList buttonConfigs = new ArrayList ();
			buttonConfigs.Add (takeBreakButtonConfig);
			setChoicesMenuWithOptions (buttonConfigs);
		}

		locationTimeDisplay.SetActive (true);
		currentStatusDisplay.SetActive (true);
	}

	private void displayGameOverMenu() {
		deactivateAllDisplays ();

		string gameOverStatus = "Game Over\n";
		if (mainGameController.AtFinalDestination ()) {
			gameOverStatus += "Congratulations! You've made it to safety!\n";
		} else {
			gameOverStatus += "Too bad, you didn't make it... this time!\n";
		}

		gameOverStatus += "Final score: { TODO }\n";

		updateCurrentStatusDisplay (gameOverStatus);
		currentStatusDisplay.SetActive (true);

		ArrayList gameOverButtonConfigs = new ArrayList ();
		gameOverButtonConfigs.Add (startOverButtonConfig);
		setChoicesMenuWithOptions (gameOverButtonConfigs);
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

			ArrayList buttonConfigs = new ArrayList ();
			buttonConfigs.Add (takeBreakButtonConfig);
			setChoicesMenuWithOptions (buttonConfigs);
		}
	}

	private void displaySuppliesMenu ()
	{
		string suppliesText = mainGameController.GetStatusText ();
		updateCurrentStatusDisplay (suppliesText);
		currentStatusDisplay.SetActive (true);

		ArrayList buttonConfigs = new ArrayList ();
		buttonConfigs.Add (backButtonConfig);
		setChoicesMenuWithOptions (buttonConfigs);
	}

	private void displayPaceMenu ()
	{
		updateCurrentStatusDisplay ("What travel pace would you like to set?...");
		currentStatusDisplay.SetActive (true);

		setChoicesMenuWithOptions (paceMenuButtonConfigs);
	}

	private void displayRationsMenu ()
	{
		updateCurrentStatusDisplay ("What portion size would you like to consume?...");
		currentStatusDisplay.SetActive (true);

		setChoicesMenuWithOptions (rationMenuButtonConfigs);
	}

	private void displayRestMenu ()
	{
		updateCurrentStatusDisplay ("How long would you like to rest?...");
		currentStatusDisplay.SetActive (true);

		setChoicesMenuWithOptions (restMenuButtonConfigs);
	}

	private void displayEventMenu ()
	{
		updateLocationTimeDisplay (mainGameController.GetDestinationDescription ());
		locationTimeDisplay.SetActive (true);

		EventStep randomEvent = EventsManager.getRandomTravelEvent ();
		updateEventMenuToStep (randomEvent);
	}

	private void updateEventMenuToStep(EventStep step) {
		ArrayList optionButtonConfigs = new ArrayList ();

		string displayText = step.displayText + "\n";
		for(int o = 0; o < step.options.Count; ++o) {
			int index = o + 1;
			EventOption option = EventsManager.getEventOption (step.options [o]);
			if (option.rewardIds.Count + option.costIds.Count > 0) {
				displayText += "Option " + index + ":\n";
			}
			if (option.rewardIds.Count > 0) {
				displayText += " Rewards\n";
				for (int r = 0; r < option.rewardIds.Count; ++r) {
					EventValue reward = EventsManager.getEventValue (option.rewardIds [r]);
					displayText += "   " + reward.resourceType.toUpperString() + ": " + reward.resourceValue + "\n";
				}
			}
			bool canAfford = true;
			if (option.costIds.Count > 0) {
				displayText += " Costs\n";
				for (int c = 0; c < option.costIds.Count; ++c) {
					EventValue cost = EventsManager.getEventValue (option.costIds [c]);
					canAfford &= mainGameController.playerCanAffordCost (cost);
					displayText += "   " + cost.resourceType.toUpperString () + ": " + cost.resourceValue + "\n";
				}
			}

			string buttonText;
			if (canAfford) {
				buttonText = option.buttonText;
			} else {
				buttonText = option.cantAffordButtonText;
			}

			ButtonConfig optionButtonConfig = new ButtonConfig (
				index + ") " + buttonText,
				delegate {
					mainGameController.ResolveEventOption (option);
					if(option.nextStepId != null && option.nextStepId != "" && canAfford) {
						EventStep nextStep = EventsManager.getEvent (option.nextStepId);
						updateEventMenuToStep (nextStep);
					} else {
						configureUIWithEvent(GUIEvents.GoToMenu);
					}
				}
			);

			optionButtonConfigs.Add (optionButtonConfig);
		}

		updateCurrentStatusDisplay (displayText);
		currentStatusDisplay.SetActive (true);

		setChoicesMenuWithOptions (optionButtonConfigs);
		choicesMenu.SetActive (true);
	}

	private void displayTradeMenu()
	{
		ArrayList optionsButtonConfigs = new ArrayList ();

		TradeEvent trade = mainGameController.GetRandomTradeEvent ();

		if (trade.realTrade) {
			if (trade.canAfford) {
				ButtonConfig agreeButton = new ButtonConfig (
					                           "Agree",
					                           delegate {
						mainGameController.ResolveTradeEvent (trade);
						configureUIWithEvent (GUIEvents.GoToMenu);
					}
				);
				ButtonConfig noThanksButton = new ButtonConfig (
					"No thanks",
					delegate { configureUIWithEvent(GUIEvents.GoToMenu); }
				);
				optionsButtonConfigs.Add (agreeButton);
				optionsButtonConfigs.Add (noThanksButton);
			} else {
				ButtonConfig cantButton = new ButtonConfig (
					"Can't afford, nevermind",
					delegate { configureUIWithEvent(GUIEvents.GoToMenu); }
				);
				optionsButtonConfigs.Add (cantButton);
			}

		} else {
			ButtonConfig tooBadButton = new ButtonConfig (
				"Nevermind",
				delegate { configureUIWithEvent(GUIEvents.GoToMenu); }
			);
			optionsButtonConfigs.Add (tooBadButton);
		}

		updateCurrentStatusDisplay (trade.tradeText);
		currentStatusDisplay.SetActive (true);

		setChoicesMenuWithOptions (optionsButtonConfigs);
	}

	public void displayTextBlurb() {
		List<string> blurbs = mainGameController.GetTextBlurbs ();
		int textBlurbIndex = UnityEngine.Random.Range (0, blurbs.Count * 2);
		if (textBlurbIndex >= blurbs.Count) {
			updateCurrentStatusDisplay ("You don't find anyone willing to talk to you.");
		} else {
			updateCurrentStatusDisplay (blurbs [textBlurbIndex]);
		}
		currentStatusDisplay.SetActive (true);

		ArrayList conversationButtons = new ArrayList ();
		ButtonConfig leaveButton = new ButtonConfig (
			"Leave",
			delegate { configureUIWithEvent(GUIEvents.GoToMenu); }
		);
		conversationButtons.Add (leaveButton);
		setChoicesMenuWithOptions (conversationButtons);
		choicesMenu.SetActive (true);
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

	private void startNewGame() {
		mainGameController.StartNewGame ();
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

		choicesMenu.SetActive (true);
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
			delegate { configureUIWithEvent (GUIEvents.HaveRandomConversation); }
		);

		tradeButtonConfig = new ButtonConfig (
			"Trade with somebody",
			delegate { configureUIWithEvent(GUIEvents.DoTrade); }
		);

		checkScoreButtonConfig = new ButtonConfig (
			"Final score",
			delegate { displayGameOverMenu(); }
		);

		startOverButtonConfig = new ButtonConfig (
			"Start New Game",
			delegate { startNewGame(); }
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

		ButtonConfig eventBreakButton = new ButtonConfig (
			"Take a break",
			delegate { configureUIWithEvent (GUIEvents.GoToMenu); }
		);

		eventBaseConfigs.Add (eventContinueButton);
		eventBaseConfigs.Add (eventBreakButton);

		takeBreakButtonConfig = new ButtonConfig (
			"Take a break",
			delegate { configureUIWithEvent (GUIEvents.GoToMenu); }
		);

		backButtonConfig = new ButtonConfig (
			"Back",
			delegate { configureUIWithEvent(GUIEvents.GoToMenu); }
		);
	}
}
