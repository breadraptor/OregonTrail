using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Health
{
	Dire,
	Poor,
	Fair,
	Good
}

public class PlayerController
{
	int pace;
	int currentHunger;
	int currentRations;
	int currentAmmo;
	Health currentHealth;
	int currentScrap;

	int distanceTravelled;


	// Use this for initialization
	public PlayerController (int pace, int hunger, int rations, int ammo, int scrap)
	{
		pace = pace;
		currentHunger = hunger;
		currentHealth = Health.Good;
		currentRations = rations;
		currentAmmo = ammo;
		currentScrap = scrap;
		distanceTravelled = 0;
	}
	
	// Update is called once per frame
	public void Update ()
	{
	}
}
