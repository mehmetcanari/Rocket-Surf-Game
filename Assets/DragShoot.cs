using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class DragShoot : MonoBehaviour
{
    Vector3 mousePressDownPos;
    Vector3 mouseReleasePos;

    Vector2 m_startPos;
    Vector2 m_deltaPos;
    
    public bool isEnded = false;
    bool useFuel = false;
    bool timerBool = false;
    bool shot = false;
    bool isUp = false;

    Rigidbody rb;


    public State currentState;
    public GameObject explosion;
    public ParticleSystem rocketFire;
    public ParticleSystem crashExplosion;
    public ParticleSystem fuelExp;
    public TrailRenderer[] rocketTrails;

    public float forceMultiplier;
    public float swerveForce;
    public float swipeGap;
    public float startFuel = 100;
    public float speed;
    float fuel;
    float usedSpeed;
    float timer = 2;
    float useFuelMult;
    float rotate;
    float roll;
    public int scoreCount = 1;

    public Image fuelBar;
    public Image scoreBar;
    public Image good;
    public Image amazing;
    public Image perfect;

    public enum State
    {
        Idle,
        Fly,
        Fall,
        Crash,
    }
    void Start()
    {
        isEnded = false;
        gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        currentState = State.Idle;
        rb = GetComponent<Rigidbody>();
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
        rocketTrails = GetComponentsInChildren<TrailRenderer>();
        usedSpeed = speed;
        fuel = startFuel;
    }
    private void Update()
    {
        #region Rocket Trail Timer
        if (timerBool)
        {
            timer -= Time.deltaTime;
            //Debug.Log(timer);
        }

        if (timer <= 0)
        {
            foreach (var item in rocketTrails)
            {
                item.gameObject.GetComponent<TrailRenderer>().time =
                    Mathf.Lerp(item.gameObject.GetComponent<TrailRenderer>().time, 0, 0.25f);
            }
        }
        #endregion

        #region States
        if (currentState == State.Crash)
        {
            isEnded = true;
        }

        if (currentState == State.Idle)
        {
            rocketFire.gameObject.SetActive(false);

            if (Input.GetMouseButtonDown(0))
            {
                mousePressDownPos = Input.mousePosition;
            }
            if (Input.GetMouseButton(0))
            {
                Vector3 forceInit = (mousePressDownPos - Input.mousePosition);
                Vector3 forceV = (new Vector3(forceInit.x, forceInit.y, forceInit.y)) * forceMultiplier;


                if (!shot)
                {
                    DrawTrajectory.Instance.UpdateTrajectory(forceV, rb, transform.position);
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                DrawTrajectory.Instance.HideLine();
                mouseReleasePos = Input.mousePosition;
                Shoot(mousePressDownPos - mouseReleasePos);
                useFuel = true;
            }
            if (transform.position.y > 18)
            {
                currentState = State.Fly;
            }
        }
        if (useFuel)
        {
            fuel -= 0.1f * (useFuelMult / 100);
        }
        if (currentState == State.Fly)
        {
            rocketFire.gameObject.SetActive(true);
            transform.Translate(Vector3.forward * usedSpeed * Time.deltaTime);
            rb.constraints = RigidbodyConstraints.None;

            #region Swerve
            if (Input.GetMouseButtonDown(0))
            {
                rb.velocity = Vector3.zero;
                m_startPos = Input.mousePosition;
                useFuel = false;
            }
            if (Input.GetMouseButton(0))
            {
                isUp = true;
                transform.DOLocalRotate(new Vector3(-60, 0, roll), 1.2f);
                m_deltaPos = (Vector2)Input.mousePosition - m_startPos;
                fuel -= 0.1f;

                if (Mathf.Abs(m_deltaPos.x) > Screen.width * swipeGap) // If slide 
                {
                    if (m_deltaPos.x > 0) //Delta X Positive
                    {
                        transform.DOLocalRotate(new Vector3(0, 60, 0), 0.5f);
                        //rotate = 60;
                    }

                    if (m_deltaPos.x < 0) //Delta X Negative
                    {
                        transform.DOLocalRotate(new Vector3(0, -60, 0), 0.5f);
                        //rotate = -60;
                    }
                }
                //if (m_deltaPos.x == 0)
                //{
                //    rotate = 0;
                //}
                m_startPos = Input.mousePosition;


            }
            if (Input.GetMouseButtonUp(0))
            {
                isUp = false;
                if (!isUp)
                {
                    transform.DOLocalRotate(new Vector3(60, 0, 0), 2f);
                }
            }
            #endregion
        }
        else
        {
            rocketFire.gameObject.SetActive(false);
        }

        if (currentState == State.Fall)
        {
            transform.DORotate(new Vector3(60, 0, 0), 1.2f);
            transform.Translate(Vector3.forward * 75 * Time.deltaTime);
            rb.velocity = Vector3.zero;

            if (Input.GetMouseButton(0) && fuel > 0)
            {
                currentState = State.Fly;
            }
        }
        #endregion

        #region Fuel
        if (fuel <= 0)
        {
            currentState = State.Fall;
        }

        fuelBar.fillAmount = fuel / startFuel;
        #endregion

        #region Rewards
        if (transform.position.z > 370)
        {
            good.gameObject.SetActive(true);
        }
        if (transform.position.z > 370)
        {
            amazing.gameObject.SetActive(true);
        }
        if (transform.position.z > 370)
        {
            perfect.gameObject.SetActive(true);
        }
        #endregion
    }
    void Shoot(Vector3 Force)
    {
        if (shot)
            return;
        Debug.Log("Shoot");
        useFuelMult = Force.y;
        rb.AddForce(new Vector3(Force.x, Force.y, Force.y) * forceMultiplier, ForceMode.Impulse);
        currentState = State.Fly;
        rb.useGravity = false;
        shot = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Fuel")
        {
            fuel += 15;
            Destroy(other.gameObject);
            Instantiate(fuelExp, other.transform.position, Quaternion.identity);
        }


        if (other.gameObject.tag == "Ring")
        {
            timerBool = true;
            timer = 2;

            foreach (var item in rocketTrails)
            {
                item.gameObject.GetComponent<TrailRenderer>().time =
                    Mathf.Lerp(item.gameObject.GetComponent<TrailRenderer>().time, 1, 1f);
            }
            usedSpeed = speed * 2;
            roll = 250;
            transform.DORotate(new Vector3(0, 0, 250), 2f).OnComplete(() =>
            {
                transform.DORotate(new Vector3(60, 0, 0), 1.2f);
                roll = 0;
                usedSpeed = speed;
            });
        }
        if (other.gameObject.tag != "Fuel" && other.gameObject.tag != "Ring")
        {
            isEnded = true;
            currentState = State.Crash;
            Instantiate(explosion, transform.position, Quaternion.identity);
            Instantiate(crashExplosion, transform.position, Quaternion.identity);
            gameObject.SetActive(false);
            Invoke("Restart", 4);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        isEnded = true;
        currentState = State.Crash;
        Instantiate(explosion, transform.position, Quaternion.identity);
        Instantiate(crashExplosion, transform.position, Quaternion.identity);
        gameObject.SetActive(false);
        Invoke("Restart", 4);
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }
}
