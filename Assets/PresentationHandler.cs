using UnityEngine;
using System.Collections;
using SimpleJSON;

public class PresentationHandler : MonoBehaviour
{

    public GameObject[] imageSlides;
    public GameObject lastTrackedSlide;

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

        timeToSpendLerping = 3.0f;

        isLeftFocus = true;
        isRightFocus = false;

        originalTextSize = new Vector3(0.4f, 0.4f, 0.04f);
        shiftedTextSize = originalTextSize * 0.2f;

        originalBackgroundSize = new Vector3(1.0f, 1.0f, 0.66f);
        shiftedBackgroundSize = originalBackgroundSize * 0.2f;

        originalModelSize = new Vector3(1f, 1f, 1f);
        shiftedModelSize = originalModelSize * 2.0f;

        originalTextPos = new Vector3(-4.9f, 0.02f, 3.21f);
        shiftedTextPos = new Vector3(originalTextPos.x * 0.2f - 2.0f, originalTextPos.y, originalTextPos.z * 0.2f);

        originalBackgroundPos = new Vector3(0f, 0f, 0f);
        shiftedBackgroundPos = new Vector3(originalBackgroundPos.x - 2.0f, originalBackgroundPos.y, originalBackgroundPos.z);

        originalModelPos = new Vector3(6.0f, 1.0f, 0.0f);
        shiftedModelPos = new Vector3(originalModelPos.x * 0.2f, originalModelPos.y, originalModelPos.z);

        Debug.Log(json);

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
            imageSlides[i].FindComponentInChildWithTag<TextMesh>("SlideText").richText = true;
            imageSlides[i].FindComponentInChildWithTag<TextMesh>("SlideText").text = JS["slides"][i]["text"].Value;
            imageSlides[i].FindComponentInChildWithTag<Renderer>("SlideBackground").materials[0].SetTexture("_MainTex", textures[JS["slides"][i]["background"].AsInt]);
        }

    }


    // Use this for initialization
    void Start() {

        Setup("{\"name\":\"Test lecture one\",\"date\":\"2015-10-01 15:30\",\"speakers\":[{\"id\":\"01\",\"name\":\"Willy Wonka\"},{\"id\":\"02\",\"name\":\"Evel Knievel\"}],\"slides\":[{\"text\":\"Test <color=green>slide</color> 1\",\"background\":\"01\",\"3dmodel\":null},{\"text\":\"Test <color=red>slide</color> 2\",\"background\":\"02\",\"3dmodel\":null},{\"text\":\"Test <color=blue>slide</color> 3\",\"background\":\"03\",\"3dmodel\":null}]}");

    }

    public void StartLerping(string direction)
    {
        if ((direction == "left") && (!isLerpingLeft && !isLerpingRight) && (!isLeftFocus))
        {
            isLerpingLeft = true;
            timeStartedLerping = Time.time;
            Debug.Log("Started lerping left");
        } else if ((direction == "right") && (!isLerpingLeft && !isLerpingRight) && (!isRightFocus))
        {
            isLerpingRight = true;
            timeStartedLerping = Time.time;
            Debug.Log("Started lerping right");
        }
    }

    // Update is called once per frame
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
            lastTrackedSlide.FindComponentInChildWithTag<Transform>("SlideText").localScale = Vector3.Lerp(originalTextSize, shiftedTextSize, percentageComplete);
            lastTrackedSlide.FindComponentInChildWithTag<Transform>("SlideBackground").localScale = Vector3.Lerp(originalBackgroundSize, shiftedBackgroundSize, percentageComplete);
            lastTrackedSlide.FindComponentInChildWithTag<Transform>("SlideModel").localScale = Vector3.Lerp(originalModelSize, shiftedModelSize, percentageComplete);

            // Change positions
            lastTrackedSlide.FindComponentInChildWithTag<Transform>("SlideText").localPosition = Vector3.Lerp(originalTextPos, shiftedTextPos, percentageComplete);
            lastTrackedSlide.FindComponentInChildWithTag<Transform>("SlideBackground").localPosition = Vector3.Lerp(originalBackgroundPos, shiftedBackgroundPos, percentageComplete);
            lastTrackedSlide.FindComponentInChildWithTag<Transform>("SlideModel").localPosition = Vector3.Lerp(originalModelPos, shiftedModelPos, percentageComplete);

            if(percentageComplete >= 1.0f)
            {
                isLerpingRight = false;
                isRightFocus = true;
                isLeftFocus = false;
                Debug.Log("Lerping right complete, isRightFocus = " + isRightFocus);
            }

        } else if(isLerpingLeft) {

            float timeSinceStarted = Time.time - timeStartedLerping;
            float percentageComplete = timeSinceStarted / timeToSpendLerping;

            // Change sizes
            lastTrackedSlide.FindComponentInChildWithTag<Transform>("SlideText").localScale = Vector3.Lerp(shiftedTextSize, originalTextSize, percentageComplete);
            lastTrackedSlide.FindComponentInChildWithTag<Transform>("SlideBackground").localScale = Vector3.Lerp(shiftedBackgroundSize, originalBackgroundSize, percentageComplete);
            lastTrackedSlide.FindComponentInChildWithTag<Transform>("SlideModel").localScale = Vector3.Lerp(shiftedModelSize, originalModelSize, percentageComplete);

            // Change positions
            lastTrackedSlide.FindComponentInChildWithTag<Transform>("SlideText").localPosition = Vector3.Lerp(shiftedTextPos, originalTextPos, percentageComplete);
            lastTrackedSlide.FindComponentInChildWithTag<Transform>("SlideBackground").localPosition = Vector3.Lerp(shiftedBackgroundPos, originalBackgroundPos, percentageComplete);
            lastTrackedSlide.FindComponentInChildWithTag<Transform>("SlideModel").localPosition = Vector3.Lerp(shiftedModelPos, originalModelPos, percentageComplete);

            if (percentageComplete >= 1.0f)
            {
                isLerpingLeft = false;
                isLeftFocus = true;
                isRightFocus = false;
                Debug.Log("Lerping left complete");
            }

        }
    }
}
