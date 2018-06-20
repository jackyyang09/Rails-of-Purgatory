using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailLogic : MonoBehaviour {

    LineRenderer railReference;
    //PlayerBehaviour player;
    Player player;
    [SerializeField]
    float length;

    bool playerOnRail;
    
    List<SpeedControlTableRow> speedTable = new List<SpeedControlTableRow>();

    public int samples;
    public int numEntries;

    public float localTimeElapsed;
    [SerializeField]
    float railTime; //Multiplier affects time it takes to progress through rail

    [SerializeField]
    float playerSpeed;

    public float speedDecay; //Player will lose this much speed per second when on rail

    int referenceRailIndex;

    public bool reversed;

	// Use this for initialization
	void Start () {
        railReference = GetComponent<LineRenderer>();
        //player = GameObject.Find("Player").GetComponent<PlayerBehaviour>();
        player = GameObject.Find("Player").GetComponent<Player>();

	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            player.SetRailStatus(true);
        }
    }

    // Update is called once per frame
    void Update () {
        if (playerOnRail && playerSpeed != 0)
        {
            //Recalculate time it takes to travel to the end of the curve
            railTime = length / playerSpeed; //Distance = rate * time

            localTimeElapsed += (Time.deltaTime / railTime); // control playback speed

            playerSpeed -= speedDecay * Time.deltaTime;
            if (playerSpeed < -1)
            {
                playerSpeed = 0;
                return;
            }

            //Manage Keyframe behaviour
            //UpdateSegmentIndices();

            // look up values from table
            // Loop through each row in the table
            for (int i = 1; i < speedTable.Count; i++)
            {
                ReorientPlayer(i);
                // Find the first sample who's distance is >= m_pKeyLocalTime
                if (speedTable[i].arcLengthNormalized >= localTimeElapsed)
                {
                    // calculate t value
                    float arc0 = speedTable[i - 1].arcLengthNormalized; // previous sample's normalized distance
                    float arc1 = speedTable[i].arcLengthNormalized; // current sample's normalized distance
                    float tVal = Mathf.InverseLerp(arc0, arc1, localTimeElapsed); // "inverse lerp" i.e. given 3 points, solve the tValue

                    // calculate intermediate table
                    Vector3 sample0 = speedTable[i - 1].sampleValue; //previous sample value
                    Vector3 sample1 = speedTable[i].sampleValue; //current sample value

                    Vector3 newPos = Vector3.Lerp(sample0, sample1, tVal);
                    player.transform.position = newPos + player.GetHeightOffset();
                    return;
                }
            }
            //We only reach here once we've gone through the whole rail
            LeaveRail();
        }
    }
    
    void CreateLookupTable()
    {
        speedTable.Clear();

        float timeStep = 1.0f / samples;

        // Create table and compute segment, t value and sample columns of table

        if (!reversed) //Player facing correct orientation relative to the rail's predefined path
        {
            for (int i = referenceRailIndex; i < railReference.positionCount - 1; i++) // loop through each segment
            {
                for (float j = 0.0f; j <= 1.0f; j += timeStep) // iterate through each sample on the current segment
                {
                    // Todo:
                    // Create a new SpeedControlTableRow and fill it in with the appropriate data
                    SpeedControlTableRow row = new SpeedControlTableRow();
                    row.segment = i; //row.segment = //...
                    row.tValue = j; //row.tValue = // ...
                    if (i == referenceRailIndex)
                        row.sampleValue = Vector3.Lerp(player.transform.position, railReference.GetPosition(i + 1), j);
                    else
                        row.sampleValue = Vector3.Lerp(railReference.GetPosition(i), railReference.GetPosition(i + 1), j);
                    speedTable.Add(row);
                }
            }
        }
        else //Player is facing opposite the rail's orientation
        {
            for (int i = referenceRailIndex; i > -1; i--) // loop through each segment
            {
                for (float j = 0.0f; j <= 1.0f; j += timeStep) // iterate through each sample on the current segment
                {
                    // Todo:
                    // Create a new SpeedControlTableRow and fill it in with the appropriate data
                    SpeedControlTableRow row = new SpeedControlTableRow();
                    row.segment = i;
                    row.tValue = j; //row.tValue = // ...
                    if (i == referenceRailIndex)
                        row.sampleValue = Vector3.Lerp(player.transform.position, railReference.GetPosition(i), j);
                    else
                        row.sampleValue = Vector3.Lerp(railReference.GetPosition(i + 1), railReference.GetPosition(i), j);
                    speedTable.Add(row);
                }
            }
        }

        // Calculate arc length column of table
        numEntries = speedTable.Count;

        if (numEntries == 0) // Shouldn't happen if above loop completed successfully   
        {
            print("Failed to create look up table.");
            return;
        }

        // Initialize first row of table
        // Remember the struct has no actor so we need to make sure to set everything manually
        // Note: the slides refer "arcLength" as "distance on curve"
        speedTable[0].arcLength = 0.0f;
        speedTable[0].arcLengthNormalized = 0.0f;

        // Loop through each point in the table and calculate the distance from the beginning of the path
        for (int i = 1; i < numEntries; i++)
        {
            // distance = length(current sample value - previous sample value)
            float distance = Vector3.Distance(speedTable[i].sampleValue, speedTable[i - 1].sampleValue);

            // m_pSpeedControlLookUpTable[i].arcLength = distance + previous sample's distance on curve
            speedTable[i].arcLength = distance + speedTable[i - 1].arcLength;
        }

        // Normalize the curve
        // This means 0 will be at the start of the path, and 1 will be at the end of the entire path
        float totalCurveLength = speedTable[numEntries - 1].arcLength; // length of the path = distance the last sample is from the beginning

        // Normalize each sample
        // Loop through each entry in the table
        // Set "ArcLengthNormalized" to sample's distance on curve divided by total length of curve
        for (int i = 1; i < numEntries; i++)
        {
            speedTable[i].arcLengthNormalized = speedTable[i].arcLength / totalCurveLength;
        }
    }

    /*
     * Sets the direction of the player forward relative to the direction of the rail
     * */
    void ReorientPlayer(int i)
    {
        Vector3 direction = speedTable[i].sampleValue - speedTable[i - 1].sampleValue;
        if (direction != Vector3.zero)
        {
            Quaternion rotation = Quaternion.LookRotation(direction);
            player.transform.eulerAngles = new Vector3(0, rotation.eulerAngles.y, 0);
        }
    }

    /*
     * Checks the direction of the rail to the orientation of the player
     * */
    void CheckOrientation()
    {
        Vector3 direction = railReference.GetPosition(referenceRailIndex + 1) - railReference.GetPosition(referenceRailIndex).normalized;

        if (Vector3.Dot(direction, player.transform.forward) < 0)
            reversed = true;
        else
            reversed = false;
    }

    public void SetReferenceRail(Transform other)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i) == other)
            {
                referenceRailIndex = i;
                CheckOrientation();
                return;
            }
        }
    }

    public void LeaveRail()
    {
        player.SetRailStatus(false);
        Vector3 velocity = player.transform.forward * playerSpeed;
        player.SetVelocity(velocity);
        playerOnRail = false;
        localTimeElapsed = 0;
    }

    void CalculateRailLength()
    {
        length = 0;
        if (!reversed)
        {
            length += Vector3.Distance(
                railReference.GetPosition(1),
                player.transform.position
                );

            for (int i = 2; i < railReference.positionCount; i++)
            {
                length += Vector3.Distance(
                    railReference.GetPosition(i),
                    railReference.GetPosition(i - 1)
                    );
            }
        }
        else
        {
            length += Vector3.Distance(
                railReference.GetPosition(railReference.positionCount - 2),
                player.transform.position
                );

            for (int i = railReference.positionCount - 3; i > -1; i--)
            {
                length += Vector3.Distance(
                    railReference.GetPosition(i),
                    railReference.GetPosition(i + 1)
                    );
            }
        }
    }

    public void SetOnRail(bool _bool)
    {
        playerOnRail = _bool;
        if (playerOnRail)
        {
            CreateLookupTable();
            CalculateRailLength();
        }
    }

    public void SetPlayerRailSpeed(float _speed)
    {
        playerSpeed = _speed;
    }
}
