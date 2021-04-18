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

    Vector2 m_stableDeltaPos;
    Vector2 m_startPos;
    Vector2 m_deltaPos;

    public bool isEnded = false;
    bool useFuel = false;
    bool timerBool = false;
    bool shot = false;
    bool isStarted = false;
    bool explosionTimer = false;
    bool forward = false;

    public CityColliders cc;
    public State currentState;
    public Rigidbody rb;
    public GameObject explosion;
    public ParticleSystem rocketFire;
    public ParticleSystem crashExplosion;
    public ParticleSystem fuelExp;
    public ParticleSystem speedParticle;
    public ParticleSystem frictionParticle;
    public ParticleSystem crashSmoke;
    public TrailRenderer[] rocketTrails;

    public float forceMultiplier;
    public float moveSmoother;
    public float swerveSpeed;
    public float swerveForce;
    public float swipeGap;
    public float startFuel = 200;
    public float speed;
    public float rocketForwardSpeed;
    private float fallSpeed = 75;
    float startingXPos;
    float fuel;
    float timer = 2;
    private float crashTimer = 2;
    float useFuelMult;
    float roll;
    float rotate;
    float speedSwitch = 1;
    float slingRotate = -90;
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
    }
    private void Update()
    {
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
                    DrawTrajectory.Instance.UpdateTrajectory(forceV, rb, new Vector3(transform.position.x, transform.position.y, transform.position.z + 200));
                }
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
                float rotateRate = (transform.position.x - startingXPos) / 4.5f;
                slingRotate = Mathf.Lerp(slingRotate, 60, 0.004f);

                if (transform.position.x < startingXPos)
                {
                    Debug.LogWarning("dönüş");
                    transform.DOLocalRotate(new Vector3(slingRotate, rotateRate, 0), 0f);
                }
                if (transform.position.x > startingXPos)
                {
                    Debug.LogWarning("dönüş");
                    transform.DOLocalRotate(new Vector3(slingRotate, rotateRate, 0), 0f);
                }
            }
            if (forward)
            {
                speedSwitch = Mathf.Lerp(speedSwitch, 1, 0.05f);
                transform.Translate(Vector3.forward * rocketForwardSpeed * speedSwitch * Time.deltaTime);
            }
            rb.constraints = RigidbodyConstraints.None;

            #region Swerve
            if (Input.GetMouseButtonDown(0))
            {
                shot = false;
                rb.velocity = Vector3.zero;
                m_startPos = Input.mousePosition;
                useFuel = false;
                forward = true;
                m_stableDeltaPos = Input.mousePosition;
            }

            if (Input.GetMouseButton(0))
            {
                transform.DOLocalRotate(new Vector3(-60, rotate, roll), 0.5f); //Up force
                m_deltaPos = (Vector2)Input.mousePosition - m_startPos;
                fuel -= 0.2f;


                //Swerve Input
                transform.position =
               new Vector3(Mathf.Lerp(transform.position.x, transform.position.x + (m_deltaPos.x / Screen.width) * swerveSpeed, Time.deltaTime * moveSmoother)
               , transform.position.y, transform.position.z);

                if (m_deltaPos.x > 0)
                {
                    transform.DORotate(new Vector3(0, 0, -60), 0.5f);
                    transform.DOLocalRotate(new Vector3(0, 30, 0), 0.5f);
                }
                if (m_deltaPos.x < 0)
                {
                    transform.DORotate(new Vector3(0, 0, 60), 0.5f);
                    transform.DOLocalRotate(new Vector3(0, -30, 0), 0.5f);
                }
                else
                {
                    //transform.DORotate(Vector3.zero, 0.5f);
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
                    transform.DOLocalRotate(new Vector3(0, 0, 0), 1f);
                }

                else if (!shot)
                {
                    transform.DOLocalRotate(new Vector3(60, 0, 0), 1f);
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
        if (fuel <= 0)
        {
            currentState = State.Fall;
        }
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
    public void Shoot(Vector3 Force)
    {
        if (shot)
            return;

        useFuelMult = Force.y;
        rb.AddForce(new Vector3(Force.x, Force.y, Force.y) * forceMultiplier, ForceMode.Impulse);
        speedSwitch = (Force.y * forceMultiplier) / 20;
        currentState = State.Fly;
        rb.useGravity = false;
        shot = true;
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

            rocketForwardSpeed = Mathf.Lerp(rocketForwardSpeed, speed * 2.4f, 0.7f);

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
            #endregion
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        explosionTimer = true;
        transform.DORotate(Vector3.zero, 0.5f);
    }
    #endregion

    #region Methods
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
