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

        // назначает характеристики врага
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
            // если враг упал с арены
            if (transform.localPosition.y < -0.5f)
            {
                DestroyObject(gameObject);
            }

            // если это синий
            if (blueEnemy)
            {
                // медленно преследует игрока
                transform.position = Vector3.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);

                // стреляет с интервалом 3
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

            // красный
            else if (redEnemy)
            {
                //  при появлении летит вверх на некоторое расстояние (выше остальных)
                if (transform.localPosition.y < 1 && freezed == false)
                {
                    transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, 1f, transform.position.z), speed * 0.1f * Time.deltaTime);
                }
                else if (transform.localPosition.y >= 1 && freezed == false && timer == 8)
                {
                    freezed = true;
                }

                // затем замирает на время
                if (freezed && timer > 0)
                {
                    timer -= Time.deltaTime;
                }
                else if (freezed)
                {
                    freezed = false;
                    timer = 0;
                }

                // после стремительно летит в игрока отнимая его здоровье (15 единиц) и умирая при попадании
                if (freezed == false && timer <= 0)
                {
                    transform.position = Vector3.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
                }
            }

        }
    }

    // запускается пуля синего в направлении игрока
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
        // если враг сталкивается с обычной пулей
        if (other.tag == "Bullet")
        {
            // если врага коснулась пуля, она его убивает, поэтому урон = максимальное здоровье врага
            thisHp -= 100;

            // вероятность рикошета коррелирует со здоровьем игрока
            rikoshet = playerScript.hp;

            // чем меньше здоровья, тем выше вероятность. -30, чтобы можно было достичь вероятности 100, пока игрок жив
            if (Random.Range(0, 100) > rikoshet - 30)
            {
                // рикошетит в ближайшего врага, если есть еще враги
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
                    // иначе просто летит дальше
                }
            }
            else // иначе пуля исчезает
            {
                Destroy(other.gameObject);
            }

            // За убийство врага игрок получает очки силы
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

        // если эта пуля уже попадала во врага
        else if (other.tag == "Bullet2")
        {
            thisHp -= 50; // меньший урон
            Destroy(other.gameObject);

            //За убийство вторичными рикошетным снарядом игрок пополняет себе немного сили или половину здоровья
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

    // находит ближайшего врага для рикошета
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
