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
                // ������ ����� �� ������� �� ��� �� ���� �� ������ � ����
                transform.position = Vector3.MoveTowards(transform.position, player.transform.position, 0.3f * Time.deltaTime);
            }
            else
            {
                // ���� ����� ���������������� �� ����, �� ������� ���������� ������ � ����� �� ����������� ������, ����� �������� �� ������ �����
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x + 1, transform.position.y, transform.position.z + 1), 0.3f * Time.deltaTime);
            }
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        // ��� ��������� �������� ���� ������ (25 ������)
        if (other.tag == "Player")
        {
            player.GetComponent<Player>().hp -= 25;
            DestroyObject(gameObject);
        }

        // ������� ���������, ��������
        else if (other.tag == "Arena")
        {
            DestroyObject(gameObject);
        }
    }
}
