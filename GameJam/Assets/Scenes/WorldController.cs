using System;

enum Weather
{
	Snow,
	Cold,
	Clear,
	Raining,
	Hot
}

enum Season {
	Summer,
	Spring,
	Winter,
	Fall
}

enum Month{
	January,
	February,
	March,
	April,
	May,
	June,
	July,
	August,
	September,
	October,
	November,
	December
}

public class WorldController
{
	const double probabilityStep = 0.05;
	int totalDistance;
	double eventProbability;
	Weather currentWeather;
	Season currentSeason;
	int seasonLength;
	Random rand = new Random ();
	DateTime date;

	public WorldController (int totalDistance, double startingProbability, Weather startingWeather, Month month, int seasonLength)
	{
		eventProbability = startingProbability;
		currentWeather = startingWeather;
		totalDistance = totalDistance;
		date = month;
		seasonLength = seasonLength
	}


	public void Update ()
	{
		UpdateEventProbability ();
	}

	private void UpdateEventProbability ()
	{
		 eventProbability = eventProbability + probabilityStep;
	}

	private void UpdateWeather() {

		int randomValue = rand.Next (1, 100);

		switch (currentSeason) {
			case Season.Summer:
				if (0 <= randomValue && randomValue <= 30) {
					currentWeather = Weather.Clear;
				}
				else if (31 <= randomValue && randomValue <= 70) {
					currentWeather = Weather.Hot;
				}
				else if (71 <= randomValue && randomValue <= 75) {
					currentWeather = Weather.Cold
				}
				else if (76 <= randomValue && randomValue <= 100) {
					currentWeather = Weather.Raining
				}
				break;
			case Season.Fall:
				if (0 <= randomValue && randomValue <= 15) {
					currentWeather = Weather.Clear;
				}
				else if (16 <= randomValue && randomValue <= 40) {
					currentWeather = Weather.Hot;
				}
				else if (41 <= randomValue && randomValue <= 65) {
					currentWeather = Weather.Cold
				}
				else if (66 <= randomValue && randomValue <= 90) {
					currentWeather = Weather.Raining
				}
				else if (91 <= randomValue && randomValue <= 100) {
					currentWeather = Weather.Snow
				}
				break;
			case Season.Spring:
				if (0 <= randomValue && randomValue <= 35) {
					currentWeather = Weather.Clear;
				}
				else if (36 <= randomValue && randomValue <= 45) {
					currentWeather = Weather.Hot;
				}
				else if (46 <= randomValue && randomValue <= 65) {
					currentWeather = Weather.Cold
				}
				else if (66 <= randomValue && randomValue <= 80) {
					currentWeather = Weather.Raining
				}
				else if (81 <= randomValue && randomValue <= 100) {
					currentWeather = Weather.Snow
				}
				break;
			case Season.Winter:
				if (0 <= randomValue && randomValue <= 15) {
					currentWeather = Weather.Clear;
				}
				else if (41 <= randomValue && randomValue <= 75) {
					currentWeather = Weather.Cold
				}
				else if (76 <= randomValue && randomValue <= 66) {
					currentWeather = Weather.Raining
				}
				else if (66 <= randomValue && randomValue <= 100) {
					currentWeather = Weather.Snow
				}
				break;
		}

	}

	private void UpdateSeason() {
		if()
	}



}

