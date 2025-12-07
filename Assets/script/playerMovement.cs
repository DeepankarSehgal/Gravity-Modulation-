using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class playerMovement : MonoBehaviour
{
    // Start is called before the first frame update
    private float PlayerSpeed = 1.5f;
    private float RotationSpeed = 5f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if(Input.GetKey(KeyCode.W)){
        //     transform.Translate(Vector3.forward * PlayerSpeed * Time.deltaTime, Space.Self); 

        //     transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, 180f, 0f), Time.deltaTime * RotationSpeed);

        // }
        //else if (Input.GetKey(KeyCode.S))
        // {
        //     transform.Translate(Vector3.forward * PlayerSpeed * Time.deltaTime, Space.Self); 

        //     transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, 0f, 0f), Time.deltaTime * RotationSpeed);
        // }
        // else if (Input.GetKey(KeyCode.A))
        // {
        //     transform.Translate(Vector3.forward * PlayerSpeed * Time.deltaTime, Space.Self);
        //     transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, 90f, 0f), Time.deltaTime * RotationSpeed);
        // }
        // else if (Input.GetKey(KeyCode.D))
        // {
        //     transform.Translate(Vector3.forward * PlayerSpeed * Time.deltaTime, Space.Self);

        //     transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, 270f, 0f), Time.deltaTime * RotationSpeed);
        // }
        
            if (Input.GetKey(KeyCode.S))
            {
                transform.Translate(Vector3.forward * PlayerSpeed * Time.deltaTime, Space.World);
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0,0f, 0f), Time.deltaTime * RotationSpeed);
            }
            else if (Input.GetKey(KeyCode.W))
            {
                transform.Translate(Vector3.back * PlayerSpeed * Time.deltaTime, Space.World);
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, 180f, 0f), Time.deltaTime * RotationSpeed);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                transform.Translate(Vector3.left * PlayerSpeed * Time.deltaTime, Space.World);
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, 270f, 0f), Time.deltaTime * RotationSpeed);
            }
            else if (Input.GetKey(KeyCode.A))
            {
                transform.Translate(Vector3.right * PlayerSpeed * Time.deltaTime, Space.World);
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, 90f, 0f), Time.deltaTime * RotationSpeed);
            }
        



    }










































}
