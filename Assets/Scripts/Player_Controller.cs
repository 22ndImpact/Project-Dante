using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    public List<Player_Character> PlayerCharacters;
    public GameObject LeaderIndicator;
    public int CurrentCharacterIndex;
    public float MoveTime;

	// Use this for initialization
	void Start ()
    {
        //Sets the indicator to the leader
        LeaderIndicator.transform.parent = PlayerCharacters[0].transform;
        LeaderIndicator.transform.localPosition = Vector3.zero;
    }
	
	// Update is called once per frame
	void Update ()
    {
        UpdateInput();
        UpdateDebugVisuals();
        //Sets the indicator to the leader TODO needs a better system
        
        //LeaderIndicator.transform.position = PlayerCharacters[0].transform.position;
    }
    
    void UpdateInput()
    {
        if (Input.GetKey(KeyCode.W))
        {
            TryMove(new Vector3(-1, 0, 0), MoveTime);
        }

        if (Input.GetKey(KeyCode.S))
        {
            TryMove(new Vector3(1, 0, 0), MoveTime);
        }

        if (Input.GetKey(KeyCode.D))
        {
            TryMove(new Vector3(0, 0, 1), MoveTime);
        }

        if (Input.GetKey(KeyCode.A))
        {
            TryMove(new Vector3(0, 0, -1), MoveTime);
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            CycleCharacters();
        }
    }

    void TryMove(Vector3 _direction, float _duration)
    {
        if(CanMove(PlayerCharacters[0], _direction))
        {
            #region Try follow logic
            //Determine who is the closest to the leader, and make them follow the leader
            float Distance1 = (PlayerCharacters[0].transform.position - PlayerCharacters[1].transform.position).sqrMagnitude;
            float Distance2 = (PlayerCharacters[0].transform.position - PlayerCharacters[2].transform.position).sqrMagnitude;

            //If character 1 is closer, make him follow first
            if (Distance1 < Distance2)
            {
                //Make character 1 attempt follow character 0
                TryFollow(PlayerCharacters[0], PlayerCharacters[1]);
                //Then make character 2 attempt to follow character 1
                TryFollow(PlayerCharacters[1], PlayerCharacters[2]);
            }
            else
            {
                //Make character 2 attempt to follow character 0
                TryFollow(PlayerCharacters[0], PlayerCharacters[2]);
                //Then make character 1 attempt to follow character 2
                TryFollow(PlayerCharacters[2], PlayerCharacters[1]);
            }
            #endregion

            #region Try moving the leader
            //Move the lead character
            PlayerCharacters[0].TryMove(_direction, _duration);
            #endregion
        }
    }

    void CycleCharacters()
    {
        //Create the new list
        List<Player_Character> newCharacterList = new List<Player_Character>();
        
        //Populate it in the right order
        newCharacterList.Add(PlayerCharacters[1]);
        newCharacterList.Add(PlayerCharacters[2]);
        newCharacterList.Add(PlayerCharacters[0]);

        //Apply the modified list back to the original list
        PlayerCharacters.Clear();
        PlayerCharacters.Add(newCharacterList[0]);
        PlayerCharacters.Add(newCharacterList[1]);
        PlayerCharacters.Add(newCharacterList[2]);

        //Refresh new Leader Icon Parent
        LeaderIndicator.transform.parent = PlayerCharacters[0].transform;
        LeaderIndicator.transform.localPosition = Vector3.zero;
    }

    void TryFollow(Player_Character _leader, Player_Character _follower)
    {
        Vector3 distance = _leader.transform.position - _follower.transform.position;
        distance.y = 0;

        //If the distance is just 1 the follow, this will make characters not move too far
        if(distance.sqrMagnitude == 1)
        {
            _follower.TryMove(distance, MoveTime);
        }
        
    }

    bool CanMove(Player_Character _character, Vector3 _direction)
    {
        return true;

        #region Raycast Method
        //Set up first ray
        Vector3 Ray1Origin= PlayerCharacters[0].transform.position;
        //Increase Z and X by 0.5 to recenter origin
        Ray1Origin.z += 0.5f;
        Ray1Origin.x += 0.5f;

        #region Change Height
        //Round up the height if on stairs aka not at a factor of 0.5f
        if (Ray1Origin.y % 0.5f > 0.1f)
        {
            Ray1Origin.y += 0.25f;
        }

        //Up the start point to be 0.125 or 25% of height
        Ray1Origin.y += 0.125f;
        #endregion

        Ray Ray1 = new Ray(Ray1Origin, _direction);
        RaycastHit hit1Results = new RaycastHit();

        //If the forward ray hits
        if (Physics.Raycast(Ray1, out hit1Results, 0.75f + 0.01f))
        {
            //Debug.Log(hitResults.distance);
            Debug.Log("Origin Point: " + Ray1.origin);
            Debug.Log("Contact Point: " + hit1Results.distance);

            //If you hit after the 0.5 point (flat wall) then you hit a ramp
            if(hit1Results.distance > 0.6f)
            {
                //If it hits but its a ramp
                return true;
            }
            else
            {
                //If it hits but its a wall
                return false;
            }
        }
        //If it doesnt hit
        else
        {
            #region Downwards ray
            //Set up second ray
            Vector3 Ray2Origin = Ray1Origin + (_direction * 0.75f);
            //Create the 2nd ray to shoot downwards to see if you hit flat ground or a ramp
            Ray Ray2 = new Ray(Ray2Origin, new Vector3(0, -1, 0));
            RaycastHit hit2Results = new RaycastHit();

            //Shoot the ray downward
            if (Physics.Raycast(Ray2, out hit2Results, 0.25f + 0.01f))
            {
                //If it hit, return true
                return true;
            }
            else
            {
                //If it doesnt hit return false
                return false;
            }
            #endregion
        }
        #endregion

        #region Height Difference Method



        #endregion
    }

    void UpdateDebugVisuals()
    {
        #region Debug Line 1
        Vector3 Line1Start = PlayerCharacters[0].transform.position;
        //Increase Z and X by 0.5 to recenter origin
        Line1Start.z += 0.5f;
        Line1Start.x += 0.5f;

        //Round up the height if on stairs aka not at a factor of 0.5f
        if (Line1Start.y % 0.5f > 0.1f)
        {
            Line1Start.y += 0.25f;
        }

        //Up the start point to be 0.125 or 25% of height
        Line1Start.y += 0.125f;
        Vector3 Line1End = Line1Start + new Vector3(-0.75f, 0, 0);
        #endregion

        if (CanMove(PlayerCharacters[0], new Vector3(-1, 0, 0)))
        {
            Debug.DrawLine(Line1Start, Line1End, Color.green);
        }
        else
        {
            Debug.DrawLine(Line1Start, Line1End, Color.red);
        }
        
        
    }
}
