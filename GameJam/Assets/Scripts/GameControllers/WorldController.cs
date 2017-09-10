using System;
using UnityEngine;

public enum Weather
{
	Snow,
	Cold,
	Clear,
	Raining,
	Hot
}

public enum Season
{
	Summer,
	Spring,
	Winter,
	Fall
}

public class WorldController
{
	int probabilityStep = 5;
  public bool eventFlag = false;
	int totalDistance;
	double eventProbability;
	public Weather currentWeather {get; set;}
	public Season currentSeason {get; set;}
	int seasonLength = 30;
	System.Random rand = new System.Random ();
	public int day {get; set;}

	public WorldController (int totalDistance, int startingProbability, Weather startingWeather, Season season, int day)
	{
		eventProbability = startingProbability;
		currentWeather = startingWeather;
		this.totalDistance = totalDistance;
		this.day = day;
		currentSeason = season;
	}

	public string toString ()
	{
		return string.Format (@"
      Season: {0},
      Weather: {1},
      Day: {2},
      EventProbability: {3}
    ",
			currentSeason,
			currentWeather,
			day,
			eventProbability
		);
	}


	public void Update ()
	{
		UpdateEventProbability ();
		UpdateSeason ();
		UpdateWeather ();
		day++;
	}

	private void UpdateEventProbability ()
	{
		eventProbability = eventProbability + probabilityStep;
    probabilityStep = rand.Next(1, 20);
    CheckEvent();
	}

	private void UpdateWeather ()
	{

		int randomValue = rand.Next (1, 100);

		switch (currentSeason) {
		case Season.Summer:
			if (0 <= randomValue && randomValue <= 30) {
				currentWeather = Weather.Clear;
			} else if (31 <= randomValue && randomValue <= 70) {
				currentWeather = Weather.Hot;
			} else if (71 <= randomValue && randomValue <= 75) {
				currentWeather = Weather.Cold;
			} else if (76 <= randomValue && randomValue <= 100) {
				currentWeather = Weather.Raining;
			}
			break;
		case Season.Fall:
			if (0 <= randomValue && randomValue <= 15) {
				currentWeather = Weather.Clear;
			} else if (16 <= randomValue && randomValue <= 40) {
				currentWeather = Weather.Hot;
			} else if (41 <= randomValue && randomValue <= 65) {
				currentWeather = Weather.Cold;
			} else if (66 <= randomValue && randomValue <= 90) {
				currentWeather = Weather.Raining;
			} else if (91 <= randomValue && randomValue <= 100) {
				currentWeather = Weather.Snow;
			}
			break;
		case Season.Spring:
			if (0 <= randomValue && randomValue <= 35) {
				currentWeather = Weather.Clear;
			} else if (36 <= randomValue && randomValue <= 45) {
				currentWeather = Weather.Hot;
			} else if (46 <= randomValue && randomValue <= 65) {
				currentWeather = Weather.Cold;
			} else if (66 <= randomValue && randomValue <= 80) {
				currentWeather = Weather.Raining;
			} else if (81 <= randomValue && randomValue <= 100) {
				currentWeather = Weather.Snow;
			}
			break;
		case Season.Winter:
			if (0 <= randomValue && randomValue <= 15) {
				currentWeather = Weather.Clear;
			} else if (41 <= randomValue && randomValue <= 75) {
				currentWeather = Weather.Cold;
			} else if (76 <= randomValue && randomValue <= 66) {
				currentWeather = Weather.Raining;
			} else if (66 <= randomValue && randomValue <= 100) {
				currentWeather = Weather.Snow;
			}
			break;
		}

	}

	private void UpdateSeason ()
	{
		if (day >= seasonLength) {
			int current = (int)currentSeason;
			current += 1;
			currentSeason = (Season)(current % 4);
			day = 0;
		}
	}

  private void CheckEvent() {
    int rnd = rand.Next(1, 100);
    Debug.Log(rnd);
    if (rnd <= eventProbability) {
      eventFlag = true;
      eventProbability = 5; //reset probability
    }
    else {
      eventFlag = false;
    }
  }

}



