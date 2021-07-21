using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INoiseFilter
{

    float Evaluate(Vector3 point);
}