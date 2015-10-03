using UnityEngine;
using System.Collections;
using SimpleJSON;

public class PresentationHandler : MonoBehaviour {

    public GameObject[] imageSlides;

	public void Setup(string json){

		Debug.Log (json);

        var JS = JSON.Parse(json);

        /*
        Debug.Log(JS["name"].Value); // Prints "Test lecture one"
        Debug.Log(JS["date"].Value); // Prints "2015-10-01 15:30"
        Debug.Log(JS["speakers"][0]["name"].Value); // Prints "Willy Wonka"
        Debug.Log(JS["speakers"][0]["id"].AsInt); // Prints 1
        Debug.Log(JS["slides"][0]["background"].Value); // Prints "#ff0000"
        */

        imageSlides[0].FindComponentInChildWithTag<TextMesh>("SlideText").text = JS["slides"][0]["text"].Value;
        imageSlides[0].FindComponentInChildWithTag<Renderer>("SlideBackground").material.color = Color.red;


    }


    // Use this for initialization
    void Start () {


		Setup ("{\"name\":\"Test lecture one\",\"date\":\"2015-10-01 15:30\",\"speakers\":[{\"id\":\"01\",\"name\":\"Willy Wonka\"},{\"id\":\"02\",\"name\":\"Evel Knievel\"}],\"slides\":[{\"text\":\"Test slide 1\",\"background\":\"#ff0000\",\"3dmodel\":null},{\"text\":\"Test slide 2\",\"background\":\"#ff0000\",\"3dmodel\":null},{\"text\":\"Test slide 3\",\"background\":\"#ff0000\",\"3dmodel\":null}]}");

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
