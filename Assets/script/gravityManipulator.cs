using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class gravityManipulator : MonoBehaviour
{
    // Start is called before the first frame update

    public float a, b, c = 9.81f;

    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.T))
        { 
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.AddForce(Vector3.up * 0.1f, ForceMode.Impulse);
            Physics.gravity = new Vector3(15,0,0) * Time.deltaTime * Time.deltaTime * 10;//not works

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(-90,180, -90), Time.deltaTime * 18);
            

        }
        else if (Input.GetKey(KeyCode.G))
        {

            Rigidbody rb = GetComponent<Rigidbody>();
            rb.AddForce(Vector3.up * 0.1f, ForceMode.Impulse);
      

            Physics.gravity = new Vector3(-9.81f, 0, 0) * Time.deltaTime * 10;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(-90, -180, 90), Time.deltaTime * 18);
            
            



        }
        else if (Input.GetKey(KeyCode.F))
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.AddForce(Vector3.up * 0.1f, ForceMode.Impulse);

            Physics.gravity = new Vector3(0, 0, 9.81f) * Time.deltaTime * 10;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(-90,360,0), Time.deltaTime * 18);
            
        }
        else if (Input.GetKey(KeyCode.H))
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            //Vector3 backwardDirection = Vector3.back; // <- X direction
            // rb.AddForce(backwardDirection * 3f, ForceMode.Impulse);
            rb.AddForce(Vector3.up * 0.1f, ForceMode.Impulse);
            Physics.gravity = new Vector3(0, 0,-9.81f) * Time.deltaTime * 10;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(-90, 180, 0), Time.deltaTime * 18);
        }
    }




}
