using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public bool pause; // на паузе ли игра

    // статы
    public int hp;
    public int power;
    public Image hpScale;
    public Image powerScale;
    public Text hpText;
    public Text powerText;

    // экран меню
    public GameObject reloadScreen;
    public Text startagain;
    public Vector2 loadPosition;

    // префабы дл€ спавна
    public Rigidbody redEnemy;
    public Rigidbody blueEnemy;
    public Rigidbody thisBody;
    public Rigidbody projectile;
    public Rigidbody projectileEnemy;
    public FixedJoystick joystick;

    // дл€ переджвижени€ и поворотов
    public Vector2 touchDeltaPosition;
    public float pointer_x;
    public float pointer_y;
    public float farX;
    public float farZ;
    public bool rotatingStarted;
    public float rotationX;
    public float rotationY;

    // скорости
    public float speedOfMovement;
    public float speedOFRotation;
    public float bulletSpeed;

    public GameObject ultaButton;

    // спавн врагов
    public float interval;
    public float killedTotal;
    public int enemyFloat;
    public float timer;
    public GameObject enemyBullets;
    public GameObject thisBullets;
    public GameObject enemyHolder;
    public List<string> enemiestoSpawn = new List<string>();
    public float numberOfType1;
    public float numberOfType2;

    // дл€ рикошета
    public List<GameObject> enemies = new List<GameObject>();

    void Start()
    {

        // исходные статы
        hp = 100;
        power = 50;
        hpText.text = "HEALTH: " + hp.ToString() + "/100";
        powerText.text = "POWER: " + power.ToString() + "/100";

        // назанчает врагов дл€ спавна
        enemiestoSpawn.Add("blue");
        for (int i = 0; i < 4; i++)
        {
            enemiestoSpawn.Add("red");
        }
    }

    void Update()
    {


        if (pause == false)
        {
            // читает текущие статы
            power = Mathf.Clamp(power, 0, 100);
            hp = Mathf.Clamp(hp, 0, 100);

            // визуальное отображение статов
            hpText.text = "HP: " + hp.ToString() + "/100";
            powerText.text = "POWER: " + power.ToString() + "/100";
            hpScale.fillAmount = hp * 0.01f;
            powerScale.fillAmount = power * 0.01f;

            // доступность ульты
            if (power == 100)
            {
                power = 100;
                ultaButton.SetActive(true);
            }
            else
            {
                ultaButton.SetActive(false);
            }

            // ≈сли игрок убит открываетс€ меню, в котором будет кнопка У«ановоФ + количество убитых врагов
            if (hp <= 0)
            {
                Paused();
                speedOFRotation = 0;
                speedOfMovement = 0;
                startagain.text = "ENEMIES KILLED: " + killedTotal.ToString();
            }

            // спавн врагов с интервалом
            if (timer < interval)
            {
                timer += Time.deltaTime;
            }
            else
            {
                enemySpawned();
            }

            // управление
            if (Input.touchCount > 0)
            {
                // поворот камеры, если зажат контрол движени€
                if (Input.touchCount > 1)
                {
                    if (rotatingStarted)
                    {
                        Touch touch = Input.GetTouch(0);
                        touchDeltaPosition = touch.deltaPosition;
                    }
                    else
                    {
                        Touch touch = Input.GetTouch(1);
                        touchDeltaPosition = touch.deltaPosition;
                    }
                    pointer_x = touchDeltaPosition.x * Time.deltaTime;
                    pointer_y = touchDeltaPosition.y * Time.deltaTime;
                    rotationX += -pointer_y * speedOFRotation;
                    rotationX = Mathf.Clamp(rotationX, -30, 30);
                    rotationY += pointer_x * speedOFRotation;

                }
                // просто повoрот камеры, без движени€
                else if (joystick.Horizontal == 0 && joystick.Vertical == 0 && Input.touchCount <= 1)
                {
                    rotatingStarted = true;
                    Touch touch = Input.GetTouch(0);
                    switch (touch.phase)
                    {
                        case TouchPhase.Ended:
                            rotatingStarted = false;
                            break;
                    }
                    touchDeltaPosition = touch.deltaPosition;
                    pointer_x = touchDeltaPosition.x * Time.deltaTime;
                    pointer_y = touchDeltaPosition.y * Time.deltaTime;
                    rotationX += -pointer_y * speedOFRotation;
                    rotationX = Mathf.Clamp(rotationX, -30, 30);
                    rotationY += pointer_x * speedOFRotation;
                }
            }

            // поворот продолжаетс€ после тапа до нужных координат
            transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0);

            // перемещение игрока
            Vector3 movement = transform.forward * joystick.Vertical + transform.right * joystick.Horizontal;
            thisBody.velocity = movement * speedOfMovement * Time.deltaTime;

            // следит, не провалилс€ ли игрок за арену
            if (transform.localPosition.y < -0.2f)
            {
                teleport();
            }
        }
    }

    //ƒополнительно возможность поставить игру на паузу и начать заново
    public void Paused()
    {
        if (pause == false)
        {
            pause = true;
            reloadScreen.transform.localPosition = loadPosition;

        }
        else if (pause && hp > 0)
        {
            pause = false;
            reloadScreen.transform.localPosition = new Vector2(900, 0);
        }
    }

    // ѕо клику на кнопку атаки игрок пускает снар€д с центра экрана
    public void Shoot()
    {
        if (hp > 0 && pause == false)
        {
            transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0);
            Rigidbody bullet = Instantiate(projectile, this.transform) as Rigidbody;
            bullet.velocity = transform.forward * bulletSpeed;
            bullet.transform.SetParent(thisBullets.transform);
        }
    }

    //  огда у игрока полна€ сила, он может активировать способность нажатием на кнопку УUltaФ, при срабатывании которой все враги умрут
    public void Ulta()
    {
        if (pause == false)
        {
            // добавл€ет в счетчик
            enemies = new List<GameObject>();
            foreach (Transform child in enemyHolder.transform)
            {
                enemies.Add(child.gameObject);
            }
            killedTotal += enemies.Count;

            power = 0;
            foreach (Transform child in enemyHolder.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            foreach (Transform child in enemyBullets.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            foreach (Transform child in thisBullets.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            ultaButton.SetActive(false);
        }
    }

    //  огда игрок подходить к краю его перемещает в случайное место на платформе подальше от всех врагов
    public void teleport()
    {
        // все пули, которые следили за игроком, исчезнут там, где был игрок
        foreach (Transform child in enemyBullets.transform)
        {
            child.GetComponent<bulletEnemy>().destroying = true;
            Destroy(child.gameObject, 5);
        }
        farX = -transform.localPosition.x;
        farZ = -transform.localPosition.z;
        farX = Mathf.Clamp(farX, -3, 3);
        farZ = Mathf.Clamp(farZ, -3, 3);
        transform.localPosition = new Vector3(farX, 1f, farZ);
        transform.localRotation = Quaternion.Euler(0, 0, 0);

       // isKinematic
    }

    public void enemySpawned()
    {
        if (pause == false)
        {
            // если в очереди нет врагов, генеирурем список заново
            if (enemiestoSpawn.Count <= 0)
            {
                // —оотношение 1 синий к 4 красным
                enemiestoSpawn.Add("blue");
                for (int i = 0; i < 5; i++)
                {
                    enemiestoSpawn.Add("red");
                }
            }

            // спавнитс€ один рандомный из списка
            enemyFloat = Random.Range(0, enemiestoSpawn.Count);
            if (enemiestoSpawn[enemyFloat] == "red")
            {
                Rigidbody enemy = Instantiate(redEnemy, new Vector3(Random.Range(-4f, 4f), 0.3f, Random.Range(-4f, 4f)), Quaternion.identity, enemyHolder.transform) as Rigidbody;
            }
            else
            {
                Rigidbody enemy = Instantiate(blueEnemy, new Vector3(Random.Range(-4f, 4f), 0.3f, Random.Range(-4f, 4f)), Quaternion.identity, enemyHolder.transform) as Rigidbody;
            }
            enemiestoSpawn.Remove(enemiestoSpawn[enemyFloat]);

            //  аждые 5 сек спаун€тс€ враги над поверхностью платформы, с каждым спауном сокращаетс€ врем€ спауна до минимального значени€ - 2 сек
            if (interval > 2)
            {
                interval -= 0.1f;
            }
            // после будет расти количество заспауненых врагов
            else
            {
                numberOfType1 += 1; // сколько дополнительных синих, начина€ от -4
                numberOfType2 += 1;// сколько дополнительных красных, начина€ от 0
                numberOfType1 = Mathf.Clamp(numberOfType1, -4, 4);
                numberOfType2 = Mathf.Clamp(numberOfType2, 0, 8);

                for (int i = 0; i < numberOfType1; i++)
                {
                    Rigidbody enemy = Instantiate(blueEnemy, new Vector3(Random.Range(-4f, 4f), 0.3f, Random.Range(-4f, 4f)), Quaternion.identity, enemyHolder.transform) as Rigidbody;
                }
                for (int i = 0; i < numberOfType2; i++)
                {
                    Rigidbody enemy = Instantiate(redEnemy, new Vector3(Random.Range(-4f, 4f), 0.3f, Random.Range(-4f, 4f)), Quaternion.identity, enemyHolder.transform) as Rigidbody;
                }
            }

            timer = 0;
        }
    }

    public void Reload()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}




