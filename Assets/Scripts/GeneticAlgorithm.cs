using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticAlgorithm {//a general class describing a Genetic Algorithm. depending on which type of IGeneticIndividual is used, it will train a different class

    public int populationSize;          //total number of individuals
    public int numParents;              //number of parents per reproduction set
    public float environmentalPressure; //from 0 to 1. 1 is no survivors
    private int numChildrenNeeded;   //filled by constructor
    public float eliteFraction;           //number of solutions to save from one generation to the next 0 is none 1 is all saved. (should never be > 1-environmentalPressure then errors) (if > like .2 then GA won't work well)
    public int numToSave;            //filled by constructor 
    private int numReproductionPairs;//filled by constructor
    private IGeneticIndividual[] individuals;
    // Use this for initialization
    public GeneticAlgorithm(IGeneticIndividual progenitor, int populationSize, int numParents, float environmentalPressure)
    {
        this.populationSize = populationSize;
        this.numParents = numParents;
        this.environmentalPressure = environmentalPressure;
        numToSave = (int)(eliteFraction * populationSize);
        numChildrenNeeded = populationSize - numToSave;//each child will have its own set of parents

        individuals = new IGeneticIndividual[populationSize];
        Initialize(progenitor);
    }

    private void Initialize(IGeneticIndividual progenitor)
    {
        for (int i = 0; i < individuals.Length; i++)
        {
            individuals[i] = progenitor;
            individuals[i].Randomize();
        }
    }

    private void FitnessSort()//call after any editing of population
    {
        Array.Sort(individuals);//will sort individuals based on fitness because GeneticIndividual and classes which implement GeneticIndividual impelements CompareTo and IComparable
    }

    private IGeneticIndividual TrainGeneration(int numGenerations)
    {
        for (int i = 0; i < numGenerations; i++)
        {
            //create elite array
            IGeneticIndividual[] eliteIndividuals = new IGeneticIndividual[numToSave];
            for (int j = 0; j < numToSave; j++)
            {
                eliteIndividuals[j] = individuals[j];
            }
            //create 2 dim array of parents(1) for each child(0)
            IGeneticIndividual[,] reproductionGroups = new IGeneticIndividual[numChildrenNeeded,numParents];
            ////TODO Fill array
            //generate children
            IGeneticIndividual[] newChidren = new IGeneticIndividual[numChildrenNeeded];
            ////TODO fill
            //mutate over children array
            ////TODO
            //combine elite and children array
            ////TODO
            FitnessSort();
        }
        return individuals[0];//the highest fitness
    }

    private IGeneticIndividual TrainFitness(double targetFitness)
    {
        while(individuals[0].Fitness() <= targetFitness)
        {
            TrainGeneration(1);
        }
        return individuals[0];//the highest fitness
    }
}
