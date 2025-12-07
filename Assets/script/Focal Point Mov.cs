using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocalPointMov : MonoBehaviour
{

    public float speed = 5.0f;
    private Rigidbody playerRb;
    private GameObject focalPoint;

    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        focalPoint = GameObject.Find("Focal point");

    }
    void Update()
    {
        float forwardInput = Input.GetAxis("Vertical");
        playerRb.AddForce(focalPoint.transform.forward * forwardInput * speed);
       // powerupIndicator.transform.position = transform.position + new Vector3(0, -0.5f, 0);
    }
}
