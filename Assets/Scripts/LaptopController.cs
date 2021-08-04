using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Vuforia;

public class LaptopController : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform spawnPosKeyboard, spawnPosScreen;
    public GameObject planeScreen, planeKeyboard, instructions, waitForText, continueText, continueButton, sniper, spray, tester, sprayButton, killedBacsText;
    public GameObject[] backteriaPrefabs, instatiatedFirstlyBacs;
    public int prefabsToSpawnPublic;
    private float speed = 0.2f;
    private List<GameObject> spawnedObjs = new List<GameObject>();
    private int bacsToKill=20;
    public ParticleSystem sprayParticle;

    private List<float> disBacToTouch = new List<float>();

    private Dictionary<float, GameObject> dictObjDistance = new Dictionary<float, GameObject>();

    private Camera cam;
    public Camera camSpray;
    private bool waitForFirst = false;
    private bool wellDone = false;

    private float waitFor = 10f;
    private float remTimeEndGame = 2f;
    private bool getSpawnedObj = false;



    void Start()
    {
        cam = Camera.main;
        for (int i = 0; i < instatiatedFirstlyBacs.Length; i++)
        {
            spawnedObjs.Add(instatiatedFirstlyBacs[i]);

        }
    }

    // Update is called once per frame
    void Update()
    {

        StateManager sm = TrackerManager.Instance.GetStateManager();

        IEnumerable<TrackableBehaviour> activeTrackables = sm.GetActiveTrackableBehaviours();
        
        if (!(activeTrackables?.Any()).GetValueOrDefault() == false) // checks whether the object is detected
        {
            if (!waitForText.activeInHierarchy &&
           !continueText.activeInHierarchy )
                instructions.SetActive(true);

            if (Input.touchCount > 0 && (Input.touches[0].phase == TouchPhase.Began || Input.touches[0].phase == TouchPhase.Moved || Input.touches[0].phase == TouchPhase.Stationary) && bacsToKill>0)
            {

                Ray ray = cam.ScreenPointToRay(Input.touches[0].position);

                RaycastHit hit;


                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.tag == "Plane" || hit.collider.tag == "PlaneScreen")
                    {


                        waitForFirst = true;
                        instructions.SetActive(false);
                        waitForText.SetActive(true);
                        waitForText.GetComponentInChildren<TextMeshProUGUI>().text = "Wait for: " + Mathf.FloorToInt(waitFor).ToString() + "s";
                        waitFor -= Time.deltaTime;
                        if (waitFor < 0) //after 10s wait
                        {
                            // wellDone = true;
                            waitForText.SetActive(false);
                            continueText.SetActive(true);
                            // continueButton.SetActive(true);
                            //kill bac
                            sniper.SetActive(true);
                            spray.SetActive(true);
                            sprayButton.SetActive(true);
                            if (getSpawnedObj == false)
                            {
                                bacsToKill = spawnedObjs.Count;
                                getSpawnedObj = true;

                            }


                        }
                        else //10s bot over
                        {
                            disBacToTouch = new List<float>();
                            dictObjDistance = new Dictionary<float, GameObject>();

                            if (spawnedObjs.Count < prefabsToSpawnPublic)
                            {

                                for (int x = 0; x < prefabsToSpawnPublic - spawnedObjs.Count; ++x)
                                {
                                    // get a random one
                                    GameObject randomPrefab = backteriaPrefabs[Random.Range(0, backteriaPrefabs.Length)];

                                    // instantiate it
                                    GameObject obj = Instantiate(randomPrefab, spawnPosKeyboard.position, randomPrefab.transform.rotation);

                                    // tester.GetComponentInChildren<TextMeshProUGUI>().text = "x: " + obj.transform.position.x.ToString() + ", y: " + obj.transform.position.y.ToString() + ", z: " + obj.transform.position.z.ToString();
                                    spawnedObjs.Add(obj);

                                    // set its position to a random point on the mesh
                                    RandomMove(obj, hit.collider.tag);

                                }
                            }

                            for (int i = 0; i < spawnedObjs.Count; i++)
                            {

                                float distToTouch = Vector3.Distance(spawnedObjs[i].transform.position, hit.point);
                                disBacToTouch.Add(distToTouch);
                                dictObjDistance[distToTouch] = spawnedObjs[i];



                            }
                            float step = speed * Time.deltaTime;
                            disBacToTouch.Sort((p1, p2) => p1.CompareTo(p2));
                            dictObjDistance[disBacToTouch[0]].transform.position = Vector3.MoveTowards(dictObjDistance[disBacToTouch[0]].transform.position, hit.point, step);
                            if (Vector3.Distance(dictObjDistance[disBacToTouch[0]].transform.position, hit.point) <= 0.01f)
                            {
                                spawnedObjs.Remove(dictObjDistance[disBacToTouch[0]]);

                                Destroy(dictObjDistance[disBacToTouch[0]]);
                            }
                        }//10s if

                    } // plane if
                }//raycast if
            }//touch if
            else if (waitForFirst == true && spray.activeInHierarchy == false)
            {
                    waitForText.SetActive(false);
                    instructions.SetActive(true);
            }



            else if (bacsToKill <= 0)
            { //continueText.SetActive(false);
               // killedBacsText.SetActive(true);
                remTimeEndGame -= Time.deltaTime;
                //tester.GetComponentInChildren<TextMeshProUGUI>().text = remTimeEndGame.ToString();
                if (remTimeEndGame < 0)
                {

                    SceneManager.LoadScene("GameOver");

                }

            }

        } // end if marker aligned


    }//end Update


    public void RandomMove(GameObject obj, string planeTag)
    {
        float x = 0;
        float y = 0;
        float z = 0;

        if (planeTag == "Plane")
        {
            
            Vector3 min = planeKeyboard.GetComponent<Renderer>().bounds.min;
            Vector3 max = planeKeyboard.GetComponent<Renderer>().bounds.max;
            x = Random.Range(min.x, max.x);
            z = Random.Range(min.z, max.z);
            obj.transform.position = new Vector3(x, spawnPosKeyboard.position.y, z);

        }
        else if (planeTag == "PlaneScreen") {
            Vector3 min = planeScreen.GetComponent<Renderer>().bounds.min;
            Vector3 max = planeScreen.GetComponent<Renderer>().bounds.max;
           x = Random.Range(min.x, max.x);
            y = Random.Range(min.y, max.y);
            obj.transform.position = new Vector3(x, y, spawnPosScreen.position.z);
        }

       

       


       

    }

    public void Spray()
    {
        //tester.GetComponentInChildren<TextMeshProUGUI>().text = "SPRAY ENTERED";
        spray.GetComponent<Animator>().Play("SprayAnim");

        sprayParticle.Play();
        // sprayParticle.GetComponent<Animator>().Play("MoveSpray");
        RaycastHit hit;
        if (Physics.Raycast(camSpray.transform.position, camSpray.transform.forward, out hit))
        {
            if (hit.collider.tag == "Bacteria")
            {
                //sprayedBac = hit.collider.gameObject;
                Destroy(hit.collider.gameObject, 1f);
                // hit.collider.gameObject.SetActive(false);
                bacsToKill--;

            }
        }
    }

    
}
