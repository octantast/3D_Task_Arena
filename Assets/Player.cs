using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public bool pause; // �� ����� �� ����

    // �����
    public int hp;
    public int power;
    public Image hpScale;
    public Image powerScale;
    public Text hpText;
    public Text powerText;

    // ����� ����
    public GameObject reloadScreen;
    public Text startagain;
    public Vector2 loadPosition;

    // ������� ��� ������
    public Rigidbody redEnemy;
    public Rigidbody blueEnemy;
    public Rigidbody thisBody;
    public Rigidbody projectile;
    public Rigidbody projectileEnemy;
    public FixedJoystick joystick;

    // ��� ������������� � ���������
    public Vector2 touchDeltaPosition;
    public float pointer_x;
    public float pointer_y;
    public float farX;
    public float farZ;
    public bool rotatingStarted;
    public float rotationX;
    public float rotationY;

    // ��������
    public float speedOfMovement;
    public float speedOFRotation;
    public float bulletSpeed;

    public GameObject ultaButton;

    // ����� ������
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

    // ��� ��������
    public List<GameObject> enemies = new List<GameObject>();

    void Start()
    {

        // �������� �����
        hp = 100;
        power = 50;
        hpText.text = "HEALTH: " + hp.ToString() + "/100";
        powerText.text = "POWER: " + power.ToString() + "/100";

        // ��������� ������ ��� ������
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
            // ������ ������� �����
            power = Mathf.Clamp(power, 0, 100);
            hp = Mathf.Clamp(hp, 0, 100);

            // ���������� ����������� ������
            hpText.text = "HP: " + hp.ToString() + "/100";
            powerText.text = "POWER: " + power.ToString() + "/100";
            hpScale.fillAmount = hp * 0.01f;
            powerScale.fillAmount = power * 0.01f;

            // ����������� �����
            if (power == 100)
            {
                power = 100;
                ultaButton.SetActive(true);
            }
            else
            {
                ultaButton.SetActive(false);
            }

            // ���� ����� ���� ����������� ����, � ������� ����� ������ ������� + ���������� ������ ������
            if (hp <= 0)
            {
                Paused();
                speedOFRotation = 0;
                speedOfMovement = 0;
                startagain.text = "ENEMIES KILLED: " + killedTotal.ToString();
            }

            // ����� ������ � ����������
            if (timer < interval)
            {
                timer += Time.deltaTime;
            }
            else
            {
                enemySpawned();
            }

            // ����������
            if (Input.touchCount > 0)
            {
                // ������� ������, ���� ����� ������� ��������
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
                // ������ ���o��� ������, ��� ��������
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

            // ������� ������������ ����� ���� �� ������ ���������
            transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0);

            // ����������� ������
            Vector3 movement = transform.forward * joystick.Vertical + transform.right * joystick.Horizontal;
            thisBody.velocity = movement * speedOfMovement * Time.deltaTime;

            // ������, �� ���������� �� ����� �� �����
            if (transform.localPosition.y < -0.2f)
            {
                teleport();
            }
        }
    }

    //������������� ����������� ��������� ���� �� ����� � ������ ������
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

    // �� ����� �� ������ ����� ����� ������� ������ � ������ ������
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

    // ����� � ������ ������ ����, �� ����� ������������ ����������� �������� �� ������ �Ulta�, ��� ������������ ������� ��� ����� �����
    public void Ulta()
    {
        if (pause == false)
        {
            // ��������� � �������
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

    // ����� ����� ��������� � ���� ��� ���������� � ��������� ����� �� ��������� �������� �� ���� ������
    public void teleport()
    {
        // ��� ����, ������� ������� �� �������, �������� ���, ��� ��� �����
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
            // ���� � ������� ��� ������, ���������� ������ ������
            if (enemiestoSpawn.Count <= 0)
            {
                // ����������� 1 ����� � 4 �������
                enemiestoSpawn.Add("blue");
                for (int i = 0; i < 5; i++)
                {
                    enemiestoSpawn.Add("red");
                }
            }

            // ��������� ���� ��������� �� ������
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

            // ������ 5 ��� ��������� ����� ��� ������������ ���������, � ������ ������� ����������� ����� ������ �� ������������ �������� - 2 ���
            if (interval > 2)
            {
                interval -= 0.1f;
            }
            // ����� ����� ����� ���������� ����������� ������
            else
            {
                numberOfType1 += 1; // ������� �������������� �����, ������� �� -4
                numberOfType2 += 1;// ������� �������������� �������, ������� �� 0
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




