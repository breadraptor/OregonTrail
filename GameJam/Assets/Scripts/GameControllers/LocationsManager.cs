using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LocationsManager
{
	private static World world;

	static public Location getLocationWithId (string id)
	{
		if (world == null) {
			Debug.Log ("World is null");
		} else if (world.locations == null) {
			Debug.Log (world.finalLocationId);
			Debug.Log ("World Locations is null");
		}
		for (int i = 0; i < world.locations.Count; ++i) {
			Location loc = (Location)world.locations [i];
			if (loc.id == id) {
				return loc;
			}
		}
		return (Location)world.locations [0];
	}

	static public Location getStartingLocation() {
		return getLocationWithId (world.startingLocationId);
	}

	static public bool locationIsFinal(string locationId) {
		return locationId == world.finalLocationId;
	}

	static LocationsManager() {
		TextAsset locationConfig = (TextAsset)Resources.Load ("Configs/Locations");
		world = JsonUtility.FromJson<World> (locationConfig.text);
	}

	public class LocationFinder : IComparer
	{
		int IComparer.Compare (object id, object loc)
		{
			return ((Location)loc).id.CompareTo ((string)id);
		}
	}

}

[System.Serializable]
public class World
{
	public string unknownLocationId;
	public string startingLocationId;
	public string finalLocationId;
	public List<Location> locations;
}

[System.Serializable]
public class Location
{
	public string name;
	public string description;
	public string id;
	public int tradeValue;
	public List<Destination> destinations;
	public List<string> textBlurbs;
}

[System.Serializable]
public class Destination
{
	public string id;
	public int distance;
}