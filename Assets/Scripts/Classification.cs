using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Unity.Barracuda;
using TMPro;
using Vuforia;

public class Classification : MonoBehaviour
{
    const int IMAGE_SIZE = 224;
    const string INPUT_NAME = "images";
    const string OUTPUT_NAME = "Softmax";

  
    public NNModel onnxFile;
    public TextAsset labelFile;

    
    public CameraView cameraView;
    public Preprocess preprocess;
   public TextMeshProUGUI uiText;

    private bool bookDetected = false;
    private bool glassesDetected = false;
    private bool phoneDetected = false;
    private bool laptopDetected = false;
    private bool stopNN = false;
    private bool scan = true;

    private float timer = 3f;
    public GameObject continueButton, scanInstructions, continueInstructions, bacFound, scanning;
    
    //public TextMeshProUGUI tester, tester2, tester3;

    string[] labels;
    IWorker worker;
    private Camera cam;
    private int frames = 80;
    private int frames_scanning = 85;
    void Start()
    {
        
       // VuforiaRuntime.Instance.Deinit();
       // tester3.text = "Start After vuf deinit";
        // cam = Camera.main;
        var model = ModelLoader.Load(onnxFile);
       // if(model == null) tester3.text = "Start After model null";
        worker = WorkerFactory.CreateWorker(WorkerFactory.Type.ComputePrecompiled, model);
        
        LoadLabels();
    }

    void LoadLabels()
    {
        //get only items in quotes
        var stringArray = labelFile.text.Split('"').Where((item, index) => index % 2 != 0);
        //get every other item
        labels = stringArray.Where((x, i) => i % 2 != 0).ToArray();
       // tester3.text = labels.Length.ToString();
    }

    void Update()
    {
       // if (cam.GetComponent<VuforiaBehaviour>().enabled == true) uiText.text = "Vuforia is active";
        WebCamTexture webCamTexture = cameraView.GetCamImage();

       // if (webCamTexture == null) tester.text = "Image is null";

        if (Time.frameCount % frames == 0 && webCamTexture.didUpdateThisFrame && webCamTexture.width > 100 && stopNN==false && scan)
        {
            //  tester.text = "if before scale entered";
            // if (worker == null) tester.text = "worker is null";
            // uiText.text = "Object is being scanned...";
            // scanInstructions.GetComponentInChildren<TextMeshProUGUI>().text = "Checking For Bacteria...";
            //uiText.text ="Entered: " +Time.frameCount.ToString();
            scanning.SetActive(true);
           // scanning.GetComponentInChildren<TextMeshProUGUI>().text = "Object scanning...";
            preprocess.ScaleAndCropImage(webCamTexture, IMAGE_SIZE, RunModel);

           
            // scanning.SetActive(false);
        }
        if (!scan)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                scanning.SetActive(false);
                timer = 3f;
                scan = true;
            }
        }


            if (bookDetected && glassesDetected && laptopDetected && phoneDetected)
        {
            scanning.SetActive(false);
            scanInstructions.SetActive(false);
            continueInstructions.SetActive(true);

            stopNN = true;
            continueButton.SetActive(true);

        }
    }

    void RunModel(byte[] pixels)
    {
        //tester2.text = "run model entered";
        StartCoroutine(RunModelRoutine(pixels));
    }

    IEnumerator RunModelRoutine(byte[] pixels)
    {
        scanInstructions.GetComponentInChildren<TextMeshProUGUI>().text = "Object scanning...";
        //scanning.GetComponentInChildren<TextMeshProUGUI>().text = "Object scanning...";
        //scanning.SetActive(true);
        // bacFound.SetActive(false);
        //tester2.text = "ienumerator entered";
        Tensor tensor = TransformInput(pixels);
       // tester2.text = "transform exited";
        var inputs = new Dictionary<string, Tensor> {
            { INPUT_NAME, tensor }
        };
       // tester2.text = "dic created";
        worker.Execute(inputs);
       // tester2.text = "worker executed";
        Tensor outputTensor = worker.PeekOutput(OUTPUT_NAME);
       // tester2.text = "peeked output";
        // yield return new WaitForCompletion(outputTensor);

        var indexWithHighestProbability = outputTensor.ArgMax()[0];
       // tester2.text = indexWithHighestProbability.ToString();
        //get largest output
        // List<float> temp = outputTensor.ToReadOnlyArray().ToList();
        // float max = temp.Max();
        // int index = temp.IndexOf(max);

        //set UI text
       // uiText.text = labels[indexWithHighestProbability];
        if (labels[indexWithHighestProbability] == "iPod"
            || labels[indexWithHighestProbability] == "cellular telephone, cellular phone, cellphone, cell, mobile phone") { phoneDetected = true;
            
            scanning.GetComponentInChildren<TextMeshProUGUI>().text = "Bacteria Found!";
            
        }
        else if (labels[indexWithHighestProbability] == "carton" || labels[indexWithHighestProbability] == "bookcase") { bookDetected = true;
            scanning.GetComponentInChildren<TextMeshProUGUI>().text = "Bacteria Found!";
            
        }
       else if (labels[indexWithHighestProbability] == "laptop, laptop computer"
            || labels[indexWithHighestProbability] == "notebook, notebook computer") {
            laptopDetected = true;
            scanning.GetComponentInChildren<TextMeshProUGUI>().text = "Bacteria Found!";
           
        }
       else if (labels[indexWithHighestProbability] == "sunglasses, dark glasses, shades") {
            glassesDetected = true;
            scanning.GetComponentInChildren<TextMeshProUGUI>().text = "Bacteria Found!";
          
        } else
        {
            scanning.GetComponentInChildren<TextMeshProUGUI>().text = "No Bacteria Found!";
        
        }

        scan = false;
        //dispose tensors
        tensor.Dispose();
        outputTensor.Dispose();
        // scanning.SetActive(false);
        //uiText.text ="Exited: " +Time.frameCount.ToString();
        yield return null;
    }

    //transform from 0-255 to -1 to 1
    Tensor TransformInput(byte[] pixels)
    {
       // tester2.text = "transform entered";
        float[] transformedPixels = new float[pixels.Length];

        for (int i = 0; i < pixels.Length; i++)
        {
            transformedPixels[i] = (pixels[i] - 127f) / 128f;
            //Debug.Log(transformedPixels[i]);
        }
        return new Tensor(1, IMAGE_SIZE, IMAGE_SIZE, 3, transformedPixels);
    }

    void OnDisable()
    {
        worker.Dispose();
    }
}
