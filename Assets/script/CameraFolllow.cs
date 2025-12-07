using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFolllow : MonoBehaviour
{

    //[SerializeField] private GameObject camera;
    //[SerializeField] private GameObject PlayerControl;

    //public void Start()
    //{
    //    camera = GameObject.Find("Main Camera");

    //}
    //public void LateUpdate()
    //{
    //    camera.transform.position = new Vector3(camera.transform.position.x, camera.transform.position.y, camera.transform.position.z);
    //}
    public float rotationSpeed;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        transform.Rotate(Vector3.up, horizontalInput * rotationSpeed * Time.deltaTime);
    }

}
