using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float FollowSpeed = 2f;
    public float yOffset = 1f;
    public Transform target;

    // Update is called once per frame
    Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        Vector3 targetPos = new Vector3(target.position.x, target.position.y + yOffset, -8f);
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, 0.15f);
    }
}