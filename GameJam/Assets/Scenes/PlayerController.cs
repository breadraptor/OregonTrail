using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Health
{
	Dead,
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
		this.pace = pace;
		currentHunger = hunger;
		currentHealth = Health.Good;
		currentRations = rations;
		currentAmmo = ammo;
		currentScrap = scrap;
		distanceTravelled = 0;
	}

	public string toString ()
	{
		return string.Format (@"
      Health: {0},
      Hunger: {1},
      Rations: {2},
      DistanceTravelled: {3},
      Pace: {4},
      Ammo: {5},
      Scrap: {6}
    ", healthToString (currentHealth), currentHunger, currentRations, distanceTravelled, pace, currentAmmo, currentScrap);
	}


	private string healthToString (Health h)
	{
		switch (h) {
		case Health.Good:
			return "Good";
		case Health.Fair:
			return "Fair";
		case Health.Poor:
			return "Poor";
		case Health.Dire:
			return "Dire";
		case Health.Dead:
			return "Dead";
		default:
			return "BROKEN";
		}
	}

	// Update is called once per frame
	public void Update ()
	{
		UpdateHealth ();
		UpdateRations ();
		UpdateDistanceTravelled ();
	}

	private void UpdateDistanceTravelled ()
	{
		distanceTravelled += pace;
	}

	private void UpdateRations ()
	{
		int newRations = currentRations - currentHunger;
		if (newRations < 0) {
			currentRations = 0;
		} else {
			currentRations = newRations;
		}
	}

	private void UpdateHealth ()
	{
		int healthScore = 0;

		int amountEaten;

		if (currentHunger > currentRations) {
			amountEaten = currentRations;
		} else {
			amountEaten = currentHunger;
		}

		switch (amountEaten) {
		case 0:
        //starving
			healthScore -= 4;
			break;

		case 1:
        //hardly any
			healthScore -= 2;
			break;

		case 2:
        //not quite enough
			healthScore -= 1;
			break;

		case 3:
        //normal amount, no change
			break;

		case 4:
        //plenty
			healthScore += 1;
			break;

		case 5:
        //lots
			healthScore += 2;
			break;

		default:
        //if they eat more than this?
			healthScore += 3;
			break;

		}

		//check if pace is extra fast or extra slow
		if (pace > 35) {
			healthScore -= 2;
		} else if (pace > 25) {
			healthScore -= 1;
		} else if (pace < 15) {
			healthScore += 1;
		} else if (pace < 10) {
			healthScore += 2;
		}

		//move health up or down depending on value
		if (healthScore > 0) {
			switch (currentHealth) {
			case Health.Fair:
				currentHealth = Health.Good;
				break;
			case Health.Poor:
				currentHealth = Health.Fair;
				break;
			case Health.Dire:
				currentHealth = Health.Poor;
				break;
			}
		} else if (healthScore < 0) {
			switch (currentHealth) {
			case Health.Good:
				currentHealth = Health.Fair;
				break;
			case Health.Fair:
				currentHealth = Health.Poor;
				break;
			case Health.Poor:
				currentHealth = Health.Dire;
				break;
			case Health.Dire:
				currentHealth = Health.Dead;
				break;
			}
		}


	}
}
