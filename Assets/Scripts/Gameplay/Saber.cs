using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Saber : MonoBehaviour
{
    public LayerMask scoreLayer;
    public LayerMask mistakeLayer;

    [Tooltip("Events to trigger when the saber scores")]
    public UnityEvent OnScore;

    public UnityEvent onScore => OnScore;

    [Tooltip("Events to trigger when the saber hits the wrong thing")]
    public UnityEvent OnMistake;

    public UnityEvent onMistake => OnMistake;
    private float scoreLayerNumber;
    private float mistakeLayerNumber;
    //private Vector3 previousPos;
    //public float angle;
    // Start is called before the first frame update
    void Start()
    {
        scoreLayerNumber = Mathf.Log(scoreLayer.value, 2);
        mistakeLayerNumber = Mathf.Log(mistakeLayer.value, 2);
    }

    // Update is called once per frame
    void Update()
    {
        /*RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 1, layer))
        {
            if (Vector3.Angle(transform.position - previousPos, hit.transform.parent.up) > angle)
            {
                Destroy(hit.transform.parent.gameObject);
            }
        }*/

        //previousPos = transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint contact = collision.GetContact(0);
        
        if (contact.otherCollider.gameObject.layer == scoreLayerNumber)
        {
            Destroy(contact.otherCollider.transform.parent.gameObject);
            OnScore.Invoke();
/*            //float cutAngle = Vector3.Angle(transform.position - previousPos, contact.otherCollider.transform.parent.up);
            float cutAngleTwo = Vector3.Angle(-contact.normal, contact.otherCollider.transform.parent.up);

            Debug.Log("Saber Collided with block, Angle : " + cutAngleTwo);

            if (cutAngleTwo >= angle)
            {
                
            }*/
        }
        if (contact.otherCollider.gameObject.layer == mistakeLayerNumber)
        {
            Destroy(contact.otherCollider.transform.parent.gameObject);
            OnMistake.Invoke();
        }
        /*



                if (collision.collider.gameObject.layer == layer)
                {
                    if (Vector3.Angle(transform.position - previousPos, collision.collider.transform.parent.up) > angle)
                    {
                        Destroy(collision.collider.transform.parent.gameObject);
                    }
                }*/
    }
}
