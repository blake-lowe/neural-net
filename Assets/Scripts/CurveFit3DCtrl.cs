using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vectrosity;

public class CurveFit3DCtrl : MonoBehaviour
{

    public int numHiddenLayers; //NN dimensions
    public int hiddenLayerSize; //

    public int populationSize;          //total number of individuals
    public int numParents;              //number of parents per reproduction set
    public int numCrossoverPoints;      //number of crossover points in genetic combination. Will be used in GeneticIndividual.Reproduce(). should be x >= numParents.
    public float mutationChance;        //from 0 to 1. measure of genetic diversity. 0 is no mutation. 1 will cause an infinite loop after changing every weight.
    public float environmentalPressure; //from 0 to 1. 1 is no survivors
    public float eliteFraction;         //number of solutions to save from one generation to the next 0 is none 1 is all saved. (should never be > 1-environmentalPressure then errors) (if > like .2 then GA won't work well)
    public int tournamentSize;          //size of the randomly chosen subset from which the most fit individual will be chosen for reproduction.  must be 1 <= x <= populationSize.

    private GeneticAlgorithm ga;

    private bool isGAInitialized = false;
    public bool isRunning = false;
    public bool isFitnessControl = false;

    public int targetGeneration = 0;
    public float targetFitness = 0;

    public int numPoints;   //
    public int numTestPoints;   //

    private double[,] testInputSets;
    private double[,] testOutputSets;

    public string function;//name of function to be approximated

    public NeuralNet bestNet;//the highest fitness solution

    public VisualNet visualNet;
    public float vNetXArea;//area devoted to the net visualization
    public float vNetYArea;

    public GameObject configPanel;//fields to be filled by unity editor

    public InputField populationField;
    public InputField numParentsField;
    public InputField numCrossoverPointsField;
    public InputField mutationChanceField;
    public InputField environmentalPressureField;
    public InputField eliteFractionField;
    public InputField tournamentSizeField;

    public InputField numHiddenLayersField;
    public InputField hiddenLayerSizeField;
    public InputField numPointsField;
    public InputField numTestPointsField;
    public Dropdown functionDropdown;//end fields

    public Text functionContent;//hud field

    public Text fitnessContent;

    public GameObject configDoneButton;
    public GameObject configCloseButton;

    public GameObject HUDContainer;

    public InputField directoryField;
    public InputField filenameField;

    public Button pauseButton;
    public Button runButton;

    public Toggle subtractToggle;

    public Text currentGenerationContent;

    public Button generationControlButton;
    public Button fitnessControlButton;

    public InputField targetGenerationField;
    public InputField targetFitnessField;

    private VectorLine[] targetFunctionLinesX;
    private VectorLine[] targetFunctionLinesY;
    private VectorLine[] NNFunctionLinesX;
    private VectorLine[] NNFunctionLinesY;
    public Vector3 functionsOrigin;
    public float functionsScale;

    public InputField NNInputField;
    public Text NNOutputContent;

    private VectorLine fitnessHistoryLine;
    public Vector2 fitnessHistoryOrigin;
    public Vector2 fitnessHistoryScale;
    private float fitnessHistoryXStep;

    private List<float> fitnessRecord = new List<float>();


    // Use this for initialization
    void Start()
    {
        Screen.SetResolution(960, 540, false);
        configPanel.SetActive(true);
        VectorLine.canvas.sortingOrder = -1;
    }

    // Update is called once per frame
    void Update()
    {
        if (isGAInitialized)
        {
            currentGenerationContent.text = ga.generationCount.ToString();
            if (int.Parse(currentGenerationContent.text) > targetGeneration)
            {
                targetGeneration = int.Parse(currentGenerationContent.text);
                targetGenerationField.text = currentGenerationContent.text;
            }
            if (isRunning)
            {
                if (isFitnessControl)//fitness control
                {
                    if (bestNet.Fitness() < targetFitness)
                    {
                        //call training ga
                        bestNet = (NeuralNet)ga.TrainGeneration(1);
                        //update fitness graph
                        fitnessContent.text = bestNet.Fitness().ToString();
                        //TODO
                        //update vnet net
                        visualNet.net = bestNet;
                        //update function curves
                        drawNNFunction();
                        //updateFitnessHistoryGraph
                        drawFitnessHistory();
                    }
                }
                else//generation control
                {
                    if (int.Parse(currentGenerationContent.text) < targetGeneration)
                    {
                        //call training ga
                        bestNet = (NeuralNet)ga.TrainGeneration(1);
                        //update fitness graph
                        fitnessContent.text = bestNet.Fitness().ToString();
                        //TODO
                        //update vnet net
                        visualNet.net = bestNet;
                        //update function curves
                        drawNNFunction();
                        //updateFitnessHistoryGraph
                        drawFitnessHistory();
                    }
                }
            }
        }
    }

    private void setupUI()
    {
        //pull info from input fields
        //ga
        populationSize = int.Parse(populationField.text);
        numParents = int.Parse(numParentsField.text);
        numCrossoverPoints = int.Parse(numCrossoverPointsField.text);
        mutationChance = float.Parse(mutationChanceField.text);
        environmentalPressure = float.Parse(environmentalPressureField.text);
        eliteFraction = float.Parse(eliteFractionField.text);
        tournamentSize = int.Parse(tournamentSizeField.text);
        //nn
        numHiddenLayers = int.Parse(numHiddenLayersField.text);
        hiddenLayerSize = int.Parse(hiddenLayerSizeField.text);
        numPoints = int.Parse(numPointsField.text);
        numTestPoints = int.Parse(numTestPointsField.text);

        switch (functionDropdown.value)//convert dropdown value to function name
        {
            case 0:
                function = "constant";
                break;
            case 1:
                function = "linearX";
                break;
            case 2:
                function = "negativeLinearX";
                break;
            case 3:
                function = "linearY";
                break;
            case 4:
                function = "negativeLinearY";
                break;
            case 5:
                function = "quadraticX";
                break;
            case 6:
                function = "quadraticY";
                break;
            case 7:
                function = "saddle";
                break;
            case 8:
                function = "negativeSaddle";
                break;
            case 9:
                function = "sine";
                break;
            default:
                function = "null";
                break;
        }

        functionContent.text = function;

        //generate test inputs and outputs from correct function and based on numTestPoints
        testInputSets = new double[numTestPoints * numTestPoints, 2];
        testOutputSets = new double[numTestPoints * numTestPoints, 1];
        for (int i = 0; i <= numTestPoints - 1; i++)//x from 0 to 1
        {
            for (int j = 0; j <= numTestPoints - 1; j++)//y from 0 to one
            {
                float x = (float)i / (float)(numTestPoints - 1f);
                float y = (float)j / (float)(numTestPoints - 1f);
                float z = functionEvaluate(x, y);
                //assign to testSets
                testInputSets[i*numTestPoints + j, 0] = x;
                testInputSets[i * numTestPoints + j, 1] = y;
                testOutputSets[i * numTestPoints + j, 0] = z;
            }
        }

        NeuralNet net = new NeuralNet(2, 1, numHiddenLayers, hiddenLayerSize, testInputSets, testOutputSets);//create net with test sets filled
        ga = new GeneticAlgorithm(net, populationSize, numParents, environmentalPressure, eliteFraction, numCrossoverPoints, mutationChance, tournamentSize);
        isGAInitialized = true;
        bestNet = (NeuralNet)ga.individuals[0];
        visualNet.net = bestNet;
        visualNet.layerSeparation = vNetXArea / (numHiddenLayers + 1);
        visualNet.nodeSeparation = vNetYArea / (hiddenLayerSize + 1);
        visualNet.Initialize();

        targetFunctionLinesX = new VectorLine[numPoints];
        targetFunctionLinesY = new VectorLine[numPoints];
        NNFunctionLinesX = new VectorLine[numPoints];
        NNFunctionLinesY = new VectorLine[numPoints];
        for (int i = 0; i < numPoints; i++)
        {
            targetFunctionLinesX[i] = new VectorLine("targetFunctionLine", new List<Vector2>(), 2.0f, LineType.Continuous);
            targetFunctionLinesY[i] = new VectorLine("targetFunctionLine", new List<Vector2>(), 2.0f, LineType.Continuous);
            NNFunctionLinesX[i] = new VectorLine("targetFunctionLine", new List<Vector2>(), 2.0f, LineType.Continuous);
            NNFunctionLinesY[i] = new VectorLine("targetFunctionLine", new List<Vector2>(), 2.0f, LineType.Continuous);
        }

        fitnessHistoryLine = new VectorLine("fitnessHistoryLine", new List<Vector2>(), 2.0f, LineType.Continuous);
        VectorLine fitHistAxesLine = new VectorLine("fitHistAxesLine", new List<Vector2>(), 2.0f, LineType.Continuous);
        fitHistAxesLine.points2.Add(new Vector2(495, 230));
        fitHistAxesLine.points2.Add(new Vector2(495, 25));
        fitHistAxesLine.points2.Add(new Vector2(900, 25));

        drawTargetFunction();
        drawNNFunction();
        drawFitnessHistory();
        fitHistAxesLine.Draw();

        NNInputField.text = "0";
        updateNNOutput();
    }

    public void drawTargetFunction()
    {
        for (int i = 0; i <= numTestPoints - 1; i++)//x from 0 to 1
        {
            for (int j = 0; j <= numTestPoints - 1; j++)//y from 0 to one
            {
                float x = (float)i / (float)(numTestPoints - 1f);
                float y = (float)j / (float)(numTestPoints - 1f);
                float z = functionEvaluate(x, y);
                //add to lines
                targetFunctionLinesX[i].points3.Add(new Vector3(functionsOrigin.x + x * functionsScale, functionsOrigin.z + z * functionsScale, functionsOrigin.y + y * functionsScale));//bc y is up in unity
                targetFunctionLinesY[j].points3.Add(new Vector3(functionsOrigin.x + x * functionsScale, functionsOrigin.z + z * functionsScale, functionsOrigin.y + y * functionsScale));//bc y is up in unity
            }
        }
        for (int i = 0; i < numPoints; i++)
        {
            targetFunctionLinesX[i].Draw();
            targetFunctionLinesX[i].SetColor(Color.blue);
            targetFunctionLinesY[i].Draw();
            targetFunctionLinesY[i].SetColor(Color.blue);
        }
            
    }

    public void drawNNFunction()
    {
        for (int i = 0; i < numPoints; i++)
        {
            NNFunctionLinesX[i].points3.Clear();
            NNFunctionLinesY[i].points3.Clear();
        }


        for (int i = 0; i <= numTestPoints - 1; i++)//x from 0 to 1
        {
            for (int j = 0; j <= numTestPoints - 1; j++)//y from 0 to one
            {
                float x = (float)i / (float)(numTestPoints - 1f);
                float y = (float)j / (float)(numTestPoints - 1f);
                float z = (float)bestNet.FeedForward(new double[] { x, y })[0];
                //add to lines
                NNFunctionLinesX[i].points3.Add(new Vector3(functionsOrigin.x + x * functionsScale, functionsOrigin.z + z * functionsScale, functionsOrigin.y + y * functionsScale));//out of order bc y is up in unity
                NNFunctionLinesY[j].points3.Add(new Vector3(functionsOrigin.x + x * functionsScale, functionsOrigin.z + z * functionsScale, functionsOrigin.y + y * functionsScale));//out of order bc y is up in unity
            }
        }

        for (int i = 0; i < numPoints; i++)
        {
            NNFunctionLinesX[i].Draw();
            NNFunctionLinesX[i].SetColor(Color.red);
            NNFunctionLinesY[i].Draw();
            NNFunctionLinesY[i].SetColor(Color.red);
        }

        updateNNOutput();
    }

    private float functionEvaluate(float x, float y)
    {
        if (function == "constant") { return 0.5f; }
        else if (function == "linearX") { return x; }
        else if (function == "negativeLinearX") { return 1f - x; }
        else if (function == "linearY") { return y; }
        else if (function == "negativeLinearY") { return 1f - y; }
        else if (function == "quadraticX") { return x * x; }
        else if (function == "quadraticY") { return y * y; }
        else if (function == "saddle") { return x * y; }
        else if (function == "negativeSaddle") { return -x * y + 1f; }
        else if (function == "sine") { return 0.5f * (Mathf.Sin(2*Mathf.PI*x) + Mathf.Sin(2 * Mathf.PI * y)) + 0.5f; }
        else
        {
            Debug.Log("Function Evaluate Error");
            return 0f;
        }
    }

    private void drawFitnessHistory()
    {
        fitnessRecord.Add((float)bestNet.Fitness());
        fitnessHistoryLine.points2.Clear();
        fitnessHistoryXStep = fitnessHistoryScale.x / ((float)fitnessRecord.Count - 1);

        for (int i = 0; i < fitnessRecord.Count; i++)
        {
            float newPointX = fitnessHistoryOrigin.x + (float)i * fitnessHistoryXStep;
            float newPointY = fitnessHistoryOrigin.y + (fitnessRecord[i] + bestNet.numOutputs) * fitnessHistoryScale.y;
            fitnessHistoryLine.points2.Add(new Vector2(newPointX, newPointY));
        }

        if (fitnessHistoryLine.points2.Count >= 2)
        {
            fitnessHistoryLine.Draw();
            fitnessHistoryLine.SetColor(Color.green);
        }

    }

    public void configDone()
    {
        setupUI();
        HUDContainer.SetActive(true);
        configPanel.SetActive(false);
    }

    public void configClose()
    {
        configPanel.SetActive(false);
        HUDContainer.SetActive(true);
    }

    public void showDetails()
    {
        //convert fields to be uneditable
        populationField.readOnly = true;
        numParentsField.readOnly = true;
        numCrossoverPointsField.readOnly = true;
        mutationChanceField.readOnly = true;
        environmentalPressureField.readOnly = true;
        eliteFractionField.readOnly = true;
        tournamentSizeField.readOnly = true;
        numHiddenLayersField.readOnly = true;
        hiddenLayerSizeField.readOnly = true;
        numPointsField.readOnly = true;
        numTestPointsField.readOnly = true;
        functionDropdown.interactable = false;

        //hide hud and show config panel
        HUDContainer.SetActive(false);
        configPanel.SetActive(true);
        configCloseButton.SetActive(true);
        configDoneButton.SetActive(false);
    }

    public void saveAsCSV()
    {
        bestNet.WriteToFile(directoryField.text, filenameField.text);
    }

    public void pause()
    {
        isRunning = false;
        pauseButton.interactable = false;
        runButton.interactable = true;
    }

    public void run()
    {
        isRunning = true;
        pauseButton.interactable = true;
        runButton.interactable = false;
    }

    public void enableGenerationControl()
    {
        isFitnessControl = false;
        fitnessControlButton.interactable = true;
        generationControlButton.interactable = false;
        targetFitnessField.interactable = false;
        targetGenerationField.interactable = true;
    }

    public void enableFitnessControl()
    {
        isFitnessControl = true;
        fitnessControlButton.interactable = false;
        generationControlButton.interactable = true;
        targetFitnessField.interactable = true;
        targetGenerationField.interactable = false;
    }

    public void updateTargetGeneration()
    {
        targetGeneration = int.Parse(targetGenerationField.text);
    }

    public void addGenerations1()
    {
        int n;
        if (subtractToggle.isOn) n = -1;
        else n = 1;
        targetGenerationField.text = (int.Parse(targetGenerationField.text) + n).ToString();
        updateTargetGeneration();
    }

    public void addGenerations10()
    {
        int n;
        if (subtractToggle.isOn) n = -10;
        else n = 10;
        targetGenerationField.text = (int.Parse(targetGenerationField.text) + n).ToString();
        updateTargetGeneration();
    }

    public void addGenerations100()
    {
        int n;
        if (subtractToggle.isOn) n = -100;
        else n = 100;
        targetGenerationField.text = (int.Parse(targetGenerationField.text) + n).ToString();
        updateTargetGeneration();
    }

    public void updateTargetFitness()
    {
        targetFitness = float.Parse(targetFitnessField.text);
    }

    public void updateNNOutput()
    {
        float input;
        if (float.TryParse(NNInputField.text, out input))
        {
            input = float.Parse(NNInputField.text);
        }
        else
        {
            NNInputField.text = "0";
            input = 0;
        }
        float output = (float)bestNet.FeedForward(new double[] { input })[0];
        NNOutputContent.text = output.ToString();
    }
}