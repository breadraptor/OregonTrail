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

	int healthNum = 100;
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
	  HealthNum: {1},
      Hunger: {2},
      Rations: {3},
      DistanceTravelled: {4},
      Pace: {5},
      Ammo: {6},
      Scrap: {7}
    ", healthToString (currentHealth), healthNum, currentHunger, currentRations, distanceTravelled, pace, currentAmmo, currentScrap);
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
//		if (healthScore > 0) {
//			switch (currentHealth) {
//			case Health.Fair:
//				currentHealth = Health.Good;
//				break;
//			case Health.Poor:
//				currentHealth = Health.Fair;
//				break;
//			case Health.Dire:
//				currentHealth = Health.Poor;
//				break;
//			}
//		} else if (healthScore < 0) {
//			switch (currentHealth) {
//			case Health.Good:
//				currentHealth = Health.Fair;
//				break;
//			case Health.Fair:
//				currentHealth = Health.Poor;
//				break;
//			case Health.Poor:
//				currentHealth = Health.Dire;
//				break;
//			case Health.Dire:
//				currentHealth = Health.Dead;
//				break;
//			}
//		}
		healthNum += healthScore;
		if (healthNum > 100) {
			healthNum = 100;
		}

		if (healthNum > 75) {
			currentHealth = Health.Good;
		} else if (healthNum > 50) {
			currentHealth = Health.Fair;
		} else if (healthNum > 25) {
			currentHealth = Health.Poor;
		} else if (healthNum > 10) {
			currentHealth = Health.Dire;
		} else if (healthNum <= 0) {
			currentHealth = Health.Dead;
		}

	}
}
