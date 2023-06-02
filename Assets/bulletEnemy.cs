using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletEnemy : MonoBehaviour
{
    public GameObject player;
    public Player playerScript;

    public bool destroying;
    void Update()
    {
        if (playerScript.pause == false)
        {
            if (destroying == false)
            {
                // Снаряд летит за игроком до тех по пока не попадёт в него
                transform.position = Vector3.MoveTowards(transform.position, player.transform.position, 0.3f * Time.deltaTime);
            }
            else
            {
                // если игрок телепортировался от края, то снаряды продолжают лететь в точку до перемещения игрока, затем исчезнет не нанеся вреда
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x + 1, transform.position.y, transform.position.z + 1), 0.3f * Time.deltaTime);
            }
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        // при попадании отнимает силу игрока (25 единиц)
        if (other.tag == "Player")
        {
            player.GetComponent<Player>().hp -= 25;
            DestroyObject(gameObject);
        }

        // касаясь цилиндров, исчезает
        else if (other.tag == "Arena")
        {
            DestroyObject(gameObject);
        }
    }
}
