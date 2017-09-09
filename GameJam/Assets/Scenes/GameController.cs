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
		player = new PlayerController ();
	}
	
	// Update is called once per frame
	void Update ()
	{
	}
}
