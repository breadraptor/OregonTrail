using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class GameController : MonoBehaviour
{

	const double REGULAR_UPDATE_INTERVAL = 3.0;
	const double RESTING_UPDATE_INTERVAL = 1.5;

  System.Random rand = new System.Random();
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

	// Use this for initialization
	void Start ()
	{

		player = new PlayerController (
			Pace.Normal,
			Portion.Normal,
			2000,
			200,
			200
		);

		world = new WorldController (
			20000,
			10,
			Weather.Clear,
			Season.Summer,
			0
		);
    populateEvents();
		frontEnd = GameObject.FindGameObjectWithTag ("FrontEndManager").GetComponent<FrontEndManager> ();
		guiMgr = GameObject.FindGameObjectWithTag ("UIManager").GetComponent<GUIManager> ();
		isResting = false;

		updateInterval = REGULAR_UPDATE_INTERVAL;
		shouldUpdate = false;
		nextUpdate = DateTime.Now.AddSeconds (updateInterval);
	}

	// Update is called once per frame
	void Update ()
	{
		if (shouldUpdate) {
			if (DateTime.Now >= nextUpdate) {
				nextUpdate = DateTime.Now.AddSeconds (updateInterval);
				UpdateWorldAndPlayer ();
        if (world.eventFlag) {
          guiMgr.eventText = "";
          StopWorldCoroutine();
          //string text = CreateEvent();
          guiMgr.eventText = CreateEvent();
          guiMgr.configureUIWithEvent(GUIEvents.WorldEvent);
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
				}
			}
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
    else if (e.resource == "health") {
      //todo
      Debug.Log("illness here");
      e.result = "";
    }
    else if (e.resource == "time") {
      int num = rand.Next(1, 5);
      e.result = "You lose " + num + " day(s).";
      world.day += num;
    }
    Debug.Log(e.result);
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

  void populateEvents() {
    worldEvents[0] = new WorldEvent("You get lost.", "time", false);
    worldEvents[1] = new WorldEvent("Find a small ration cache.", "rations", true);
    worldEvents[2] = new WorldEvent("You have the twengies.", "health", false);
    worldEvents[3] = new WorldEvent("You find an abandoned car.", "any", true);
    worldEvents[4] = new WorldEvent("Someone stole from your camp.", "any", false);
    worldEvents[5] = new WorldEvent("A kind traveller gave you some extra batteries.", "ammo", true);
    worldEvents[6] = new WorldEvent("Your backpack ripped.", "any", false);
    worldEvents[7] = new WorldEvent("Rough terrain.", "time", false);
    worldEvents[8] = new WorldEvent("You find an abandoned camp.", "any", true);
    worldEvents[9] = new WorldEvent("Some food spoiled.", "rations", false);
  }
}

class WorldEvent {

  public string description;
  public string resource;
  public bool good;
  public string result;

  public WorldEvent(string description, string resource, bool good) {
    this.description = description;
    this.resource = resource; 
    this.good = good; //true is good, false is bad
  }
}