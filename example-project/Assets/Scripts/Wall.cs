using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{

    public GameObject projector;
    public float distance;
    Queue<float> positions;
    public int bufferSize;
    public float threshold;
    bool calibrating { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        transform.position = projector.transform.position + new Vector3(0, 0, distance);
        transform.rotation = projector.transform.rotation;
        positions = new Queue<float>();
    }

    // the plan:
    /// <summary>
    /// when we enter calibration mode, don't do any projection mapping
    /// instead, keep track of the user's head position (specifically z-position)
    /// if the range of z positions is constant enough (check previous data), use the average as the distance
    /// don't forget to add the length of the average person's head
    /// at the very least log the distance
    /// </summary>
    /// 

    public void UpdateCalibration(Vector3 headPos)
    {
        if (!calibrating) return;

        if (positions.Count >= bufferSize)
        {
            while (positions.Count >= bufferSize)
            {
                positions.Dequeue();
            }
        }

        positions.Enqueue(headPos.z);
        if (positions.Count < bufferSize) return;
        
        float max = -Mathf.Infinity;
        float min = Mathf.Infinity;
        float[] posArray = positions.ToArray();
        for (int i = 0; i < posArray.Length; i++)
        {
            if (posArray[i] > max)
            {
                max = posArray[i];
            } if (posArray[i] < min)
            {
                min = posArray[i];
            }
        }
        float range = max - min;
        if (range < threshold)
        {
            float average = 0;
            foreach(float position in posArray)
            {
                average += position;
            }
            average = average / posArray.Length;
            Debug.Log("Calibration complete! Head at distance of " + average + "m");
            calibrating = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("StartCalibration"))
        {
            if (!calibrating)
            {
                Debug.Log("starting calibration...");
                calibrating = true;
            }
        }
    }
}
