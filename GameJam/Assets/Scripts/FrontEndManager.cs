using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FrontEndManager : MonoBehaviour {

    public GameObject canvas;
    public GameObject maincamera;
	// Use this for initialization
	void Start () {
        InvokeRepeating("AssetUpdate", 0, 3.0f); //todo remove this once this is tied into WorldController
    }
	
	// AssetUpdate is called once every 3 seconds
	void AssetUpdate () {
        //Debug.Log(maincamera.GetComponent<GameController>().world.currentWeather);
        if (maincamera.GetComponent<GameController>().world.currentWeather == Weather.Raining)
        {
            canvas.transform.Find("Ground").GetComponent<Image>().color = new Color32(50, 193, 195, 255); //blue
        }
        else if (maincamera.GetComponent<GameController>().world.currentWeather == Weather.Snow ||
            maincamera.GetComponent<GameController>().world.currentSeason == Season.Winter)
        {
            canvas.transform.Find("Ground").GetComponent<Image>().color = new Color32(188, 188, 188, 255); //grey
        }
        else if (maincamera.GetComponent<GameController>().world.currentSeason == Season.Spring)
        {
            canvas.transform.Find("Ground").GetComponent<Image>().color = new Color32(106, 137, 39, 255); //dark green
        }
        else if (maincamera.GetComponent<GameController>().world.currentSeason == Season.Fall)
        {
            canvas.transform.Find("Ground").GetComponent<Image>().color = new Color32(201, 143, 76, 255); //brown
        }
        else if (maincamera.GetComponent<GameController>().world.currentSeason == Season.Summer)
        {
            canvas.transform.Find("Ground").GetComponent<Image>().color = new Color32(22, 237, 117, 255); //light green
        }
        //if (PlayerController.getDistancedTravelled() > 50)
        //{
            //todo change background image
        //}
        {

        }
    }
}
