using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderDamp : MonoBehaviour
{
    public Material camMat;
    void Start()
    {
        camMat.SetFloat("_Lerp", 0);
    }
}
