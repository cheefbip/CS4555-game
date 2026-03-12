using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float FollowSpeed = 2f;
    public float yOffset = 1f;
    public Transform target;
    public Material waterMaterial;
    public Transform floor;

    // Update is called once per frame
    Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        Vector3 targetPos = new Vector3(target.position.x, target.position.y + yOffset, -8f);
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, 0.15f);

        Vector2 offset = new Vector2(transform.position.x, transform.position.z) / -2560f;

        waterMaterial.SetVector("_Offset", offset);

        floor.localPosition = new Vector3(transform.position.x, floor.localPosition.y, transform.position.z);

        
        
    }
}