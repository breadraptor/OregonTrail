using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class GameController : MonoBehaviour
{

	const double REGULAR_UPDATE_INTERVAL = 3.0;
	const double RESTING_UPDATE_INTERVAL = 1.5;

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
				if (isResting) {
					daysToRest -= 1;
					if (daysToRest < 0) {
						isResting = false;
						updateInterval = REGULAR_UPDATE_INTERVAL;
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
}
