using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ShaderMatrix : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<MeshRenderer>().sharedMaterial.SetVector("ihat", transform.right);
        GetComponent<MeshRenderer>().sharedMaterial.SetVector("jhat", transform.up);
        GetComponent<MeshRenderer>().sharedMaterial.SetVector("khat", transform.forward);
        Debug.Log(transform.right);
    }
}
