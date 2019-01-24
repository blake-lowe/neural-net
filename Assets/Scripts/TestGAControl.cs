using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGAControl : MonoBehaviour {

    public int testIndividualGeneSize;

    public int populationSize;          //total number of individuals
    public int numParents;              //number of parents per reproduction set
    public int numCrossoverPoints;      //number of crossover points in genetic combination. Will be used in GeneticIndividual.Reproduce(). should be x >= numParents.
    public float mutationChance;        //from 0 to 1. measure of genetic diversity. 0 is no mutation. 1 will cause an infinite loop after changing every weight.
    public float environmentalPressure; //from 0 to 1. 1 is no survivors
    public float eliteFraction;         //number of solutions to save from one generation to the next 0 is none 1 is all saved. (should never be > 1-environmentalPressure then errors) (if > like .2 then GA won't work well)
    public int tournamentSize;          //size of the randomly chosen subset from which the most fit individual will be chosen for reproduction.  must be 1 <= x <= populationSize.
    public int numGenerationsPerSecond;          //how much training to do

    private GeneticAlgorithm ga;
    private float nextGenerationTime;

    public AnimationCurve plotBest = new AnimationCurve();
    public AnimationCurve plotWorst = new AnimationCurve();

    // Use this for initialization
    void Start () {
        TestGeneticIndividual progenitor = new TestGeneticIndividual(testIndividualGeneSize);
        ga = new GeneticAlgorithm(progenitor, populationSize, numParents, environmentalPressure, eliteFraction, numCrossoverPoints, mutationChance, tournamentSize);
	}
	
	// Update is called once per frame
	void Update () {
        float secondsPerGeneration = 1 / numGenerationsPerSecond;
        if (Time.time > nextGenerationTime)
        {
            if (ga != null)
            {
                ga.TrainGeneration(1);
                TestGeneticIndividual bestIndividual = (TestGeneticIndividual)ga.individuals[0];
                TestGeneticIndividual worstIndividual = (TestGeneticIndividual)ga.individuals[populationSize-1];
                //debug
                plotBest.AddKey(Time.realtimeSinceStartup, (float)bestIndividual.Fitness());
                plotWorst.AddKey(Time.realtimeSinceStartup, (float)worstIndividual.Fitness());

                nextGenerationTime += secondsPerGeneration;
            }
        }
    }
}
