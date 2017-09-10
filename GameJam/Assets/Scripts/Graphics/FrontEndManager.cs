﻿using System.Collections;using System.Collections.Generic;using UnityEngine;using UnityEngine.UI;public class FrontEndManager : MonoBehaviour{	public GameObject canvas;	private GameController gameController;	// Use this for initialization	void Start ()	{		gameController = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameController> ();	}	// AssetUpdate is called once every 3 seconds	public void AssetUpdate ()	{		Season s = gameController.GetSeason ();		Weather w = gameController.GetWeather ();		if (w == Weather.Raining) {			canvas.transform.Find ("Ground").GetComponent<Image> ().color = new Color32 (50, 193, 195, 255); //blue		} else if (w == Weather.Snow || s == Season.Winter) {			canvas.transform.Find ("Ground").GetComponent<Image> ().color = new Color32 (188, 188, 188, 255); //grey		} else if (s == Season.Spring) {			canvas.transform.Find ("Ground").GetComponent<Image> ().color = new Color32 (106, 137, 39, 255); //dark green		} else if (s == Season.Fall) {			canvas.transform.Find ("Ground").GetComponent<Image> ().color = new Color32 (201, 143, 76, 255); //brown		} else if (s == Season.Summer) {			canvas.transform.Find ("Ground").GetComponent<Image> ().color = new Color32 (22, 237, 117, 255); //light green		}		//if (PlayerController.getDistancedTravelled() > 50)		//{		//todo change background image		//}		{		}	}}