using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Activator : MonoBehaviour
{
    public bool activate = false;

    private void Update()
    {
        if (activate)
            activate = false;
    }
}
