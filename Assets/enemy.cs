using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy : MonoBehaviour
{
    public GameObject player;
    public Player playerScript;

    public bool blueEnemy;
    public bool redEnemy;
    public GameObject closest;

    public float thisHp;
    public float rikoshet;
    public float speed;
    public float timer;

    public bool freezed;

    void Start()
    {
        player = GameObject.Find("Player");
        playerScript = player.GetComponent<Player>();

        // ��������� �������������� �����
        if (this.name == "Blue(Clone)")
        {
            blueEnemy = true;
            thisHp = 100;
            speed = 0.1f;
        }
        else
        {
            redEnemy = true;
            thisHp = 50;
            speed = 4;
            timer = 8;
        }
    }

    public void Update()
    {
        if (playerScript.pause == false)
        {
            // ���� ���� ���� � �����
            if (transform.localPosition.y < -0.5f)
            {
                DestroyObject(gameObject);
            }

            // ���� ��� �����
            if (blueEnemy)
            {
                // �������� ���������� ������
                transform.position = Vector3.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);

                // �������� � ���������� 3
                if (timer < 3)
                {
                    timer += Time.deltaTime;
                }
                else
                {
                    bulletBlue();
                    timer = 0;
                }

            }

            // �������
            else if (redEnemy)
            {
                //  ��� ��������� ����� ����� �� ��������� ���������� (���� ���������)
                if (transform.localPosition.y < 1 && freezed == false)
                {
                    transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, 1f, transform.position.z), speed * 0.1f * Time.deltaTime);
                }
                else if (transform.localPosition.y >= 1 && freezed == false && timer == 8)
                {
                    freezed = true;
                }

                // ����� �������� �� �����
                if (freezed && timer > 0)
                {
                    timer -= Time.deltaTime;
                }
                else if (freezed)
                {
                    freezed = false;
                    timer = 0;
                }

                // ����� ������������ ����� � ������ ������� ��� �������� (15 ������) � ������ ��� ���������
                if (freezed == false && timer <= 0)
                {
                    transform.position = Vector3.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
                }
            }

        }
    }

    // ����������� ���� ������ � ����������� ������
    public void bulletBlue()
    {
        if (playerScript.pause == false)
        {
            Rigidbody bullet = Instantiate(playerScript.projectileEnemy, transform.position, Quaternion.identity, playerScript.enemyBullets.transform) as Rigidbody;
            bullet.transform.GetComponent<bulletEnemy>().player = player;
            bullet.transform.GetComponent<bulletEnemy>().playerScript = playerScript;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        // ���� ���� ������������ � ������� �����
        if (other.tag == "Bullet")
        {
            // ���� ����� ��������� ����, ��� ��� �������, ������� ���� = ������������ �������� �����
            thisHp -= 100;

            // ����������� �������� ����������� �� ��������� ������
            rikoshet = playerScript.hp;

            // ��� ������ ��������, ��� ���� �����������. -30, ����� ����� ���� ������� ����������� 100, ���� ����� ���
            if (Random.Range(0, 100) > rikoshet - 30)
            {
                // ��������� � ���������� �����, ���� ���� ��� �����
                if (Random.Range(0, 10) > 3 && playerScript.enemyHolder.transform.childCount > 0)
                {
                    other.tag = "Bullet2";
                    FindClosestEnemy();
                    other.GetComponent<Rigidbody>().velocity = transform.forward * 0;
                    other.GetComponent<bullet>().target = closest.transform.position;
                    other.GetComponent<bullet>().move = true;
                }
                else
                {
                    // ����� ������ ����� ������
                }
            }
            else // ����� ���� ��������
            {
                Destroy(other.gameObject);
            }

            // �� �������� ����� ����� �������� ���� ����
            if (blueEnemy)
            {
                playerScript.power += 50;
            }
            else
            {
                playerScript.power += 15;
            }

            playerScript.killedTotal += 1;
            DestroyObject(gameObject);
        }

        // ���� ��� ���� ��� �������� �� �����
        else if (other.tag == "Bullet2")
        {
            thisHp -= 50; // ������� ����
            Destroy(other.gameObject);

            //�� �������� ���������� ���������� �������� ����� ��������� ���� ������� ���� ��� �������� ��������
            if (thisHp <= 0)
            {
                if (Random.Range(0, 10) > 5)
                {
                    playerScript.power += 10;

                }
                else
                {
                    playerScript.hp += 50;
                }

                playerScript.killedTotal += 1;
                DestroyObject(gameObject);
            }
        }
        if (redEnemy)
        {
            if (other.tag == "Player")
            {
                playerScript.hp -= 15;
                DestroyObject(gameObject);
            }

            if (other.tag == "Arena")
            {
                DestroyObject(gameObject);
            }
        }
    }

    // ������� ���������� ����� ��� ��������
    public void FindClosestEnemy()
    {
        playerScript.enemies = new List<GameObject>();
        foreach (Transform child in playerScript.enemyHolder.transform)
        {
            playerScript.enemies.Add(child.gameObject);
        }
        playerScript.enemies.Remove(this.gameObject);
        float distance = Mathf.Infinity;
        foreach (GameObject go in playerScript.enemies)
        {
            Vector3 diff = go.transform.position - transform.position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }
    }
}
