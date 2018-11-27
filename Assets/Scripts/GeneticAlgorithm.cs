using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticAlgorithm {

    public int populationSize;
    public int numParents;
    public float environmentalPressure;//from 0 to 1. 1 is no survivors
    public int numToKill;

    private IGeneticIndividual[] individuals;
    // Use this for initialization
    public GeneticAlgorithm(int populationSize, int numParents, float environmentalPressure)
    {
        this.populationSize = populationSize;
        this.numParents = numParents;
        this.environmentalPressure = environmentalPressure;
        numToKill = (int)(environmentalPressure * populationSize);


        individuals = new IGeneticIndividual[populationSize];
        Initialize();
    }

    private void Initialize()
    {
        for (int i = 0; i < individuals.Length; i++)
        {
            individuals[i] = individuals[i].Randomize();
        }
    }

    private void FitnessSort()
    {
        Array.Sort(individuals);//will sort individuals based on fitness because GeneticIndividual and classes which implement GeneticIndividual impelements CompareTo and IComparable
    }

    private IGeneticIndividual TrainGeneration(int numGenerations)
    {
        for (int i = 0; i < numGenerations; i++)
        {
            //one iteration
        }
        return individuals[0];//the highest fitness
    }

    private IGeneticIndividual TrainFitness(double targetFitness)
    {
        return individuals[0];//the highest fitness
    }
}
