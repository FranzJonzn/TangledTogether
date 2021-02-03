using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cammeraFocus : MonoBehaviour
{
    public Transform character_1;
    public Transform character_2;

    // Update is called once per frame
    void Update()
    {


        transform.position = (character_1.position + character_2.position) * 0.5f;

        
    }
}
