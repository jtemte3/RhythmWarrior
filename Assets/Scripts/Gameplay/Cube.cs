using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Cube : MonoBehaviour
{

    public float speed;
    public float despawnTime;
    private float destroyTime;
    /*[Tooltip("Events to trigger when the saber hits the wrong thing")]
    public UnityEvent OnMiss;

    public UnityEvent onMiss => OnMiss;*/


    private void Start()
    {
        destroyTime = Time.time + despawnTime;
    }
    // Update is called once per frame
    void Update()
    {
        if (Time.time < destroyTime)
        {
            transform.position += Time.deltaTime * transform.forward * speed;
        }
        else
        {
            //OnMiss.Invoke();
            Destroy(gameObject);
        }
        
    }
}
