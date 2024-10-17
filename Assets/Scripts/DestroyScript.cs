using UnityEngine;

public class DestroyScript : MonoBehaviour
{
    public float DestroyTime = 5f;
    // Update is called once per frame
    void Awake()
    {
        Destroy(this.gameObject, DestroyTime);
    }
}
