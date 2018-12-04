using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticAlgorithm {//a general class describing a Genetic Algorithm. depending on which type of IGeneticIndividual is used, it will train a different class

    public int populationSize;          //total number of individuals
    public int numParents;              //number of parents per reproduction set
    public float environmentalPressure; //from 0 to 1. 1 is no survivors
    private int numToKill;           //filled by constructor
    public int numToSave;

    private IGeneticIndividual[] individuals;
    // Use this for initialization
    public GeneticAlgorithm(IGeneticIndividual progenitor, int populationSize, int numParents, float environmentalPressure)
    {
        this.populationSize = populationSize;
        this.numParents = numParents;
        this.environmentalPressure = environmentalPressure;
        numToKill = (int)(environmentalPressure * populationSize);


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

    private void FitnessSort()
    {
        Array.Sort(individuals);//will sort individuals based on fitness because GeneticIndividual and classes which implement GeneticIndividual impelements CompareTo and IComparable
    }

    private IGeneticIndividual TrainGeneration(int numGenerations)
    {
        for (int i = 0; i < numGenerations; i++)
        {
            //one iteration TODO
        }
        return individuals[0];//the highest fitness
    }

    private IGeneticIndividual TrainFitness(double targetFitness)
    {
        //todo
        return individuals[0];//the highest fitness
    }
}
