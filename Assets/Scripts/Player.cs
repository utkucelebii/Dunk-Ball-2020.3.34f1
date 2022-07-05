using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    private InputManager inputManager;
    private Rigidbody rb;

    private float loseBounceness;
    private float loseBouncenessDecrease;
    private float turnSmoothVelocity;
    private float T = 0;
    private Vector3 shootPos;
    private Vector3 shootTarget;
    private bool ballOnAir;
    private bool isGrounded;

    [Header("Hoop")]
    [SerializeField] private Transform hoop;
    [SerializeField] private GameObject[] confetties;

    [Space(10)]

    [Header("Ball")]
    [SerializeField] private Vector3 startingPos;
    [SerializeField] private float bounceForce;
    [SerializeField] private float speed;
    [SerializeField] private float distance;
    private ParticleSystem smokeFX;

    [Space(10)]

    [Header("Canvas")]
    [SerializeField] private Text distanceBetweenHoop;
    [SerializeField] private GameObject InGamePanel;
    [SerializeField] private GameObject VictoryPanel;
    [SerializeField] private GameObject DeathPanel;

    private void Awake()
    {
        inputManager = InputManager.Instance;
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        if (LevelManager.Instance.currentLevel == null)
            LevelManager.Instance.currentLevel = 1;

        transform.position = startingPos;
        smokeFX = transform.GetChild(0).GetComponent<ParticleSystem>();
        loseBounceness = bounceForce;
        loseBouncenessDecrease = bounceForce / 4;

        foreach (var c in confetties)
            c.SetActive(false);
    }

    private void FixedUpdate()
    {
        float dist = Vector3.Distance(transform.position, hoop.position);
        distanceBetweenHoop.color = (dist > distance)? Color.red: Color.green;
        distanceBetweenHoop.text = "DISTANCE: " + ((int)dist).ToString();
    }

    private void Update()
    {
        transform.LookAt(hoop);

        if(inputManager.isSwipe && !ballOnAir && !inputManager.isThrow)
        {
            Vector3 direction = new Vector3(inputManager.direction.x, 0, inputManager.direction.y);

            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + transform.eulerAngles.y;

            Vector3 moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;

            transform.position += moveDir.normalized * speed * Time.deltaTime;
            //rb.rotation = Quaternion.Euler(rb.rotation.eulerAngles + moveDir.normalized * speed * Time.deltaTime);
        }

        if (inputManager.isThrow && !ballOnAir && isGrounded)
        {
            ballOnAir = true;
            isGrounded = false;
            shootPos = transform.position;

            Vector3 between = (hoop.position - shootPos).normalized;

            if (Vector3.Distance(shootPos, hoop.position) > distance)
            {
                Vector3 sT = shootPos + between * 10;
                sT.y = hoop.position.y;
                shootTarget = sT;
            }
            else
            {
                shootTarget = hoop.position;
                inputManager.isItScore = true;
            }
        }

        if (ballOnAir)
        {
            T += Time.deltaTime;
            float duration = 1.25f;

            float t01 = T / duration;

            Vector3 A = shootPos;
            Vector3 B = shootTarget;

            Vector3 arc = Vector3.up * 5 * Mathf.Sin(t01 * 3.14f);

            Vector3 pos = Vector3.Lerp(A, B, t01);

            transform.position = pos + arc;

            if (t01 >= 1)
            {
                ballOnAir = false;
                T = 0;
                shootPos = Vector3.zero;
            }
        }
    }


    private void OnCollisionEnter(Collision other)
    {
        if(other.transform.tag == "ground" && !inputManager.isItScore)
        {
            Vector3 velocity = rb.velocity;
            velocity.y = bounceForce;
            rb.velocity = velocity;

            smokeFX.Play();

            inputManager.isThrow = false;
            isGrounded = true;
        }

        if (other.transform.tag == "ground" && inputManager.isItScore && loseBounceness >= 0)
        {
            Vector3 velocity = rb.velocity;
            loseBounceness -= loseBouncenessDecrease;
            velocity.y = loseBounceness;
            rb.velocity = velocity;
        }

        if(other.transform.tag == "enemy" && !inputManager.isItScore)
        {
            InGamePanel.SetActive(false);
            DeathPanel.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "death" && !inputManager.isItScore)
        {
            InGamePanel.SetActive(false);
            DeathPanel.SetActive(true);
        }

        if(other.transform.tag == "score")
        {
            foreach (var c in confetties)
                c.SetActive(true);

            StartCoroutine(OpenVictoryPanel());
        }
    }

    IEnumerator OpenVictoryPanel()
    {
        yield return new WaitForSeconds(4.25f);
        InGamePanel.SetActive(false);
        VictoryPanel.SetActive(true);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        inputManager.isItScore = false;
    }

    public void nextScene()
    {
        LevelManager.Instance.loadNextScene();

        SceneManager.LoadScene("Level" + LevelManager.Instance.currentLevel);
        inputManager.isItScore = false;
    }
}
