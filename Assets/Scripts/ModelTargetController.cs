using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Vuforia;

public class ModelTargetController : MonoBehaviour
{


    public GameObject modelTargetIphone, modelTragetCup, modelTragetBook, modelTargetLaptop, modelTargetGlasses;
    // Start is called before the first frame update
 
       private ObjectTracker objTracker;
    private DataSet currentDataSet;

    public void LoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void LoadNewTargetModel(string dataBaseName)
    {
        TrackerManager tm = (TrackerManager)TrackerManager.Instance;
        objTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
        
        if (objTracker == null) return;
        currentDataSet = objTracker.GetActiveDataSets().FirstOrDefault();
       // Debug.Log(currentDataSet);
        //if (currentDataSet != null)
        //{
            objTracker.Stop();
            objTracker.DeactivateDataSet(currentDataSet);
        //}
        /*
        if (dataBaseName == "CoffeeCup") {
            modelTargetIphone.SetActive(false);
            modelTragetCup.SetActive(true); }
        if (dataBaseName == "Laptop") {
            modelTragetBook.SetActive(false);
            modelTargetLaptop.SetActive(true); }
        if (dataBaseName == "IPhoneX") {
            modelTargetGlasses.SetActive(false);
            modelTargetIphone.SetActive(true); }
        if (dataBaseName == "Book") {
            modelTragetCup.SetActive(false);
            modelTragetBook.SetActive(true); }*/
        currentDataSet = objTracker.CreateDataSet();
            currentDataSet.Load(dataBaseName);
            objTracker.ActivateDataSet(currentDataSet);
            objTracker.Start();
        

    }

    public void TESTSETACTIVE(string dataBaseName)
    {
        if (dataBaseName == "CoffeeCup")
        {
            modelTargetIphone.SetActive(false);
            modelTragetCup.SetActive(true);
            TrackerManager tm = (TrackerManager)TrackerManager.Instance;
            objTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
            objTracker.Start();
        }
        if (dataBaseName == "Laptop")
        {
            modelTragetBook.SetActive(false);
            modelTargetLaptop.SetActive(true);
            TrackerManager tm = (TrackerManager)TrackerManager.Instance;
            objTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
            objTracker.Start();
        }
        if (dataBaseName == "IPhoneX")
        {
            modelTargetGlasses.SetActive(false);
            modelTargetIphone.SetActive(true);
            TrackerManager tm = (TrackerManager)TrackerManager.Instance;
            objTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
            objTracker.Start();
        }
        if (dataBaseName == "Book")
        {
            modelTragetCup.SetActive(false);
            modelTragetBook.SetActive(true);
            TrackerManager tm = (TrackerManager)TrackerManager.Instance;
            objTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
            objTracker.Start();
        }
        
    }

    public void DeactivateObjectTracker()
    {
        TrackerManager tm = (TrackerManager)TrackerManager.Instance;
        objTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();

        if (objTracker == null) return;
        currentDataSet = objTracker.GetActiveDataSets().FirstOrDefault();
        // Debug.Log(currentDataSet);
        //if (currentDataSet != null)
        //{
        objTracker.Stop();
        objTracker.DeactivateDataSet(currentDataSet);
        modelTargetIphone.SetActive(true);
    }
    public void SwapActiveDatasets(string datasetToActivate)
    {
        // ObjectTracker tracks targets contained in a DataSet and provides methods for creating and (de)activating datasets.
        ObjectTracker objectTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
        IEnumerable<DataSet> datasets = objectTracker.GetDataSets();

        IEnumerable<DataSet> activeDataSets = objectTracker.GetActiveDataSets();
        List<DataSet> activeDataSetsToBeRemoved = activeDataSets.ToList();

        // 1. Loop through all the active datasets and deactivate them.
        foreach (DataSet ads in activeDataSetsToBeRemoved)
        {
            objectTracker.DeactivateDataSet(ads);
        }

        // Swapping of the datasets should NOT be done while the ObjectTracker is running.
        // 2. So, Stop the tracker first.
        objectTracker.Stop();

        // 3. Then, look up the new dataset and if one exists, activate it.
        foreach (DataSet ds in datasets)
        {
            if (ds.Path.Contains(datasetToActivate))
            {
                objectTracker.ActivateDataSet(ds);
            }
        }

        // 4. Loop through the trackable behaviours and set the GuideView.
        IEnumerable<TrackableBehaviour> tbs = TrackerManager.Instance.GetStateManager().GetTrackableBehaviours();
        foreach (TrackableBehaviour tb in tbs)
        {
            if (tb is ModelTargetBehaviour && tb.isActiveAndEnabled)
            {
                Debug.Log("TrackableName: " + tb.TrackableName);
                (tb as ModelTargetBehaviour).GuideViewMode = ModelTargetBehaviour.GuideViewDisplayMode.GuideView2D;
            }

        }

        // 5. Finally, restart the object tracker.
        objectTracker.Start();
    }
}
