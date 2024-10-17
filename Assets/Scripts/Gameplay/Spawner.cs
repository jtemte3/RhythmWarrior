using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Level Setup")]
    public GameObject[] cubes;
    public Transform[] points;

    public bool isSongActive;
    public float beat;
    public float timer;
    public float endTime;

    // Update is called once per frame
    void Update()
    {
        if (isSongActive)
        {
            if (Time.time < endTime)
            {
                if (timer > beat)
                {
                    GameObject cube = Instantiate(cubes[Random.Range(0, cubes.Length)], points[Random.Range(0, points.Length)]);
                    cube.transform.parent = null;
                    timer -= beat;
                }

                timer += Time.deltaTime;
            }
        }
    }
}
