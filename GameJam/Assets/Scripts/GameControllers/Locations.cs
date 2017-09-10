using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Locations
{
	static private ArrayList locations;
	static private string unknownLocationId = "UNKNOWN";
	static private string startingLocationId = "old_festerland";
	static private string finalLocationId = "crows_rest";

	static private LocationFinder locFinder;

	static public Location getLocationWithId (string id)
	{
		for (int i = 0; i < locations.Count; ++i) {
			Location loc = (Location)locations [i];
			if (loc.id == id) {
				return loc;
			}
		}
		return (Location)locations [0];
	}

	static public Location getStartingLocation() {
		return getLocationWithId (startingLocationId);
	}

	static public bool locationIsFinal(string locationId) {
		return locationId == finalLocationId;
	}

	static public void init ()
	{
		locations = new ArrayList ();
		locFinder = new LocationFinder ();

		// NOTE: Always have 'UNKNOWN' location first in list
		Location unknownLocation = new Location ("UNKNOWN LOCATION", "YOU SHOULD NOT BE ABLE TO GET HERE", "UNKNOWN");
		unknownLocation.addDestination (new Destination ("UNKNOWN", 0));
		locations.Add (unknownLocation);

		string festerlandDesc = "This 'city' is built on the edge of an old, stinking swamp.";
		string shamblesDesc = "A relatively new settlement, it is built of scrap from the old world.";
		string flintDesc = "Built out of the ruins of an old mining town, though the mines have run dry.";
		string crossroadsDesc = "More of a giant market than town, many caravans pass through here";
		string lostPassDesc = "Along a dangerous route over the mountains. Many travelers don't make it this far.";
		string downstoneDesc = "The only remnant of what was here before, was a weathered sign bearing the name.";
		string hangedmanDesc = "Home to a militant conclave, several dead bodies hang from the gates wearing signs describing their crimes.";
		string lastBulletDesc = "Established by a hunter, at the site of his most harrowing adventure.";
		string theHangingFortDesc = "A bustling community of farmers, no one knows how it got its name.";
		string longRoadsRestDesc = "This waystop serves the few survivors of the Long Road.";
		string silverGullyDesc = "The surrounding hills practically glitter with the now useless old world wealth.";
		string lakeFishboneDesc = "A once bustling fishing town, now clinging to a lake too acidic for life.";
		string shatterPassDesc = "This settlment clings to life in a mountain range devestated by earthquakes.";
		string theDeepsDesc = "This place marks the entrance to a series of old mining caves.";
		string sunriseCityDesc = "A golden city on a hilltop, overrun with corruption and crime.";
		string crowsRestDesc = "A city that's gotten its legs under it. There is a functioning government and power grid.";

		Location festerland = new Location ("Old Festerland", festerlandDesc, "old_festerland");
		festerland.addDestination(new Destination("shambles", 350));
		festerland.addDestination(new Destination("the_hanging_fort", 500));
		Location shambles = new Location ("Shambles", shamblesDesc, "shambles");
		shambles.addDestination(new Destination("flint", 600));
		shambles.addDestination(new Destination("downstone", 840));
		Location flint = new Location ("Flint", flintDesc, "flint");
		flint.addDestination(new Destination("crossroads", 520));
		Location crossroads = new Location ("Crossroads", crossroadsDesc, "crossroads");
		crossroads.addDestination(new Destination("lost_pass", 830));
		Location lostPass = new Location ("Lost Pass", lostPassDesc, "lost_pass");
		lostPass.addDestination(new Destination("crows_rest", 615));
		Location downstone = new Location ("Downstone", downstoneDesc, "downstone");
		downstone.addDestination(new Destination("hangedman", 730));
		downstone.addDestination(new Destination("last_bullet", 900));
		Location hangedman = new Location ("Hangedman", hangedmanDesc, "hangedman");
		hangedman.addDestination(new Destination("crows_rest", 885));
		Location lastBullet = new Location ("Last Bullet", lastBulletDesc, "last_bullet");
		lastBullet.addDestination(new Destination("crows_rest", 820));
		Location theHangingFort = new Location ("The Hanging Fort", theHangingFortDesc, "the_hanging_fort");
		theHangingFort.addDestination(new Destination("long_roads_rest", 1865));
		theHangingFort.addDestination(new Destination("silver_gully", 855));
		theHangingFort.addDestination(new Destination("shatter_pass", 630));
		Location longRoadsRest = new Location ("Long Road's Rest", longRoadsRestDesc, "long_roads_rest");
		longRoadsRest.addDestination(new Destination("crows_rest", 430));
		Location silverGully = new Location ("Silver Gully", silverGullyDesc, "silver_gully");
		silverGully.addDestination(new Destination("lake_fishbone", 920));
		Location lakeFishbone = new Location ("Lake Fishbone", lakeFishboneDesc, "lake_fishbone");
		lakeFishbone.addDestination(new Destination("crows_rest", 845));
		Location shatterPass = new Location ("Shatter Pass", shatterPassDesc, "shatter_pass");
		shatterPass.addDestination(new Destination("the_deeps", 620));
		Location theDeeps = new Location ("The Deeps", theDeepsDesc, "the_deeps");
		theDeeps.addDestination(new Destination("lost_pass", 440));
		Location sunriseCity = new Location ("Sunrise City", sunriseCityDesc, "sunrise_city");
		sunriseCity.addDestination(new Destination("crows_rest", 540));
		Location crowsRest = new Location ("Crow's Rest", crowsRestDesc, "crows_rest");

		locations.Add (festerland);
		locations.Add (shambles);
		locations.Add (flint);
		locations.Add (crossroads);
		locations.Add (lostPass);
		locations.Add (downstone);
		locations.Add (hangedman);
		locations.Add (lastBullet);
		locations.Add (theHangingFort);
		locations.Add (longRoadsRest);
		locations.Add (silverGully);
		locations.Add (lakeFishbone);
		locations.Add (shatterPass);
		locations.Add (theDeeps);
		locations.Add (sunriseCity);
		locations.Add (crowsRest);

		// Old Festerlan ->
		//      Shambles -> 350
		//           Flint -> 600
		//                Crossroads -> 520
		//                     Lost Pass -> 830
		//                          Crows Rest 615
		//           Downstone -> 840
		//                Hangedman -> 730
		//                     Crows Rest 885
		//                Last Bullet -> 900
		//                     Crows Rest 820
		//      The Hanging Fort -> 500
		//           Long Road's Rest -> 1865
		//                Crows Rest 430
		//           Silver Gully -> 855
		//                Lake Fishbone -> 920
		//                     Crows Rest -> 845
		//           Shatter Pass -> 630
		//                The Deeps -> 620
		//                     Lost Pass -> 440
		//                          
   
	}

	public class LocationFinder : IComparer
	{
		int IComparer.Compare (object id, object loc)
		{
			return ((Location)loc).id.CompareTo ((string)id);
		}
	}

}

public class Location
{
	public string name;
	public string description;
	public string id;
	public ArrayList destinations = new ArrayList ();

	public Location (string locationName, string locationDescription, string locationId)
	{
		name = locationName;
		description = locationDescription;
		id = locationId;
	}

	public void addDestination (Destination d)
	{
		destinations.Add (d);
	}
}

public class Destination
{
	public string id;
	public int distance;

	public Destination (string destinationId, int destinationDistance)
	{
		id = destinationId;
		distance = destinationDistance;
	}
}