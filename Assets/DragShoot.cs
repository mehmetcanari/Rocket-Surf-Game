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

    private Vector2 m_startPos;
    private Vector2 m_deltaPos;
    public bool isEnded = false;
    bool once = true;
    private bool timerBool = false;

    Rigidbody rb;

    bool shot = false;
    bool count = false;
    bool isUp = false;

    public State currentState;
    public GameObject explosion;
    public ParticleSystem rocketFire;
    public ParticleSystem crashExplosion;
    public ParticleSystem fuelExp;
    public TrailRenderer[] rocketTrails;

    public float forceMultiplier;
    public float swerveForce;
    public float swipeGap;
    private float fuel;
    public float speed;
    private float timer = 2;
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
            }
            if (transform.position.y > 18)
            {
                currentState = State.Fly;
            }
        }

        if (currentState == State.Fly)
        {
            rocketFire.gameObject.SetActive(true);
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            rb.constraints = RigidbodyConstraints.None;


            if (count)
            {
                if (fuel < scoreCount)
                {
                    rb.useGravity = true;
                    currentState = State.Fall;
                    Debug.Log("Fall");
                }
                count = false;
            }

            #region Swerve
            if (Input.GetMouseButtonDown(0))
            {
                rb.velocity = Vector3.zero;
                m_startPos = Input.mousePosition;
            }
            if (Input.GetMouseButton(0))
            {
                isUp = true;
                transform.DOLocalRotate(new Vector3(-60, 0, 0), 1.2f);
                m_deltaPos = (Vector2)Input.mousePosition - m_startPos;

                if (Mathf.Abs(m_deltaPos.x) > Screen.width * swipeGap) // If slide 
                {
                    if (m_deltaPos.x > 0) //Delta X Positive
                    {
                        Debug.Log("Swerve");
                        transform.DOLocalRotate(new Vector3(0, 60, 0), 0.5f);
                    }

                    if (m_deltaPos.x < 0) //Delta X Negative
                    {
                        transform.DOLocalRotate(new Vector3(0, -60, 0), 0.5f);
                    }
                }
                m_startPos = Input.mousePosition;


            }
            if (Input.GetMouseButtonUp(0))
            {
                isUp = false;
                if (!isUp)
                {
                    transform.DOLocalRotate(new Vector3(60, 0, 0), 1.2f);
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
            transform.DORotate(new Vector3(45, 0, 0), 5);
            transform.Translate(Vector3.forward * 25 * Time.deltaTime);
        }

        #endregion

        #region Fuel Bar Color
        if (fuel >= 4 && fuel < 8)
        {
            fuelBar.color = new Color(255 / 255f, 105 / 255f, 0f, 1f);
            if (once && good != null)
            {
                good.gameObject.SetActive(true);
                once = false;
            }

            Invoke("RewardFalse", 1.5f);
        }
        if (fuel >= 8 && fuel < 16)
        {
            fuelBar.color = new Color(155 / 255f, 0f, 1f, 1f);
            if (once && amazing != null)
            {
                amazing.gameObject.SetActive(true);
                once = false;
            }
            Invoke("RewardFalse", 1.5f);
        }
        if (fuel >= 16)
        {
            fuelBar.color = new Color(0f, 0f, 1f, 1f);
            if (once && perfect != null)
            {
                perfect.gameObject.SetActive(true);
                once = false;
            }
            Invoke("RewardFalse", 1.5f);
        }
        #endregion

        #region Score Bar Color
        if (scoreCount == 4)
        {
            scoreBar.color = new Color(255 / 255f, 105 / 255f, 0f, 1f);

        }
        if (scoreCount == 8)
        {
            scoreBar.color = new Color(155 / 255f, 0f, 1f, 1f);

        }
        if (scoreCount == 16)
        {
            scoreBar.color = new Color(0f, 0f, 1f, 1f);

        }
        #endregion
    }
    void Shoot(Vector3 Force)
    {
        if (shot)
            return;
        Debug.Log("Shoot");
        rb.AddForce(new Vector3(Force.x, Force.y, Force.y) * forceMultiplier, ForceMode.Impulse);
        currentState = State.Fly;
        rb.useGravity = false;
        shot = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Fuel")
        {
            fuel++;
            fuelBar.fillAmount += 0.0625f;
            Destroy(other.gameObject);
            Instantiate(fuelExp, other.transform.position, Quaternion.identity);
        }

        if (other.gameObject.tag == "ScoreCount")
        {
            scoreCount += scoreCount;
            count = true;
        }

        if (other.gameObject.tag != "Fuel" && other.gameObject.tag != "ScoreCount" && other.gameObject.tag != "Ring")
        {
            isEnded = true;
            currentState = State.Crash;
            Instantiate(explosion, transform.position, Quaternion.identity);
            Instantiate(crashExplosion, transform.position, Quaternion.identity);
            gameObject.SetActive(false);
            Invoke("Restart", 4);
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
                       
            transform.DORotate(new Vector3(0, 0, 250), 2f).OnComplete(() =>
            {
                transform.DORotate(Vector3.zero, 1f);
                speed = speed * 1.1f; 
            });
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        isEnded = true;
        Instantiate(explosion, transform.position, Quaternion.identity);
        Instantiate(crashExplosion, transform.position, Quaternion.identity);
        gameObject.SetActive(false);
        Invoke("Restart", 4);
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }

    void RewardFalse()
    {
        if (good != null)
        {
            if (good.gameObject.activeSelf)
            {
                Destroy(good);
                once = true;
                return;
            }
        }
        if (amazing != null)
        {
            if (amazing.gameObject.activeSelf)
            {
                Destroy(amazing);
                once = true;
                return;
            }
        }
        if (perfect != null)
        {
            if (perfect.gameObject.activeSelf)
            {
                Destroy(perfect);
                once = true;
                return;
            }
        }
    }
}
