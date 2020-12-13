using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class AsteroidFieldGenerator : MonoBehaviour
{
    //public Transform asteroidPrefab;
    public List<Transform> asteroids;
    public int fieldRadius = 100;
    public int asteroidCount = 500;
    public float minSize = 1f;
    public float maxSize = 20f;

    public void GenerateAsteroidField()
    {
        //foreach (Transform child in transform)
        //{
        //    Destroy(child.gameObject);
        //}

        ClearChildren();

        for (int loop = 0; loop < asteroidCount; loop++)
        {
            Transform temp = Instantiate(asteroids[Random.Range(0, asteroids.Count)], Random.insideUnitSphere * fieldRadius, Random.rotation, this.transform);
            temp.localScale = temp.localScale * Random.Range(minSize, maxSize);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ClearChildren()
    {
        Debug.Log(transform.childCount);
        int i = 0;

        //Array to hold all child obj
        GameObject[] allChildren = new GameObject[transform.childCount];

        //Find all child obj and store to that array
        foreach (Transform child in transform)
        {
            allChildren[i] = child.gameObject;
            i += 1;
        }

        //Now destroy them
        foreach (GameObject child in allChildren)
        {
            DestroyImmediate(child.gameObject);
        }

        Debug.Log(transform.childCount);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, fieldRadius);
    }
}
