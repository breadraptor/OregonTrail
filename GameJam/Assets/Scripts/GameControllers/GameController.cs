using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class GameController : MonoBehaviour
{

	const double REGULAR_UPDATE_INTERVAL = 1.0;//3.0;
	const double RESTING_UPDATE_INTERVAL = 0.5;//1.5;

	System.Random rand = new System.Random ();
	private WorldEvent[] worldEvents = new WorldEvent[10];

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
		populateEvents ();
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
			2000,
			200,
			200
		);

		world = new WorldController (
			0,
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
						//string text = CreateEvent();
						guiMgr.eventText = CreateEvent ();
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
			Locations.init ();
			currentDestination = Locations.getStartingLocation ();
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
		frontEnd.AssetUpdate ();
		guiMgr.updateCurrentStatusDisplay (GetStatusText ());
		nextUpdate = DateTime.Now.AddSeconds (updateInterval);
		shouldUpdate = true;
	}

	public void StopWorldCoroutine ()
	{
		shouldUpdate = false;
	}

  private string CreateEvent() {
    //todo should put events in a text file and read that in probably
    int eventIndex = rand.Next(0, 9);
    WorldEvent e = worldEvents[eventIndex];
    if (e.resource == "any") {
      // todo make this able to affect more than one resource
      int num = rand.Next(1, 2);
      if (num == 1) {
        e.resource = "rations";
      }
      else {
        e.resource = "scrap";
      }  
    }
    if (e.resource == "rations") {
      int num = rand.Next(3, 10);
      if (e.good) {
        e.result = "You gain " + num + " rations.";
        player.currentRations += num;
      }
      else {
        e.result = "You lose " + num + " rations.";
        player.currentRations -= num;
      }
    }
    else if (e.resource == "scrap") {
      int num = rand.Next(5, 15);
      if (e.good) {
        e.result = "You gain " + num + " scrap.";
        player.currentScrap += num;
      }
      else {
        e.result = "You lose " + num + " scrap.";
        player.currentScrap -= num;
      }
    }
    else if (e.resource == "ammo") {
      int num = rand.Next(10, 30);
      if (e.good) {
        e.result = "You gain " + num + " ammo.";
        player.currentAmmo += num;
      }
      else {
        e.result = "You lose " + num + " ammo.";
        player.currentAmmo -= num;
      }
    }
    else if (e.resource == "health" && player.illness == null) {
      int num = rand.Next(1, 3);
      if (num == 1) {
        player.illness = "twengies";
      }
      else if (num == 2) {
        player.illness = "dysentery";
      }
      else {
        player.illness = "loon eye";
      }
      e.result = player.illness + ".";
    }
    else if (e.resource == "time") {
      int num = rand.Next(1, 5);
      e.result = "You lose " + num + " day(s).";
      world.day += num;
    }
    return e.description + " " + e.result;
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
			player.currentRations,
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
		return AtCurrentDestination () && Locations.locationIsFinal (currentDestination.id);
	}

	public bool CanTrade() {
		return AtCurrentDestination () && currentDestination.tradeValue > 0;
	}

	public void SetNewDestination (Location newLocation, int distance)
	{
		currentDestination = newLocation;
		distanceToCurrentDestination = distance;
	}

	public ArrayList GetNextDestinations ()
	{
		return currentDestination.destinations;
	}

	public string GetLocationDescription() {
		return currentDestination.description;
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
		print (string.Format (@"
----- Player -----
    {0}
----- World -----
    {1}
    ",
			player.toString (),
			world.toString ()
		));
		player.Update ();
		world.Update ();
		frontEnd.AssetUpdate ();
	}

	void populateEvents ()
	{
		worldEvents [0] = new WorldEvent ("You get lost.", "time", false);
		worldEvents [1] = new WorldEvent ("Find a small ration cache.", "rations", true);
		worldEvents [2] = new WorldEvent ("You have the ", "health", false);
		worldEvents [3] = new WorldEvent ("You find an abandoned car.", "any", true);
		worldEvents [4] = new WorldEvent ("Someone stole from your camp.", "any", false);
		worldEvents [5] = new WorldEvent ("A traveller gave you some extra batteries.", "ammo", true);
		worldEvents [6] = new WorldEvent ("Your backpack ripped.", "any", false);
		worldEvents [7] = new WorldEvent ("Rough terrain.", "time", false);
		worldEvents [8] = new WorldEvent ("You find an abandoned camp.", "any", true);
		worldEvents [9] = new WorldEvent ("Some food spoiled.", "rations", false);
	}
}

class WorldEvent
{

	public string description;
	public string resource;
	public bool good;
	public string result;

	public WorldEvent (string description, string resource, bool good)
	{
		this.description = description;
		this.resource = resource; 
		this.good = good; //true is good, false is bad
	}
}