using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameController : MonoBehaviour
{

	public PlayerController player;
	public WorldController world;
	Coroutine worldCoroutine;

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
	}


	// Update is called once per frame
	void Update ()
	{
	}

  public void StartWorldCoroutine() {
    StartCoroutine ("DriveWorldOnInterval");
  }

  public void StopWorldCoroutine() {
    print ("stopping");
    StopCoroutine("DriveWorldOnInterval");
  }

	private IEnumerator DriveWorldOnInterval ()
	{
		while (true) {
			yield return new WaitForSeconds (3);
			UpdateWorldAndPlayer ();
		}
	}

  public string GetStatusText() {
    string fmtString = @"Date: {0}
    Health: {1}
    Rations: {2}
    Miles Travelled: {3} miles";
    string dateString = string.Format("{0} {1}", world.currentSeason, world.day);
    return string.Format(
      fmtString,
      dateString,
      player.currentHealth,
      player.currentRations,
      player.distanceTravelled
    );

  }


	void UpdateWorldAndPlayer ()
	{

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
	}
}
