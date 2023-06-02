using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    public Vector3 target;
    public bool move;
    public Rigidbody m_Rigidbody;
    private void Start()
    {
        // самоуничтожается по времени, если ни с чем не пересечется
        Destroy(gameObject, 10);
    }

    public void FixedUpdate()
    {
        if (move)
        { // преследует ближайшую цель, если это рикошет
           transform.position = Vector3.MoveTowards(transform.position, target, 2*Time.deltaTime);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        // касаясь цилиндров, исчезает
        if (other.tag == "Arena")
        {
            Destroy(this);
        }
    }

}
