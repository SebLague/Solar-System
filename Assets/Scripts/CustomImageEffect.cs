using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomImageEffect : MonoBehaviour {
	
	public bool active;
    public Shader shader;
    protected Material material;

    public virtual Material GetMaterial () {
        if (material == null || material.shader != shader) {
            material = new Material (shader);
        }

        return material;
    }

    public virtual void Release () {

    }
}