using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Vuforia;

public class BookController : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform testerPos;
    public GameObject plane, instructions, waitForText, continueText, continueButton;
    public GameObject[] backteriaPrefabs, instatiatedFirstlyBacs;
    public int prefabsToSpawnPublic;
    private float speed = 0.06f;
    private List<GameObject> spawnedObjs = new List<GameObject>();
   
    private List<float> disBacToTouch = new List<float>();

    private Dictionary<float, GameObject> dictObjDistance = new Dictionary<float, GameObject>();

    private Camera cam;

    private bool waitForFirst = false;
    private bool wellDone = false;

    private float waitFor = 10f;



    void Start()
    {
        cam = Camera.main;
        for (int i = 0; i < instatiatedFirstlyBacs.Length; i++) {
            spawnedObjs.Add(instatiatedFirstlyBacs[i]);
          
        }
    }

    // Update is called once per frame
    void Update()
    {

        StateManager sm = TrackerManager.Instance.GetStateManager();

        IEnumerable<TrackableBehaviour> activeTrackables = sm.GetActiveTrackableBehaviours();
        if (!(activeTrackables?.Any()).GetValueOrDefault() == false) 
        {
            if (!waitForText.activeInHierarchy &&
           !continueText.activeInHierarchy)
                instructions.SetActive(true);
            
            if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began || Input.touches[0].phase == TouchPhase.Moved || Input.touches[0].phase == TouchPhase.Stationary)
            {

                Ray ray = cam.ScreenPointToRay(Input.touches[0].position);

                RaycastHit hit;

               
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.tag == "Plane" || hit.collider.tag == "Book")
                    {


                        waitForFirst = true;
                        instructions.SetActive(false);
                        waitForText.SetActive(true);
                        waitForText.GetComponentInChildren<TextMeshProUGUI>().text = "Wait for: " + Mathf.FloorToInt(waitFor).ToString() + "s";
                        waitFor -= Time.deltaTime;
                        if (waitFor < 0)
                        {
                            wellDone = true;
                            waitForText.SetActive(false);
                            continueText.SetActive(true);
                            continueButton.SetActive(true);


                        }
                        else
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
                                    GameObject obj = Instantiate(randomPrefab, testerPos.position, randomPrefab.transform.rotation);

                                    // tester.GetComponentInChildren<TextMeshProUGUI>().text = "x: " + obj.transform.position.x.ToString() + ", y: " + obj.transform.position.y.ToString() + ", z: " + obj.transform.position.z.ToString();
                                    spawnedObjs.Add(obj);

                                    // set its position to a random point on the mesh
                                    RandomMove(obj);

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
                        }

                    }
                }
            }
            else
            {
                if (waitForFirst == true && wellDone == false)
                {
                    waitForText.SetActive(false);
                    instructions.SetActive(true);
                }
            }
        }
    }//end Update


    public void RandomMove(GameObject obj)
    {
        
        Vector3 min = plane.GetComponent<Renderer>().bounds.min;
        Vector3 max = plane.GetComponent<Renderer>().bounds.max;
       
        float y = 0;
        float z = 0;
     
        y = Random.Range(min.y, max.y);
             z = Random.Range(min.z, max.z);

        
        obj.transform.position = new Vector3(testerPos.transform.position.x, y, z);
     
    }

    public void LoadScene()
    {
        SceneManager.LoadScene("Laptop");
    }
}
