using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Vuforia;

public class GlassesController : MonoBehaviour
{
    public GameObject instructions, waitForText, tester, continueText, continueButton;

    private Camera cam;

    private float waitFor = 5f;

    private bool waitForFirst = false;
    private bool wellDone = false;

    //private bool tracked = false;
    // Start is called before the first frame update
    private void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        // Get the Vuforia StateManager
        StateManager sm = TrackerManager.Instance.GetStateManager();

        // Query the StateManager to retrieve the list of
        // currently 'active' trackables 
        //(i.e. the ones currently being tracked by Vuforia)
        IEnumerable<TrackableBehaviour> activeTrackables = sm.GetActiveTrackableBehaviours();
        if (!(activeTrackables?.Any()).GetValueOrDefault() == false) //glasses are being tracked tracked == true || !(activeTrackables?.Any()).GetValueOrDefault() == false 
        {
            //tracked = true;
            if (!waitForText.activeInHierarchy &&
           !continueText.activeInHierarchy)
            {
                instructions.SetActive(true);
                instructions.GetComponentInChildren<TextMeshProUGUI>().text = "Place Your Finger Among The Bacteria";
            }


            if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began || Input.touches[0].phase == TouchPhase.Moved || Input.touches[0].phase == TouchPhase.Stationary)
            {
                Ray ray = cam.ScreenPointToRay(Input.touches[0].position);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {

              
                    tester.GetComponentInChildren<TextMeshProUGUI>().text = "If raycast entered";
                    if (hit.collider.tag == "Glasses")
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
                    }
                    /*else
                    {
                        tester.GetComponentInChildren<TextMeshProUGUI>().text = "If collider exited";
                    }*/
                }
               /* else
                {
                    tester.GetComponentInChildren<TextMeshProUGUI>().text = "If raycast exited";
                }
                */

            }
            else
            {
                if (waitForFirst == true && wellDone == false)
                {
                    waitForText.SetActive(false);
                    instructions.SetActive(true);
                    instructions.GetComponentInChildren<TextMeshProUGUI>().text = "Place Your Finger Among The Bacteria";
                }
            }


        } else
        {
            instructions.SetActive(true);
            instructions.GetComponentInChildren<TextMeshProUGUI>().text = "Align The Marker With Your Glasses";
        }
       
       
    }
    public void LoadScene()
    {
        SceneManager.LoadScene("Phone");
    }
}
