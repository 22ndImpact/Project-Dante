using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Character : MonoBehaviour
{

    bool moving = false;
    //Store Target Position
    public Vector3 targetPosition; 

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void TryMove(Vector3 _direction, float _duration)
    {
        if (!moving)
        {
            StartCoroutine(Move(_direction, _duration));
        }
    }

    IEnumerator Move(Vector3 _direction, float _duration)
    {
        //Set moving flag to true
        moving = true;

        //Store Starting Position
        Vector3 startingPosition = transform.position;
        //Store Target Position
        targetPosition = transform.position + _direction;

        //Loop through delaying by frame time. Lerping between positions;
        for (float i = 0; i <= _duration; i += Time.deltaTime)
        {
            //Adjust the Y position based on terrain
            //Raycast downwards from middle of character, get hit point and adjust position to that
            Vector3 rayOrigin = transform.position;
            //Shifts the ray origin to the middle of the character tile
            rayOrigin.x += 0.5f;
            rayOrigin.z += 0.5f;
            rayOrigin.y += 1.1f;

            Ray ray = new Ray(rayOrigin, new Vector3(0, -1, 0));
            RaycastHit hit = new RaycastHit();
            if(Physics.Raycast(ray, out hit))
            {
                //Debug.Log(hit.transform.name);
                targetPosition.y = hit.point.y;
            }
            

            //Create the current position
            Vector3 currentPosition = Vector3.LerpUnclamped(startingPosition, targetPosition, i / _duration);

            //Apply the current position
            transform.position = currentPosition;
            //Yield
            yield return null;
        }

        //Final movement to clamp right on the next cell
        transform.position = targetPosition;

        //Set moving flag to false
        moving = false;

        
    }
}
