using System.IO;
using UnityEngine;

public class KinectCameraController : MonoBehaviour
{
    public Vector3 startPosition;
    public Vector3 startAngles;
    public float scale;
    //public float timeCount = 0.0f;
    //Queue<Vector3> rotationQueue = new Queue<Vector3>();
    //public int maxQueueSize;
    public GameObject anchor;
    bool headFound = false;
    Vector3 initialZVector;

    public bool evaluate = true;
    bool evaluating = false;
    public float maxRecordTime = 5f;
    float countdown = 0f;
    string path = "Data/position_data.txt";
    StreamWriter writer;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = startPosition;
        transform.eulerAngles = startAngles;
        Debug.Log("evaluate mode: " + evaluate);
    }

    void Update()
    {
        if (evaluate)
        {
            if (Input.GetButtonDown("StartRecording") && countdown == 0f)
            {
                Debug.Log("starting recording...");
                countdown = maxRecordTime;
                writer = new StreamWriter(path);
                evaluating = true;
            }

            if (countdown > 0f)
            {
                countdown -= Time.deltaTime;
                if (countdown <= 0f)
                {
                    countdown = 0f;
                    evaluating = false;
                    Debug.Log("finished recording!");
                    writer.Flush();
                    writer.Close();
                }
            }
        }
    }

    public void UpdatePosition(Vector3 headPosition)
    {
        if (!headFound)
        {
            headFound = true;
            initialZVector = new Vector3(0, 0, 0);// headPosition.z);
        }
        Debug.Log("head at: " + headPosition);
        Vector3 newPosition = startPosition + scale * (headPosition-initialZVector);
        transform.position = newPosition;
        transform.LookAt(anchor.transform);
        if (evaluating)
        {
            writer.WriteLine(headPosition.ToString("F5"));
        }

        /*while (rotationQueue.Count >= maxQueueSize)
        {
            rotationQueue.Dequeue();
        }
        rotationQueue.Enqueue(headRotation.eulerAngles);

        Vector3[] rotations = rotationQueue.ToArray();

        Vector3 avg = new Vector3();

        foreach(Vector3 rot in rotations)
        {
            avg.x += rot.x;
            avg.y += rot.y;
        }

        avg = avg / rotations.Length;
        */

        /*
        Vector3 currAngles = transform.rotation.eulerAngles;
        Vector3 headAngles = headRotation.eulerAngles;

        Vector3 newAngles = currAngles;
        if (Mathf.Abs(currAngles.x - headAngles.x) < 10)
        {
            newAngles.x = headAngles.x;
        }
        if (Mathf.Abs(currAngles.y - headAngles.y) < 10)
        {
            newAngles.y = headAngles.y;
        }
        //Vector3 headAngles = headRotation.eulerAngles;
        //Vector3 newAngles = new Vector3(avg.x, avg.y, 0);
        transform.rotation = Quaternion.Euler(newAngles);// Quaternion.Slerp(transform.rotation, headRotation, timeCount);
        Debug.Log("current angles: " + newAngles);
        //timeCount += Time.deltaTime;
        */
        
    }

    private void OnApplicationQuit()
    {
        if (writer != null)
        {
            writer.Flush();
            writer.Close();
            writer.Dispose();
        }
    }
}
