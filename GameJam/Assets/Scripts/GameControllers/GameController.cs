using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class GameController : MonoBehaviour
{

	const double REGULAR_UPDATE_INTERVAL = 1.0;//3.0;
	const double RESTING_UPDATE_INTERVAL = 0.5;//1.5;

	System.Random rand = new System.Random ();

	private PlayerController player;
	private WorldController world;

	Coroutine worldCoroutine;
	FrontEndManager frontEnd;
	GUIManager guiMgr;
	double updateInterval;

	bool shouldUpdate;
	DateTime nextUpdate;

	bool isResting;
	int daysToRest;

	Location currentDestination;
	int distanceToCurrentDestination;

	// Use this for initialization
	void Start ()
	{
		frontEnd = GameObject.FindGameObjectWithTag ("FrontEndManager").GetComponent<FrontEndManager> ();
		guiMgr = GameObject.FindGameObjectWithTag ("UIManager").GetComponent<GUIManager> ();

		StartNewGame ();
	}

	public void StartNewGame() {
		currentDestination = null;
		distanceToCurrentDestination = 0;

		player = new PlayerController (
			Pace.Normal,
			Portion.Normal,
			80,
			5,
			25
		);

		world = new WorldController (
			10,
			Weather.Clear,
			Season.Summer,
			1
		);

		isResting = false;

		EnsureInitialDestinationSet ();

		updateInterval = REGULAR_UPDATE_INTERVAL;
		shouldUpdate = false;
		nextUpdate = DateTime.Now.AddSeconds (updateInterval);

		guiMgr.configureUIWithEvent (GUIEvents.GoToMenu);
	}

	// Update is called once per frame
	void Update ()
	{
		if (shouldUpdate) {
      if (DateTime.Now >= nextUpdate) {
				nextUpdate = DateTime.Now.AddSeconds (updateInterval);
				UpdateWorldAndPlayer ();
				if (player.currentHealth == Health.Dead) {
					StopWorldCoroutine ();
					// TODO: Make front end manager show grave stone or something...
					guiMgr.configureUIWithEvent (GUIEvents.PlayerDeath);
				} else {
					if (world.eventFlag) {
						guiMgr.eventText = "";
						StopWorldCoroutine ();
						guiMgr.configureUIWithEvent (GUIEvents.WorldEvent);
						world.eventFlag = false;
					}
					if (isResting) {
						daysToRest -= 1;
						if (daysToRest < 0) {
							isResting = false;
							updateInterval = REGULAR_UPDATE_INTERVAL;
							guiMgr.configureUIWithEvent (GUIEvents.GoToMenu);
						} else {
							guiMgr.updateCurrentStatusDisplay ("Resting for " + (daysToRest + 1) + " more day(s)");
						}
					} else {
						distanceToCurrentDestination -= (int)player.pace;
						guiMgr.updateLocationTimeDisplay (GetDestinationDescription ());
						if (distanceToCurrentDestination <= 0) {
							distanceToCurrentDestination = 0;
							StopWorldCoroutine ();
							guiMgr.configureUIWithEvent (GUIEvents.GoToMenu);
						}
					}
				}
			}
		}
	}

	public void EnsureInitialDestinationSet() {
		if (currentDestination == null) {
			currentDestination = LocationsManager.getStartingLocation ();
			distanceToCurrentDestination = 0;
		}
	}

	public void RestForDays (int numDays)
	{
		isResting = true;
		daysToRest = numDays;
		updateInterval = RESTING_UPDATE_INTERVAL;
		SetPlayerPace (Pace.Resting);
	}

	public void StartWorldCoroutine ()
	{
    //frontEnd.StartPlayer();
		frontEnd.AssetUpdate ();
		guiMgr.updateCurrentStatusDisplay (GetStatusText ());
		nextUpdate = DateTime.Now.AddSeconds (updateInterval);
		shouldUpdate = true;
	}

	public void StopWorldCoroutine ()
	{
    	//frontEnd.StopPlayer();
    	shouldUpdate = false;
	}

	public void ResolveTradeEvent(TradeEvent trade) {
		switch (trade.rewardType) {
		case ResourceTypes.Ammo:
		case ResourceTypes.Scrap:
		case ResourceTypes.Rations:
			player.ModifyResource (trade.rewardAmount, trade.rewardType);
			break;
		case ResourceTypes.Medicine:
			player.ModifyHealth (trade.rewardAmount);
			break;
		}

		player.ModifyResource (-trade.costAmount, trade.costType);
	}

	public void ResolveEventOption(EventOption option) {
		for (int r = 0; r < option.rewardIds.Count; ++r) {
			EventValue reward = EventsManager.getEventValue (option.rewardIds [r]);
			switch (reward.resourceType) {
			case ResourceTypes.Ammo:
			case ResourceTypes.Scrap:
			case ResourceTypes.Rations:
				player.ModifyResource (reward.resourceValue, reward.resourceType);
				break;
			case ResourceTypes.Medicine:
				player.ModifyHealth (reward.resourceValue);
				break;
			case ResourceTypes.Time:
				Debug.Log ("ERROR! Cannot Reward Time!");
				break;
			}
		}

		for (int c = 0; c < option.costIds.Count; ++c) {
			EventValue cost = EventsManager.getEventValue (option.costIds [c]);
			switch (cost.resourceType) {
			case ResourceTypes.Ammo:
			case ResourceTypes.Scrap:
			case ResourceTypes.Rations:
				player.ModifyResource (-cost.resourceValue, cost.resourceType);
				break;
			case ResourceTypes.Medicine:
				// TODO: Can we _cost_ health?
				break;
			case ResourceTypes.Time:
				for (int i = 0; i < cost.resourceValue; ++i) {
					UpdateWorldAndPlayer ();
				}
				break;
			}
		}

		if(option.eventFlags != null) {
			world.AddEventFlags (option.eventFlags);
		}

		switch (option.diseaseEffect) {
		case HealthEffect.None:
		case HealthEffect.TYPE_COUNT:
			break;
		case HealthEffect.Cured:
			player.illness = HealthEffect.None;
			break;
		default:
			if (player.illness == HealthEffect.None) {
				// TODO: Multiple illnesses?
				player.illness = option.diseaseEffect;
			}
			break;
		}
	}

	public bool playerCanAffordCost(EventValue cost) {
		switch (cost.resourceType) {
		case ResourceTypes.Rations:
		case ResourceTypes.Ammo:
		case ResourceTypes.Scrap:
			return player.canAffordEventCost (cost);
		default:
			return true; // This covers cases like "costing" time
		}
	}

	public TradeEvent GetRandomTradeEvent() {
		if (CanTrade ()) {
			ArrayList costs = new ArrayList ();
			costs.Add (ResourceTypes.Rations);
			costs.Add (ResourceTypes.Ammo);
			costs.Add (ResourceTypes.Scrap);
			costs.Add (ResourceTypes.Scrap); // Duplicate scrap to increase odds
			int randomCostIndex = UnityEngine.Random.Range(0, costs.Count);
			ResourceTypes costType = (ResourceTypes)costs [randomCostIndex];

			ArrayList rewards = new ArrayList ();
			rewards.Add (ResourceTypes.Rations);
			rewards.Add (ResourceTypes.Ammo);
			rewards.Add (ResourceTypes.Scrap);
			rewards.Add (ResourceTypes.Medicine);
			rewards.Remove (costType);

			int randomRewardIndex = UnityEngine.Random.Range (0, rewards.Count);
			ResourceTypes rewardType = (ResourceTypes)rewards [randomRewardIndex];

			int rewardAmount = UnityEngine.Random.Range (1, ((int)currentDestination.tradeValue * 3));
			int costAmount = rewardAmount + UnityEngine.Random.Range (1, currentDestination.tradeValue);

			bool canAfford = player.GetResource (costType) >= costAmount;

			string tradeMessage = "You find someone willing to trade " + rewardAmount + " " + rewardType.ToString() + " for " + costAmount + " " + costType.ToString();

			return new TradeEvent (true, canAfford, tradeMessage, costType, costAmount, rewardType, rewardAmount);
		} else {
			string noTradeMessage = "You can't find anyone who wants to trade.";
			return new TradeEvent (false, false, noTradeMessage, ResourceTypes.TYPE_COUNT, 0, ResourceTypes.TYPE_COUNT, 0);
		}
	}

	public string GetStatusText ()
	{
		string fmtString = @"Date: {0}
Health: {1}
Rations: {2}
Miles Travelled: {3} miles";
		string dateString = string.Format ("{0} {1}", world.currentSeason, world.day);
		return string.Format (
			fmtString,
			dateString,
			player.currentHealth,
			player.GetResource(ResourceTypes.Rations),
			player.distanceTravelled
		);
	}

	public Weather GetWeather ()
	{
		return world.currentWeather;
	}

	public Season GetSeason ()
	{
		return world.currentSeason;
	}

	public void SetPlayerPace (Pace newPace)
	{
		player.pace = newPace;
	}

	public Pace GetPlayerPace ()
	{
		return player.pace;
	}

	public Portion GetPlayerPortion ()
	{
		return player.currentPortion;
	}

	public void SetPlayerPortion (Portion newPortion)
	{
		player.currentPortion = newPortion;
	}

	public bool AtCurrentDestination ()
	{
		return distanceToCurrentDestination <= 0;
	}

	public bool AtFinalDestination() {
		return AtCurrentDestination () && LocationsManager.locationIsFinal (currentDestination.id);
	}

	public bool CanTrade() {
		return AtCurrentDestination() && currentDestination.tradeValue > 0;
	}

	public void SetNewDestination (Location newLocation, int distance)
	{
		currentDestination = newLocation;
		distanceToCurrentDestination = distance;
	}

	public List<Destination> GetNextDestinations ()
	{
		return currentDestination.destinations;
	}

	public string GetLocationDescription() {
		return currentDestination.description;
	}

	public List<String> GetTextBlurbs() {
		return currentDestination.textBlurbs;
	}

	public string GetDestinationDescription() {
		if (AtCurrentDestination ()) {
			return "At " + currentDestination.name;
		} else {
			return "Traveling to " + currentDestination.name + "\nDistance left: " + distanceToCurrentDestination;
		}
	}

	void UpdateWorldAndPlayer ()
	{

		guiMgr.updateCurrentStatusDisplay (GetStatusText ());
//		print (string.Format (@"
//----- Player -----
//    {0}
//----- World -----
//    {1}
//    ",
//			player.toString (),
//			world.toString ()
//		));
		player.Update ();
		world.Update ();
		frontEnd.AssetUpdate ();
	}
}

public class TradeEvent
{
	public bool realTrade; // false means no one wants to trade
	public bool canAfford;
	public string tradeText;
	public ResourceTypes costType;
	public int costAmount;
	public ResourceTypes rewardType;
	public int rewardAmount;

	public TradeEvent(bool realTradeEvent, bool canAffordTrade, string tradeEventText, ResourceTypes tradeCostType, int tradeCostAmount, ResourceTypes tradeRewardType, int tradeRewardAmount) {
		realTrade = realTradeEvent;
		canAfford = canAffordTrade;
		tradeText = tradeEventText;
		costType = tradeCostType;
		costAmount = tradeCostAmount;
		rewardType = tradeRewardType;
		rewardAmount = tradeRewardAmount;
	}
}