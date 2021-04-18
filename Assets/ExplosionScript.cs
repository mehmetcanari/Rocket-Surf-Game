using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionScript : MonoBehaviour
{
    GameObject scoreCounter;

    public GameObject White1buildingFracturedPre;
    public GameObject White2buildingFracturedPre;
    public GameObject White3buildingFracturedPre;
    public GameObject WhitetownhallFracturedPre;

    public GameObject FiskiyeFracturedPre;

    GameObject FiskiyeFractured;

    GameObject White1buildingFractured;
    GameObject White2buildingFractured;
    GameObject White3buildingFractured;
    GameObject WhitetownhallFractured;


    public GameObject Orange1buildingFracturedPre;
    public GameObject Orange2buildingFracturedPre;
    public GameObject Orange3buildingFracturedPre;
    public GameObject OrangetownhallFracturedPre;

    GameObject Orange1buildingFractured;
    GameObject Orange2buildingFractured;
    GameObject Orange3buildingFractured;
    GameObject OrangetownhallFractured;

    public GameObject Purple1buildingFracturedPre;
    public GameObject Purple2buildingFracturedPre;
    public GameObject Purple3buildingFracturedPre;
    public GameObject PurpletownhallFracturedPre;

    GameObject Purple1buildingFractured;
    GameObject Purple2buildingFractured;
    GameObject Purple3buildingFractured;
    GameObject PurpletownhallFractured;

    public GameObject Blue1buildingFracturedPre;
    public GameObject Blue2buildingFracturedPre;
    public GameObject Blue3buildingFracturedPre;
    public GameObject BluetownhallFracturedPre;

    GameObject Blue1buildingFractured;
    GameObject Blue2buildingFractured;
    GameObject Blue3buildingFractured;
    GameObject BluetownhallFractured;
    private void Start()
    {
        transform.localScale = new Vector3(0, 0, 0);
        scoreCounter = GameObject.FindGameObjectWithTag("ScoreCounter");
    }
    void Update()
    {
        transform.localScale += new Vector3(5f, 5f, 5f);
        GetComponent<MeshRenderer>().enabled = false;
        Destroy(gameObject, 1f);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "PointImage")
        {
            Destroy(other.gameObject);
        }
        if (other.gameObject.tag == "White1building")
        {
            Destroy(other.gameObject);
            White1buildingFractured = Instantiate(White1buildingFracturedPre, other.transform.position, Quaternion.identity);
            White1buildingFractured.transform.localScale = other.gameObject.transform.localScale;
            White2buildingFractured.transform.rotation = other.transform.rotation;
            scoreCounter.GetComponent<ScoreCounter>().score += 10;
        }
        if (other.gameObject.tag == "White2building")
        {
            Destroy(other.gameObject);
            White2buildingFractured = Instantiate(White2buildingFracturedPre, other.transform.position, Quaternion.identity);
            White2buildingFractured.transform.localScale = other.gameObject.transform.localScale;
            White2buildingFractured.transform.rotation = other.transform.rotation;
            scoreCounter.GetComponent<ScoreCounter>().score += 5;
        }
        if (other.gameObject.tag == "White3building")
        {
            Destroy(other.gameObject);
            White3buildingFractured = Instantiate(White3buildingFracturedPre, other.transform.position, Quaternion.identity);
            White3buildingFractured.transform.localScale = other.gameObject.transform.localScale;
            White3buildingFractured.transform.rotation = other.transform.rotation;
            scoreCounter.GetComponent<ScoreCounter>().score += 25;
        }
        if (other.gameObject.tag == "Whitetownhall")
        {
            Destroy(other.gameObject);
            WhitetownhallFractured = Instantiate(WhitetownhallFracturedPre, other.transform.position, Quaternion.identity);
            WhitetownhallFractured.transform.localScale = other.gameObject.transform.localScale;
            WhitetownhallFractured.transform.rotation = other.transform.rotation;
            scoreCounter.GetComponent<ScoreCounter>().score += 50;
        }
        #region OrangeBuildings
        if (other.gameObject.tag == "Orange1building")
        {
            Destroy(other.gameObject);
            Orange1buildingFractured = Instantiate(Orange1buildingFracturedPre, other.transform.position, Quaternion.identity);
            Orange1buildingFractured.transform.localScale = other.gameObject.transform.localScale;
            Orange1buildingFractured.transform.rotation = other.transform.rotation;
            scoreCounter.GetComponent<ScoreCounter>().score += 20;
        }
        if (other.gameObject.tag == "Orange2building")
        {
            Destroy(other.gameObject);
            Orange2buildingFractured = Instantiate(Orange2buildingFracturedPre, other.transform.position, Quaternion.identity);
            Orange2buildingFractured.transform.localScale = other.gameObject.transform.localScale;
            Orange2buildingFractured.transform.rotation = other.transform.rotation;
            scoreCounter.GetComponent<ScoreCounter>().score += 10;
        }
        if (other.gameObject.tag == "Orange3building")
        {
            Destroy(other.gameObject);
            Orange3buildingFractured = Instantiate(Orange3buildingFracturedPre, other.transform.position, Quaternion.identity);
            Orange3buildingFractured.transform.localScale = other.gameObject.transform.localScale;
            Orange3buildingFractured.transform.rotation = other.transform.rotation;
            scoreCounter.GetComponent<ScoreCounter>().score += 50;
        }
        if (other.gameObject.tag == "Orangetownhall")
        {
            Destroy(other.gameObject);
            OrangetownhallFractured = Instantiate(OrangetownhallFracturedPre, other.transform.position, Quaternion.identity);
            OrangetownhallFractured.transform.localScale = other.gameObject.transform.localScale;
            OrangetownhallFractured.transform.rotation = other.transform.rotation;
            scoreCounter.GetComponent<ScoreCounter>().score += 100;
        }
        #endregion
        #region fiskiye
        if (other.gameObject.tag == "Fiskiye")
        {
            Destroy(other.gameObject);
            FiskiyeFractured = Instantiate(FiskiyeFracturedPre, other.transform.position, Quaternion.identity);
            FiskiyeFractured.transform.localScale = other.gameObject.transform.localScale / 10;
            scoreCounter.GetComponent<ScoreCounter>().score += 80;
        }
        #endregion
        #region PurpleBuildings
        if (other.gameObject.tag == "Purple1building")
        {
            Destroy(other.gameObject);
            Purple1buildingFractured = Instantiate(Purple1buildingFracturedPre, other.transform.position, Quaternion.identity);
            Purple1buildingFractured.transform.localScale = other.gameObject.transform.localScale;
            Purple1buildingFractured.transform.rotation = other.transform.rotation;
            scoreCounter.GetComponent<ScoreCounter>().score += 40;
        }
        if (other.gameObject.tag == "Purple2building")
        {
            Destroy(other.gameObject);
            Purple2buildingFractured = Instantiate(Purple2buildingFracturedPre, other.transform.position, Quaternion.identity);
            Purple2buildingFractured.transform.localScale = other.gameObject.transform.localScale;
            Purple2buildingFractured.transform.rotation = other.transform.rotation;
            scoreCounter.GetComponent<ScoreCounter>().score += 20;
        }
        if (other.gameObject.tag == "Purple3building")
        {
            Destroy(other.gameObject);
            Purple3buildingFractured = Instantiate(Purple3buildingFracturedPre, other.transform.position, Quaternion.identity);
            Purple3buildingFractured.transform.localScale = other.gameObject.transform.localScale;
            Purple3buildingFractured.transform.rotation = other.transform.rotation;
            scoreCounter.GetComponent<ScoreCounter>().score += 100;
        }
        if (other.gameObject.tag == "Purpletownhall")
        {
            Destroy(other.gameObject);
            PurpletownhallFractured = Instantiate(PurpletownhallFracturedPre, other.transform.position, Quaternion.identity);
            PurpletownhallFractured.transform.localScale = other.gameObject.transform.localScale;
            PurpletownhallFractured.transform.rotation = other.transform.rotation;
            scoreCounter.GetComponent<ScoreCounter>().score += 200;
        }
        #endregion
        #region BlueBuildings
        if (other.gameObject.tag == "Blue1building")
        {
            Destroy(other.gameObject);
            Blue1buildingFractured = Instantiate(Blue1buildingFracturedPre, other.transform.position, Quaternion.identity);
            Blue1buildingFractured.transform.localScale = other.gameObject.transform.localScale;
            Blue1buildingFractured.transform.rotation = other.transform.rotation;
            scoreCounter.GetComponent<ScoreCounter>().score += 80;
        }
        if (other.gameObject.tag == "Blue2building")
        {
            Destroy(other.gameObject);
            Blue2buildingFractured = Instantiate(Blue2buildingFracturedPre, other.transform.position, Quaternion.identity);
            Blue2buildingFractured.transform.localScale = other.gameObject.transform.localScale;
            Blue2buildingFractured.transform.rotation = other.transform.rotation;
            scoreCounter.GetComponent<ScoreCounter>().score += 40;
        }
        if (other.gameObject.tag == "Blue3building")
        {
            Destroy(other.gameObject);
            Blue3buildingFractured = Instantiate(Blue3buildingFracturedPre, other.transform.position, Quaternion.identity);
            Blue3buildingFractured.transform.localScale = other.gameObject.transform.localScale;
            Blue3buildingFractured.transform.rotation = other.transform.rotation;
            scoreCounter.GetComponent<ScoreCounter>().score += 200;
        }
        if (other.gameObject.tag == "Bluetownhall")
        {
            Destroy(other.gameObject);
            BluetownhallFractured = Instantiate(BluetownhallFracturedPre, other.transform.position, Quaternion.identity);
            BluetownhallFractured.transform.localScale = other.gameObject.transform.localScale;
            BluetownhallFractured.transform.rotation = other.transform.rotation;
            scoreCounter.GetComponent<ScoreCounter>().score += 400;
        }
    }
    #endregion
  
}
