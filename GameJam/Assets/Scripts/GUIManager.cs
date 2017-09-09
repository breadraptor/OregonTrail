using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour {

	public Font mainFont;
	private GameObject choicesMenuObject; 
	private GameObject defaultText;

	ArrayList testTexts;

	// Use this for initialization
	void Start () {
		testTexts = new ArrayList ();
	
		defaultText = GameObject.FindGameObjectWithTag ("UIDefaultText");

	

		choicesMenuObject = GameObject.FindGameObjectWithTag ("UIChoicesMenu");

		for (int i = 0; i < 1; ++i) {
			string text = "This is test text " + i;
			AddTextToGO (text, choicesMenuObject);
		}

//		GUIText testText = choicesMenuObject.AddComponent<GUIText> ();
//
//		testText.font = mainFont;
//		testText.text = "THIS IS SOME TEST TEXT";
//		testText.enabled = true;
//		testText.transform.parent = choicesMenuObject.transform;
	}

	void AddTextToGO(string text, GameObject parentCanvas) {

		GameObject textObj = Instantiate (defaultText, parentCanvas.transform);
		Text textCom = textObj.GetComponent<Text> ();
		textCom.text = text;

//		GUIText guiText = textObj.GetComponent<GUIText>();
//		guiText.text = text;
//		guiText.fontSize = 28;






//		GameObject textObj = new GameObject ("textObject", typeof(RectTransform));
//
//		CanvasRenderer renderer = textObj.AddComponent<CanvasRenderer> ();
//		renderer.SetMaterial (mainFont.material, mainFont.material.mainTexture);
//
//		Debug.Log ("Mat Count: " + renderer.materialCount);
//		Debug.Log ("Pop Count: " + renderer.popMaterialCount);
//		textObj.transform.SetParent (parentCanvas.transform);
//		GUIText guiText = textObj.AddComponent<GUIText> ();
//		guiText.font = mainFont;
//		guiText.fontSize = 22;
//		guiText.text = text;
//		guiText.enabled = true;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
