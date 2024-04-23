using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleBarRotater : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 130f;

    void Update()
    {
        transform.Rotate(-Vector3.forward, rotationSpeed * Time.deltaTime);
    }
}
