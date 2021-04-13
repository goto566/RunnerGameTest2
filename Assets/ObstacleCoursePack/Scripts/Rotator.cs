using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float speed = 3f;

    public float force = 0;
    public Vector3 hitDir;

    public bool
        rotateX = false,
        rotateY = false,
        rotateZ = false;



    // Update is called once per frame
    void Update()
    {
        float rX = 0;
        float rY = 0;
        float rZ = 0;
        if (rotateX) rX = 1;
        if (rotateY) rY = 1;
        if (rotateZ) rZ = 1;

        transform.Rotate(
            rX * speed * Time.deltaTime / 0.01f,
            rY * speed * Time.deltaTime / 0.01f,
            rZ * speed * Time.deltaTime / 0.01f,
            Space.Self);
    }





}
