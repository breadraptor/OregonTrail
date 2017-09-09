using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameController : MonoBehaviour
{

	PlayerController player;
	WorldController world;

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

		InvokeRepeating ("UpdateWorldAndPlayer", 0.0f, 3.0f);
		print ("sup");
	}

	// Update is called once per frame
	void Update ()
	{
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
