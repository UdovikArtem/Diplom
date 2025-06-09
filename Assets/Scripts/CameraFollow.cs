using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private float speed = 2f;
    [SerializeField]
    private float yOffset = 1f;
    [SerializeField]
    private float xMin = -1000000000;
    [SerializeField]
    private float xMax = 10000000000;
    [SerializeField]
    private Transform target;

    // Update is called once per frame
    void Update()
    {
        float xPos = target.position.x;
        if(xPos < xMin)
        {
            xPos = xMin;
        }
        if(xPos > xMax)
        {
            xPos = xMax;
        }
        Vector3 newPos = new Vector3(xPos, target.position.y + yOffset, -10f);
        transform.position = Vector3.Slerp(transform.position, newPos, speed * Time.deltaTime);
    }

    private void Start()
    {
        transform.position = new Vector3(target.position.x, target.position.y, -10);
    }
}
