using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    private Vector3 offset = new Vector3(0f, 0f, -10f);
    [Range(.25f, .99f)]
    [SerializeField] float smoothTime;
    private Vector3 velocity = Vector3.zero;

    [SerializeField] private Transform player;


    private void FixedUpdate()
    {
        Vector3 playerPosition = player.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, playerPosition, ref velocity, smoothTime);
    }
}
