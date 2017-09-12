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

public enum ResourceTypes {
	Rations,
	Ammo,
	Scrap,
	Medicine,

	TYPE_COUNT
}

public class PlayerController
{
	public Pace pace;
	public Portion currentPortion;
	public Health currentHealth;
	public Hashtable resources;
	int healthNum = 100;
	public int distanceTravelled;
	public string illness = null;
	private bool gimme;
	System.Random rand = new System.Random();

  private int daysSick = 0;

	// Use this for initialization
	public PlayerController (Pace pace, Portion portion, int rations, int ammo, int scrap)
	{
		this.pace = pace;
		currentHealth = Health.Good;
		resources = new Hashtable ();
		resources.Add (ResourceTypes.Rations, rations);
		resources.Add (ResourceTypes.Ammo, ammo);
		resources.Add (ResourceTypes.Scrap, scrap);
		resources.Add (ResourceTypes.Medicine, 0);
		currentPortion = portion;
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
    ", 
			currentHealth, 
			healthNum, 
			currentPortion, 
			resources[ResourceTypes.Rations], 
			distanceTravelled, 
			pace, 
			resources[ResourceTypes.Ammo], 
			resources[ResourceTypes.Scrap]
		);
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

	private void UpdateRations () {
		ModifyResource (-(int)currentPortion, ResourceTypes.Rations);
	}

	public int GetResource(ResourceTypes type) {
		return (int)resources [type];
	}

	public void ModifyResource(int amount, ResourceTypes type) {
		int current = (int)resources [type];
		resources [type] = Mathf.Max (0, current + amount);
	}

	public void ModifyHealth(int amount) {
		healthNum = Mathf.Min (100, healthNum + amount);

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

	private void UpdateHealth ()
	{
		int currentRations = (int)resources [ResourceTypes.Rations];
		int healthScore = 0;

		int amountEaten;

		if ((int)currentPortion > currentRations) {
			amountEaten = currentRations;
		} else {
			amountEaten = (int)currentPortion;
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
			int num = rand.Next (1, 100);
			if (healthScore >= 4 && daysSick >= 3 && num >= 20) {
				// cured for good behavior
				illness = null;
				gimme = true;
				daysSick = 0;
			} else if ((healthScore < 4 && num >= 80) || (daysSick >= 10)) {
				// cured, but you got lucky (or you were sick for long enough)
				illness = null;
				daysSick = 0;
				gimme = true;
				healthScore -= 2;
			} else if (healthScore > 0) {
				// base illness subtraction.
				healthScore -= 15;
			} else if (currentHealth - 25 <= 0 && gimme) {
				Debug.Log ("gimme was used");
				gimme = false;
				healthScore -= 20;
			} else {
				// stop killing yourself
				healthScore -= 25;
			}
		}
		ModifyHealth (healthScore);
	}
}

static class EnumHelpers {
	public static string ToString(this ResourceTypes type) {
		switch (type) {
		case ResourceTypes.Rations:
			return "rations";
		case ResourceTypes.Ammo:
			return "ammo";
		case ResourceTypes.Scrap:
			return "scrap";
		case ResourceTypes.Medicine:
			return "medicine";
		default:
			return "UNKNOWN";
		}
	}

	public static string ToUpperString(this ResourceTypes type) {
		string lowerString = type.ToString ();
		return lowerString [0].ToString ().ToUpper() + lowerString.Substring (1);
	}
}