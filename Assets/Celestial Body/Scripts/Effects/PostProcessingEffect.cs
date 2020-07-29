using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PostProcessingEffect {

    protected Material material;

    public abstract Material GetMaterial ();

    public virtual void ReleaseBuffers () {

    }
}