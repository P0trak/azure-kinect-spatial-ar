using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CubeTest : MonoBehaviour
{

    public float[] sizes;
    public float[] distances;
    public GameObject cube;
    public Vector3 startPosition;
    public GameObject userStartPosition;

    int[] values;
    int count = 0;
    bool testing = false;
    bool finished = false;
    bool showing = false;
    public float cubeTime = 5f;
    float cubeCountdown;

    // Start is called before the first frame update
    void Start()
    {
        cube.SetActive(true);
        values = new int[sizes.Length * distances.Length];
        for (int i = 0; i < values.Length; i++)
        {
            values[i] = i;
        }
        //permute values
        for (int i = 0; i < values.Length; i++)
        {
            int tmp = values[i];
            int r = Random.Range(i, values.Length);
            values[i] = values[r];
            values[r] = tmp;
        }

        cubeCountdown = cubeTime;

        if (userStartPosition != null)
        {
            startPosition = userStartPosition.transform.position;
        }
        cube.transform.position = startPosition;
    }

    void GenerateNewCube()
    {
        showing = true;
        cube.SetActive(false);
        int value = values[count];
        int sizeIndex = value % sizes.Length;
        float size = sizes[sizeIndex];
        int distIndex = (int)(value / distances.Length);
        float distance = distances[distIndex];
        cube.transform.localScale = new Vector3(size, size, size);
        cube.transform.position = startPosition + new Vector3(0, 0, distance);
        cube.SetActive(true);
        Debug.Log("Cube of size " + size + " at distance " + distance);
        count++;
        if (count == values.Length)
        {
            Debug.Log("all cubes generated!");
            count = 0;
            finished = true;
        }
    }

    void HideCube()
    {
        cube.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("GenerateCube"))
        {
            if (!showing)
            {
                GenerateNewCube();
            }
            testing = true;
        }

        if (showing)
        {
            cubeCountdown -= Time.deltaTime;
            if (cubeCountdown <= 0)
            {
                showing = false;
                cubeCountdown = cubeTime;
                HideCube();
            }
            if (finished)
            {
                testing = false;
            }
        }
    }
}
