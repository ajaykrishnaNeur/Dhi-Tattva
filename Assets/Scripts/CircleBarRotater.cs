using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleBarRotater : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private float rotationSpeed = 130f; // Adjust this to control the rotation speed

    // Update is called once per frame
    void Update()
    {
        // Rotate the sprite around the Z-axis
        transform.Rotate(-Vector3.forward, rotationSpeed * Time.deltaTime);
    }
}
