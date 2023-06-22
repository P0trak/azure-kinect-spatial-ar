using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetInit : MonoBehaviour
{
    public GameObject target;
    public GameObject support;
    public float wallZPosition;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Awake()
    {
        Vector3 targetPos = target.transform.position;
        float distToBackWall = targetPos.z - wallZPosition;
        support.transform.position = new Vector3(support.transform.position.x, support.transform.position.y, -distToBackWall / 2);
        support.transform.localScale = new Vector3(support.transform.localScale.x, support.transform.localScale.y, distToBackWall / 2);
    }

}
