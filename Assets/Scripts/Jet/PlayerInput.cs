using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerInput : MonoBehaviour {



    public GameObject debug;

    public RectTransform leftStickPanel;
    public Image leftStickImage;
    public Vector3 leftStickImageOrigin;

    public RectTransform rightStickPanel;
    public Image rightStickImage;
    public Vector3 rightStickImageOrigin;

    public Image leftStickButtonImage;
    public Color leftStickButtonPressedColor;
    public Color leftStickButtonNormalColor;

    public Image rightStickButtonImage;
    public Color rightStickButtonPressedColor;
    public Color rightStickButtonNormalColor;

    public Image rightBumperButtonImage;
    public Color rightBumperButtonPressedColor;
    public Color rightBumperButtonNormalColor;

    public Image leftBumperButtonImage;
    public Color leftBumperButtonPressedColor;
    public Color leftBumperButtonNormalColor;

    public Image aButtonImage;
    public Color aButtonPressedColor;
    public Color aButtonNormalColor;

    public Image bButtonImage;
    public Color bButtonPressedColor;
    public Color bButtonNormalColor;

    public Image xButtonImage;
    public Color xButtonPressedColor;
    public Color xButtonNormalColor;

    public Image yButtonImage;
    public Color yButtonPressedColor;
    public Color yButtonNormalColor;



     /*public Image startButtonImage;
     public Color startButtonPressedColor;
     public Color startButtonNormalColor;

     public Image selectButtonImage;
     public Color selectButtonPressedColor;
     public Color selectButtonNormalColor;
     */

    public Slider leftTrigggerSlider;
    public Slider rightTriggerSlider;

    public Vector3 lstick, rstick;
    public bool lStickButton, rStickButton;
    public bool aButton, bButton, xButton, yButton;
    public bool up, down, left, right;
    public bool select, start;
    public bool lBumper, rBumper;
    public float lTrigger, rTrigger;


    public void Start()
    {
        leftStickImageOrigin = leftStickImage.rectTransform.anchoredPosition;
        rightStickImageOrigin = rightStickImage.rectTransform.anchoredPosition;
        leftStickButtonNormalColor = leftStickButtonImage.color;
        rightStickButtonNormalColor = rightStickButtonImage.color;
        aButtonNormalColor = aButtonImage.color;
        bButtonNormalColor = bButtonImage.color;
        xButtonNormalColor = xButtonImage.color;
        yButtonNormalColor = yButtonImage.color;
        leftBumperButtonNormalColor = leftBumperButtonImage.color;
        rightBumperButtonNormalColor = rightBumperButtonImage.color;
        /*startButtonNormalColor = startButtonImage.color;
        selectButtonNormalColor = selectButtonImage.color;
        */
    }
	
	// Update is called once per frame
	void Update () {
        lstick = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        rstick = new Vector2(Input.GetAxis("Horizontal2"), Input.GetAxis("Vertical2"));
        aButton = Input.GetButton("A");
        bButton = Input.GetButton("B");
        xButton = Input.GetButton("X");
        yButton = Input.GetButton("Y");
        start = Input.GetButton("Submit");
        /*select = Input.GetButton("Select");
        up = Input.GetButton("Up");
        down = Input.GetButton("Down");
        left = Input.GetButton("Left");
        right = Input.GetButton("Right");
        */
        lBumper = Input.GetButton("Left Bumper");
        rBumper = Input.GetButton("Right Bumper");

        lTrigger = Input.GetAxis("Left Trigger");
        rTrigger = Input.GetAxis("Right Trigger");

        leftStickImage.rectTransform.anchoredPosition = new Vector3((leftStickPanel.rect.width/2) * lstick.x, (leftStickPanel.rect.height/2) * lstick.y,0) +  leftStickImageOrigin;
        rightStickImage.rectTransform.anchoredPosition = new Vector3((rightStickPanel.rect.width / 2) * rstick.x, (rightStickPanel.rect.height / 2) * rstick.y, 0) + rightStickImageOrigin;

        //A
        if (aButton)
            aButtonImage.color = aButtonPressedColor;
        else
            aButtonImage.color = aButtonNormalColor;
        //B
        if (bButton)
            bButtonImage.color = bButtonPressedColor;
        else
            bButtonImage.color = bButtonNormalColor;
        //X
        if (xButton)
            xButtonImage.color = xButtonPressedColor;
        else
            xButtonImage.color = xButtonNormalColor;
        //Y
        if (yButton)
            yButtonImage.color = yButtonPressedColor;
        else
            yButtonImage.color = yButtonNormalColor;
        //L Stick
        if (lStickButton)
            leftStickButtonImage.color = leftStickButtonPressedColor;
        else
            leftStickButtonImage.color = leftStickButtonNormalColor;
        //R Stick
        if (rStickButton)
            rightStickButtonImage.color = rightStickButtonPressedColor;
        else
            rightStickButtonImage.color = rightStickButtonNormalColor;

        //L Bumper
        if (lBumper)
            leftBumperButtonImage.color = leftBumperButtonPressedColor;
        else
            leftBumperButtonImage.color = leftBumperButtonNormalColor;


        //R Bumper
        if (rBumper)
            rightBumperButtonImage.color = rightBumperButtonPressedColor;
        else
            rightBumperButtonImage.color = rightBumperButtonNormalColor;

        leftTrigggerSlider.value = lTrigger;
        rightTriggerSlider.value = rTrigger;
    }
}
