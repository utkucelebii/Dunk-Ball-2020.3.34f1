using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    public bool isSwipe, isThrow;
    public Vector3 direction;
    public bool isItScore;

    private Vector3 startingPosition, endingPosition;
    private float startingTime, endingTime;
    private float timePass;

    private float maxTime = 0.2f;
    private float swipeResistanceY = 150f;

    private void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            startingTime = Time.time;
            startingPosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            timePass += Time.deltaTime;
            direction = -(startingPosition - Input.mousePosition).normalized;

            if (timePass > maxTime && !isSwipe && !isThrow)
            {
                isSwipe = true;
                isThrow = false;
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            timePass = 0;
            isSwipe = false;

            endingTime = Time.time;
            endingPosition = Input.mousePosition;

            float totalTime = endingTime - startingTime;
            Vector2 deltaSwipe = startingPosition - endingPosition;

            if(totalTime < maxTime && Mathf.Abs(deltaSwipe.y) > swipeResistanceY)
            {
                direction = endingPosition - startingPosition;
                isThrow = true;
            }


        }
    }

}
