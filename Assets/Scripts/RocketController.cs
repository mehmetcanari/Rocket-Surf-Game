using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class RocketController : MonoBehaviour
{
    private Vector3 mousePressDownPos;
    private Vector3 mouseReleasePos;

    private Vector2 m_stableDeltaPos;
    private Vector2 m_startPos;
    private Vector2 m_deltaPos;

    public bool cameraRay = false;
    public bool isEnded = false;
    private bool useFuel = false;
    private bool timerBool = false;
    private bool shot = false;
    private bool isStarted = false;
    private bool explosionTimer = false;
    private bool forward = false;
    private bool tutorialStop = true;

    public State currentState;
    public CityColliders cc;
    public Rigidbody rb;
    public GameObject explosion;
    public GameObject swerveTutorial;
    public GameObject[] pointImages;
    public TrailRenderer[] rocketTrails;
    public LineRenderer lr;
    public ScoreCounter sc;

    [Header("Particles")]
    public ParticleSystem rocketFire;
    public ParticleSystem crashExplosion;
    public ParticleSystem fuelExp;
    public ParticleSystem speedParticle;
    public ParticleSystem frictionParticle;
    public ParticleSystem crashSmoke;
    public ParticleSystem ringDestroyParticle;


    [Header("Rocket Movement")]
    public float forceMultiplier;
    public float moveSmoother;
    public float swerveSpeed;
    public float swerveForce;
    public float swipeGap;
    public float startFuel = 200;
    public float speed;
    public float rocketForwardSpeed;

    public int scoreCount = 1;
    public float colorLerp = 0.1f;
    private float fallSpeed = 75;
    private float startingXPos;
    private float fuel;
    private float timer = 2;
    private float crashTimer = 0.5f;
    private float useFuelMult;
    private float roll;
    private float rotate;
    private float speedSwitch = 1;
    private float slingRotate = -90;
    private float colorR = 0;
    private float colorG = 0;

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
        #region Start Arguments
        cameraRay = false;
        tutorialStop = true;
        pointImages = GameObject.FindGameObjectsWithTag("PointImage");
        foreach (var item in cc.cities)
        {
            item.gameObject.GetComponent<MeshRenderer>().receiveShadows = false;
            item.gameObject.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }

        isEnded = false;
        isStarted = false;
        gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        currentState = State.Idle;
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
        rocketTrails = GetComponentsInChildren<TrailRenderer>();
        rocketForwardSpeed = speed;
        fuel = startFuel;
        startingXPos = transform.position.x;
        #endregion
    }
    private void Update()
    {
        #region Tutorial
        if (gameObject.transform.position.y >= 180 && tutorialStop)
        {
            Debug.Log("Tutorial");
            Time.timeScale = 0f;
            swerveTutorial.SetActive(true);
            swerveTutorial.GetComponent<Animator>().SetBool("scaleAnim", true);
            if (Input.GetMouseButtonDown(0))
            {
                tutorialStop = false;
                Time.timeScale = 1f;
                swerveTutorial.GetComponent<Animator>().SetBool("scaleAnim", false);
            }
        }
        if (swerveTutorial.transform.localScale == Vector3.zero && !tutorialStop)
        {
            swerveTutorial.SetActive(false);
        }
        #endregion

        #region Reset Input
        if (Input.GetKey(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }
        #endregion

        #region Explosion Timer
        if (explosionTimer)
        {
            frictionParticle.gameObject.SetActive(true);
            crashTimer -= Time.deltaTime;
            cameraRay = true;
        }

        if (crashTimer <= 0)
        {
            frictionParticle.gameObject.SetActive(false);
            Instantiate(crashSmoke, transform.position, Quaternion.identity);
            crashSmoke.transform.position = gameObject.transform.position;
            currentState = State.Crash;
            Instantiate(explosion, transform.position, Quaternion.identity);
            Instantiate(crashExplosion, transform.position, Quaternion.identity);
            gameObject.SetActive(false);
            Invoke("Restart", 6);
        }

        #endregion

        #region Rocket Trail Timer
        if (timerBool)
        {
            timer -= Time.deltaTime;
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

        #region Crash State
        if (currentState == State.Crash)
        {
            isEnded = true;
            sc.CalculateScore();
        }
        #endregion

        #region Idle State
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
                    DrawTrajectory.Instance.UpdateTrajectory(forceV, rb, new Vector3(transform.position.x, transform.position.y, transform.position.z + 50));
                }

                #region LineRenderer Color
                Vector3 mousePos = Input.mousePosition;
                float xValue = (mousePressDownPos.x - mousePos.x);

                lr.startColor = new Color(colorR, colorG, 0, 1);
                lr.endColor = new Color(colorR, colorG, 0, 1);

                if (xValue < -850 || xValue > 850)
                {
                    colorR = Mathf.Lerp(colorR, 1, colorLerp);
                    colorG = Mathf.Lerp(colorG, 0, colorLerp);
                }
                if (xValue < -200 && xValue > -850 || xValue > 200 && xValue < 850)
                {
                    colorR = Mathf.Lerp(colorR, 1, colorLerp);
                    colorG = Mathf.Lerp(colorG, 1, colorLerp);
                }
                if (xValue > -200 && xValue < 200)
                {
                    colorR = Mathf.Lerp(colorR, 0, colorLerp);
                    colorG = Mathf.Lerp(colorG, 1, colorLerp);
                }
                #endregion

            }
            if (Input.GetMouseButtonUp(0) && !isStarted) //Trajectory Shoot
            {
                DrawTrajectory.Instance.HideLine();
                mouseReleasePos = Input.mousePosition;
                Shoot(mousePressDownPos - mouseReleasePos);
                useFuel = true;
                isStarted = true;
            }

            if (transform.position.y > 18)
            {
                currentState = State.Fly;
            }
        }

        if (useFuel)
        {
            fuel -= 0.2f * (useFuelMult / 100);
        }
        #endregion

        #region Fly State
        if (currentState == State.Fly)
        {
            rocketFire.gameObject.SetActive(true);

            if (!forward) // Left & right rotation in sling shot 
            {
                float rotateRate = (transform.position.x - startingXPos) / 2.5f;
                slingRotate = Mathf.Lerp(slingRotate, 60, 0.002f);

                if (transform.position.x < startingXPos)
                {
                    transform.DOLocalRotate(new Vector3(slingRotate, rotateRate, 0), 0f);
                }
                if (transform.position.x > startingXPos)
                {
                    transform.DOLocalRotate(new Vector3(slingRotate, rotateRate, 0), 0f);
                }
            }
            if (forward)
            {
                speedSwitch = Mathf.Lerp(speedSwitch, 1, 0.05f);
                transform.Translate(Vector3.forward * rocketForwardSpeed * speedSwitch * Time.deltaTime);
            }
            if (fuel <= 0)
            {
                currentState = State.Fall;
            }
            rb.constraints = RigidbodyConstraints.None;

            #region Swerve
            if (!tutorialStop) // If the tutorial appear, game will stop
            {
                if (Input.GetMouseButtonDown(0))
                {
                    shot = false;
                    rb.velocity = Vector3.zero;
                    m_startPos = Input.mousePosition;
                    useFuel = false;
                    forward = true;
                    m_stableDeltaPos = Input.mousePosition;

                    //rocketForwardSpeed += 50;

                    rocketForwardSpeed = Mathf.Lerp(rocketForwardSpeed, rocketForwardSpeed + 50, 0.7f);
                }

                if (Input.GetMouseButton(0))
                {
                    transform.DOLocalRotate(new Vector3(-60, rotate, roll), 0.5f); //Up force
                    m_deltaPos = (Vector2)Input.mousePosition - m_startPos;
                    fuel -= 20f * Time.deltaTime;

                    foreach (var item in rocketTrails)
                    {
                        item.gameObject.GetComponent<TrailRenderer>().time =
                            Mathf.Lerp(item.gameObject.GetComponent<TrailRenderer>().time, 1, 1f);
                    }


                    //Swerve Input
                    transform.position =
                   new Vector3(Mathf.Lerp(transform.position.x, transform.position.x + (m_deltaPos.x / Screen.width) * swerveSpeed, Time.deltaTime * moveSmoother)
                   , transform.position.y, transform.position.z);

                    if (Mathf.Abs(m_deltaPos.x) > Screen.width * swipeGap)
                    {

                        if (m_deltaPos.x > 0)
                        {
                            transform.DORotate(new Vector3(0, 0, -60), 0.5f);
                            //transform.DOLocalRotate(new Vector3(0, 30, 0), 0.5f);
                            transform.Rotate(new Vector3(0, 30 * Time.deltaTime, 0));
                        }

                        if (m_deltaPos.x < 0)
                        {
                            transform.DORotate(new Vector3(0, 0, 60), 0.5f);
                            //transform.DOLocalRotate(new Vector3(0, -30, 0), 0.5f);
                            transform.Rotate(new Vector3(0, -30 * Time.deltaTime, 0));
                        }
                    }

                    if (m_startPos.x == m_stableDeltaPos.x)
                    {
                        rotate = 0;
                    }

                    //Local Rotate Input
                    //if (Mathf.Abs(m_deltaPos.x) > Screen.width * swipeGap) // If slide 
                    //{
                    //    if (m_deltaPos.x > 0) //Delta X Positive
                    //    {
                    //        rotate = 60;
                    //    }
                    //    if (m_deltaPos.x < 0) //Delta X Negative
                    //    {
                    //        rotate = -60;
                    //    }
                    //}
                    //if (m_startPos.x == m_stableDeltaPos.x)
                    //{
                    //    rotate = 0;
                    //}
                    m_startPos = Input.mousePosition;


                }
                if (Input.GetMouseButtonUp(0))
                {
                    if (shot)
                    {
                        transform.DORotate(new Vector3(0, 0, 0), 1f);
                    }

                    else if (!shot)
                    {
                        transform.DOLocalRotate(new Vector3(60, 0, 0), 1f);

                        foreach (var item in rocketTrails)
                        {
                            item.gameObject.GetComponent<TrailRenderer>().time =
                              Mathf.Lerp(item.gameObject.GetComponent<TrailRenderer>().time, 0, 1f);
                        }
                        //rocketForwardSpeed -= 50;
                        rocketForwardSpeed = Mathf.Lerp(rocketForwardSpeed, rocketForwardSpeed - 50, 0.7f);
                    }
                }
            }

            #endregion
        }
        else
        {
            rocketFire.gameObject.SetActive(false);
        }
        #endregion

        #region Fall State
        if (currentState == State.Fall)
        {
            transform.DORotate(new Vector3(60, 0, 0), 1.2f);
            transform.Translate(Vector3.forward * fallSpeed * Time.deltaTime);
            rb.velocity = Vector3.zero;

            fallSpeed += 20 * Time.deltaTime;

            if (Input.GetMouseButton(0) && fuel > 0)
            {
                currentState = State.Fly;
            }
        }
        #endregion

        #endregion

        #region Fuel
        if (fuel > startFuel)
        {
            fuel = startFuel;
        }
        fuelBar.fillAmount = fuel / startFuel;
        #endregion

        #region Rewards
        if (transform.position.z > 370 && transform.position.z < 375)
        {
            good.gameObject.SetActive(true);
            good.transform.DOScale(new Vector3(5, 1.5f, 5), 0.5f);
            Invoke("GoodFalse", 2);
        }
        if (transform.position.z > 880 && transform.position.z < 885)
        {
            amazing.gameObject.SetActive(true);
            amazing.transform.DOScale(new Vector3(5, 1.5f, 5), 0.5f);
            Invoke("AmazingFalse", 2);
        }
        if (transform.position.z > 1385 && transform.position.z < 1390)
        {
            perfect.gameObject.SetActive(true);
            perfect.transform.DOScale(new Vector3(5, 1.5f, 5), 0.5f);
            Invoke("PerfectFalse", 2);
        }
        #endregion
    }

    #region Triggers / Colliders
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

            rocketForwardSpeed = Mathf.Lerp(rocketForwardSpeed, speed * 3.4f, 0.8f);

            roll = 250;
            speedParticle.gameObject.SetActive(true);
            transform.DORotate(new Vector3(0, 0, 250), 2f).OnComplete(() =>
            {
                transform.DORotate(new Vector3(60, 0, 0), 1.2f);
                roll = 0;

                rocketForwardSpeed = Mathf.Lerp(speed, rocketForwardSpeed, 0.7f);
                speedParticle.gameObject.SetActive(false);
            });
        }
        if (other.gameObject.tag != "Fuel" && other.gameObject.tag != "Ring")
        {
            //Sets the explosion countdown while rocket breaking the buildings
            explosionTimer = true;
            foreach (var image in pointImages)
            {
                image.SetActive(false);
            }

            #region White Building Pre Explosion
            if (other.gameObject.tag == "White1building")
            {
                Instantiate(explosion, transform.position, Quaternion.identity);
                explosion.transform.DOScale(new Vector3(0.01f, 0.01f, 0.01f), 0);
                currentState = State.Fall;
            }
            else if (other.gameObject.tag == "White2building")
            {
                Instantiate(explosion, transform.position, Quaternion.identity);
                explosion.transform.DOScale(new Vector3(0.01f, 0.01f, 0.01f), 0);
                currentState = State.Fall;
            }
            else if (other.gameObject.tag == "White3building")
            {
                Instantiate(explosion, transform.position, Quaternion.identity);
                explosion.transform.DOScale(new Vector3(0.01f, 0.01f, 0.01f), 0);
                currentState = State.Fall;
            }
            else if (other.gameObject.tag == "Whitetownhall")
            {
                Instantiate(explosion, transform.position, Quaternion.identity);
                explosion.transform.DOScale(new Vector3(0.01f, 0.01f, 0.01f), 0);
                currentState = State.Fall;
            }
            #endregion

            #region Orange Building Pre Explosion
            if (other.gameObject.tag == "Orange1building")
            {
                Instantiate(explosion, transform.position, Quaternion.identity);
                explosion.transform.DOScale(new Vector3(0.01f, 0.01f, 0.01f), 0);
                currentState = State.Fall;
            }
            else if (other.gameObject.tag == "Orange2building")
            {
                Instantiate(explosion, transform.position, Quaternion.identity);
                explosion.transform.DOScale(new Vector3(0.01f, 0.01f, 0.01f), 0);
                currentState = State.Fall;
            }
            else if (other.gameObject.tag == "Orange3building")
            {
                Instantiate(explosion, transform.position, Quaternion.identity);
                explosion.transform.DOScale(new Vector3(0.01f, 0.01f, 0.01f), 0);
                currentState = State.Fall;
            }
            else if (other.gameObject.tag == "Orangetownhall")
            {
                Instantiate(explosion, transform.position, Quaternion.identity);
                explosion.transform.DOScale(new Vector3(0.01f, 0.01f, 0.01f), 0);
                currentState = State.Fall;
            }
            #endregion

            #region Blue Building Pre Explosion
            if (other.gameObject.tag == "Blue1building")
            {
                Instantiate(explosion, transform.position, Quaternion.identity);
                explosion.transform.DOScale(new Vector3(0.01f, 0.01f, 0.01f), 0);
                currentState = State.Fall;
            }
            else if (other.gameObject.tag == "Blue2building")
            {
                Instantiate(explosion, transform.position, Quaternion.identity);
                explosion.transform.DOScale(new Vector3(0.01f, 0.01f, 0.01f), 0);
                currentState = State.Fall;
            }
            else if (other.gameObject.tag == "Blue3building")
            {
                Instantiate(explosion, transform.position, Quaternion.identity);
                explosion.transform.DOScale(new Vector3(0.01f, 0.01f, 0.01f), 0);
                currentState = State.Fall;
            }
            else if (other.gameObject.tag == "Bluetownhall")
            {
                Instantiate(explosion, transform.position, Quaternion.identity);
                explosion.transform.DOScale(new Vector3(0.01f, 0.01f, 0.01f), 0);
                currentState = State.Fall;
            }
            #endregion

            #region Purple Building Pre Explosion
            if (other.gameObject.tag == "Purple1building")
            {
                Instantiate(explosion, transform.position, Quaternion.identity);
                explosion.transform.DOScale(new Vector3(0.01f, 0.01f, 0.01f), 0);
                currentState = State.Fall;
            }
            else if (other.gameObject.tag == "Purple2building")
            {
                Instantiate(explosion, transform.position, Quaternion.identity);
                explosion.transform.DOScale(new Vector3(0.01f, 0.01f, 0.01f), 0);
                currentState = State.Fall;
            }
            else if (other.gameObject.tag == "Purple3building")
            {
                Instantiate(explosion, transform.position, Quaternion.identity);
                explosion.transform.DOScale(new Vector3(0.01f, 0.01f, 0.01f), 0);
                currentState = State.Fall;
            }
            else if (other.gameObject.tag == "Purpletownhall")
            {
                Instantiate(explosion, transform.position, Quaternion.identity);
                explosion.transform.DOScale(new Vector3(0.01f, 0.01f, 0.01f), 0);
                currentState = State.Fall;
            }
            #endregion

            #region Trees Pre Explosion
            if (other.gameObject.tag == "kısacam")
            {
                Instantiate(explosion, transform.position, Quaternion.identity);
                explosion.transform.DOScale(new Vector3(0.01f, 0.01f, 0.01f), 0);
                currentState = State.Fall;
            }
            else if (other.gameObject.tag == "yamukagac")
            {
                Instantiate(explosion, transform.position, Quaternion.identity);
                explosion.transform.DOScale(new Vector3(0.01f, 0.01f, 0.01f), 0);
                currentState = State.Fall;
            }
            else if (other.gameObject.tag == "agacuc")
            {
                Instantiate(explosion, transform.position, Quaternion.identity);
                explosion.transform.DOScale(new Vector3(0.01f, 0.01f, 0.01f), 0);
                currentState = State.Fall;
            }
            #endregion

        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        explosionTimer = true;
        foreach (var image in pointImages)
        {
            image.SetActive(false);
        }
        transform.DORotate(Vector3.zero, 0.2f);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Ring")
        {
            Destroy(other.gameObject, 0.5f);
            Instantiate(ringDestroyParticle, other.gameObject.transform.position, Quaternion.identity);
        }
    }
    #endregion

    #region Methods
    public void Shoot(Vector3 Force)
    {
        if (shot)
            return;

        //useFuelMult = Force.y;
        rb.AddForce(new Vector3(Force.x, Force.y, Force.y) * forceMultiplier, ForceMode.Impulse);
        speedSwitch = (Force.y * forceMultiplier) / 20;
        currentState = State.Fly;
        rb.useGravity = false;
        shot = true;
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }

    void GoodFalse()
    {
        good.transform.DOScale(Vector3.zero, 0.5f).OnComplete(() =>
        {
            good.gameObject.SetActive(false);
        });
    }
    void AmazingFalse()
    {
        amazing.transform.DOScale(Vector3.zero, 0.5f).OnComplete(() =>
        {
            amazing.gameObject.SetActive(false);
        });
    }
    void PerfectFalse()
    {
        perfect.transform.DOScale(Vector3.zero, 0.5f).OnComplete(() =>
        {
            perfect.gameObject.SetActive(false);
        });
    }
    #endregion
}
