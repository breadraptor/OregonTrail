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

public enum Pace {
  Resting = 0,
  Crawling = 5,
  Slow = 10,
  Normal = 20,
  Quick = 25,
  Strenuous = 30
}

public enum Portion {
  Starving = 1,
  Meager = 2,
  Moderate = 3,
  Normal = 4,
  Plentiful = 5
}

public class PlayerController
{
  public Pace pace;
  public Portion currentPortion;
  public int currentRations;
  public int currentAmmo;
  public Health currentHealth;
  public int currentScrap;
	int healthNum = 100;
	public int distanceTravelled;
  public string illness = null;
  System.Random rand = new System.Random();

  private int daysSick = 0;

	// Use this for initialization
	public PlayerController (Pace pace, Portion portion, int rations, int ammo, int scrap)
	{
		this.pace = pace;
		currentPortion = portion;
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
      Portion: {2},
      Rations: {3},
      DistanceTravelled: {4},
      Pace: {5},
      Ammo: {6},
      Scrap: {7}
    ", currentHealth, healthNum, currentPortion, currentRations, distanceTravelled, pace, currentAmmo, currentScrap);
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
		distanceTravelled += (int)pace;
	}

	private void UpdateRations ()
	{
		int newRations = currentRations - (int) currentPortion;
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

		if ((int) currentPortion > currentRations) {
			amountEaten = currentRations;
		} else {
			amountEaten = (int) currentPortion;
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
    switch (pace) {
      case Pace.Resting:
        healthScore += 3;
        break;
      case Pace.Crawling:
        healthScore += 2;
        break;
      case Pace.Slow:
        healthScore += 1;
        break;
      case Pace.Quick:
        healthScore -= 1;
        break;
      case Pace.Strenuous:
        healthScore -= 2;
        break;

      default:
        //this is chill
        break;
    }

    if (illness != null) {
      daysSick++;
      int num = rand.Next(1, 100);
      if (healthScore >= 4 && daysSick >= 3 && num >= 20) {
        // cured for good behavior
        illness = null;
        daysSick = 0;
      }
      else if ((healthScore < 4 && num >= 80) || (daysSick >= 15)) {
        // cured, but you got lucky (or you were sick for long enough)
        illness = null;
        daysSick = 0;
        healthScore -= 2;
      }
      else if (healthScore > 0) {
        // base illness subtraction.
        healthScore -= 15;
      }
      else {
        // stop killing yourself
        healthScore -= 25;
      }
    }

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
