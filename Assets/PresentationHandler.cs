using UnityEngine;
using System.Collections;
using SimpleJSON;
using Vuforia;
using System;

public class PresentationHandler : MonoBehaviour
{

    public GameObject[] imageSlides;
    public int lastTrackedSlideIndex;

    //The start and finish positions for the interpolation
    private Vector3 originalTextSize;
    private Vector3 shiftedTextSize;
    private Vector3 originalBackgroundSize;
    private Vector3 shiftedBackgroundSize;
    private Vector3 originalModelSize;
    private Vector3 shiftedModelSize;

    private Vector3 originalTextPos;
    private Vector3 shiftedTextPos;
    private Vector3 originalBackgroundPos;
    private Vector3 shiftedBackgroundPos;
    private Vector3 originalModelPos;
    private Vector3 shiftedModelPos;

    //Whether we are currently interpolating or not
    private bool isLerpingLeft;
    private bool isLerpingRight;

    //What position we are currently in
    private bool isLeftFocus;
    private bool isRightFocus;

    //The Time.time value when we started the interpolation
    private float timeStartedLerping;

    //The time we want to spend on lerping;
    private float timeToSpendLerping;


    public void Setup(string json) {

        var JS = JSON.Parse(json);

        Texture[] textures = Resources.LoadAll<Texture>("Textures");

        /*
        Debug.Log(JS["name"].Value); // Prints "Test lecture one"
        Debug.Log(JS["date"].Value); // Prints "2015-10-01 15:30"
        Debug.Log(JS["speakers"][0]["name"].Value); // Prints "Willy Wonka"
        Debug.Log(JS["speakers"][0]["id"].AsInt); // Prints 1
        Debug.Log(JS["slides"][0]["background"].Value); // Prints "#ff0000"
        */

        for (int i = 0; i < imageSlides.Length; i++) {

            imageSlides[i].FindComponentInChildWithTag<TextMesh>("SlideText").text = "";
            imageSlides[i].FindComponentInChildWithTag<Renderer>("SlideBackground").materials[0].SetTexture("_MainTex", textures[i]);
            /*
            imageSlides[i].FindComponentInChildWithTag<TextMesh>("SlideText").richText = true;
            imageSlides[i].FindComponentInChildWithTag<TextMesh>("SlideText").text = JS["slides"][i]["text"].Value;
            imageSlides[i].FindComponentInChildWithTag<Renderer>("SlideBackground").materials[0].SetTexture("_MainTex", textures[JS["slides"][i]["background"].AsInt]);
            */
        }

    }


    void Start() {

        Setup("{\"name\":\"Test lecture one\",\"date\":\"2015-10-01 15:30\",\"speakers\":[{\"id\":\"01\",\"name\":\"Willy Wonka\"},{\"id\":\"02\",\"name\":\"Evel Knievel\"}],\"slides\":[{\"text\":\"\",\"background\":\"0\",\"3dmodel\":null},{\"text\":\"\",\"background\":\"01\",\"3dmodel\":null},{\"text\":\"\",\"background\":\"02\",\"3dmodel\":null}]}");

        // The amount of seconds that the lerp-animation should last
        timeToSpendLerping = 3.0f;

        // Setting The Slide as default focus
        isLeftFocus = true;
        isRightFocus = false;

        // Storing the original Sizes and Positions of everything that is to be lerped
        originalTextSize = imageSlides[0].FindComponentInChildWithTag<Transform>("SlideText").localScale;
        shiftedTextSize = originalTextSize * 0.2f;

        originalBackgroundSize = imageSlides[0].FindComponentInChildWithTag<Transform>("SlideBackground").localScale;
        shiftedBackgroundSize = originalBackgroundSize * 0.2f;

        originalModelSize = imageSlides[0].FindComponentInChildWithTag<Transform>("SlideModel").localScale;
        shiftedModelSize = originalModelSize * 2.0f;

        originalTextPos = imageSlides[0].FindComponentInChildWithTag<Transform>("SlideText").localPosition;
        shiftedTextPos = new Vector3(originalTextPos.x * 0.2f - 2.0f, originalTextPos.y, originalTextPos.z * 0.2f);

        originalBackgroundPos = imageSlides[0].FindComponentInChildWithTag<Transform>("SlideBackground").localPosition;
        shiftedBackgroundPos = new Vector3(originalBackgroundPos.x - 2.0f, originalBackgroundPos.y, originalBackgroundPos.z);

        originalModelPos = imageSlides[0].FindComponentInChildWithTag<Transform>("SlideModel").localPosition;
        shiftedModelPos = new Vector3(originalModelPos.x * 0.2f, originalModelPos.y, originalModelPos.z);

        //Making sure the camera uses Auto-focus
        VuforiaBehaviour.Instance.RegisterVuforiaStartedCallback(OnVuforiaStarted);
        VuforiaBehaviour.Instance.RegisterOnPauseCallback(OnPaused);
    }

    private void OnVuforiaStarted()
    {
        CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);
    }

    private void OnPaused(bool paused)
    {
        if (!paused) // resumed
        {
            // Set autofocus mode again when app is resumed
            CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);
        }
    }



    public void SetLastTrackedIndex(string index)
    {
        lastTrackedSlideIndex = int.Parse(index);
		try{
	        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
	        AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
	        jo.Call("setLastSlideIndex", index);
		}catch(Exception ex){
			Debug.Log("===exception while setting last slide index in java==");
			//not present while debuging
		}
    }

    public void StartLerping(string direction)
    {
        if ((direction == "left") && (!isLerpingLeft && !isLerpingRight) && (!isLeftFocus))
        {
            isLerpingLeft = true;
            timeStartedLerping = Time.time;
        } else if ((direction == "right") && (!isLerpingLeft && !isLerpingRight) && (!isRightFocus))
        {
            isLerpingRight = true;
            timeStartedLerping = Time.time;
        }
    }

    void Update() {

        if(Input.GetKeyDown("a"))
        {
            StartLerping("left");
        }
        if (Input.GetKeyDown("s"))
        {
            StartLerping("right");
        }

    }

    void FixedUpdate()
    {
        if(isLerpingRight)
        {

            float timeSinceStarted = Time.time - timeStartedLerping;
            float percentageComplete = timeSinceStarted / timeToSpendLerping;

            // Change sizes
            imageSlides[lastTrackedSlideIndex].FindComponentInChildWithTag<Transform>("SlideText").localScale = Vector3.Lerp(originalTextSize, shiftedTextSize, percentageComplete);
            imageSlides[lastTrackedSlideIndex].FindComponentInChildWithTag<Transform>("SlideBackground").localScale = Vector3.Lerp(originalBackgroundSize, shiftedBackgroundSize, percentageComplete);
            imageSlides[lastTrackedSlideIndex].FindComponentInChildWithTag<Transform>("SlideModel").localScale = Vector3.Lerp(originalModelSize, shiftedModelSize, percentageComplete);

            // Change positions
            imageSlides[lastTrackedSlideIndex].FindComponentInChildWithTag<Transform>("SlideText").localPosition = Vector3.Lerp(originalTextPos, shiftedTextPos, percentageComplete);
            imageSlides[lastTrackedSlideIndex].FindComponentInChildWithTag<Transform>("SlideBackground").localPosition = Vector3.Lerp(originalBackgroundPos, shiftedBackgroundPos, percentageComplete);
            imageSlides[lastTrackedSlideIndex].FindComponentInChildWithTag<Transform>("SlideModel").localPosition = Vector3.Lerp(originalModelPos, shiftedModelPos, percentageComplete);

            if(percentageComplete >= 1.0f)
            {
                isLerpingRight = false;
                isRightFocus = true;
                isLeftFocus = false;
            }

        } else if(isLerpingLeft) {

            float timeSinceStarted = Time.time - timeStartedLerping;
            float percentageComplete = timeSinceStarted / timeToSpendLerping;

            // Change sizes
            imageSlides[lastTrackedSlideIndex].FindComponentInChildWithTag<Transform>("SlideText").localScale = Vector3.Lerp(shiftedTextSize, originalTextSize, percentageComplete);
            imageSlides[lastTrackedSlideIndex].FindComponentInChildWithTag<Transform>("SlideBackground").localScale = Vector3.Lerp(shiftedBackgroundSize, originalBackgroundSize, percentageComplete);
            imageSlides[lastTrackedSlideIndex].FindComponentInChildWithTag<Transform>("SlideModel").localScale = Vector3.Lerp(shiftedModelSize, originalModelSize, percentageComplete);

            // Change positions
            imageSlides[lastTrackedSlideIndex].FindComponentInChildWithTag<Transform>("SlideText").localPosition = Vector3.Lerp(shiftedTextPos, originalTextPos, percentageComplete);
            imageSlides[lastTrackedSlideIndex].FindComponentInChildWithTag<Transform>("SlideBackground").localPosition = Vector3.Lerp(shiftedBackgroundPos, originalBackgroundPos, percentageComplete);
            imageSlides[lastTrackedSlideIndex].FindComponentInChildWithTag<Transform>("SlideModel").localPosition = Vector3.Lerp(shiftedModelPos, originalModelPos, percentageComplete);

            if (percentageComplete >= 1.0f)
            {
                isLerpingLeft = false;
                isLeftFocus = true;
                isRightFocus = false;
            }

        }
    }
}
