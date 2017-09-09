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
			20,
			1,
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
    StartWorldCoroutine();
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
