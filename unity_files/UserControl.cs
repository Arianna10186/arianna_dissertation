using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserControl : MonoBehaviour
{
    private GameObject user;
    // Start is called before the first frame update
    void Start()
    {
        user = GameObject.Find("Player"); 
    }

    // Update is called once per frame
    void Update()
    {
        // control user with wasd keys
        if (Input.GetKey("w"))
        {
            user.transform.position += new Vector3(0, 0, 0.01f);
        }
        if (Input.GetKey("s"))
        {
            user.transform.position += new Vector3(0, 0, -0.01f);
        }
        if (Input.GetKey("d"))
        {
            user.transform.position += new Vector3(0.01f, 0, 0);
        }
        if (Input.GetKey("a"))
        {
            user.transform.position += new Vector3(-0.01f, 0, 0);
        }
    }
}
