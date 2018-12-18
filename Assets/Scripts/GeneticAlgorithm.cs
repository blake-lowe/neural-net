using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticAlgorithm {//a general class describing a Genetic Algorithm. depending on which type of IGeneticIndividual is used, it will train a different class. Algorithm uses tournament selection and elitism.

    public int populationSize;          //total number of individuals
    public int numParents;              //number of parents per reproduction set
    public int numCrossoverPoints;      //number of crossover points in genetic combination. Will be used in GeneticIndividual.Reproduce(). should be x >= numParents.
    public float mutationChance;        //from 0 to 1. measure of genetic diversity. 0 is no mutation. 1 will cause an infinite loop after changing every weight.
    public float environmentalPressure; //from 0 to 1. 1 is no survivors
    public int tournamentSize;          //size of the randomly chosen subset from which the most fit individual will be chosen for reproduction.  must be 1 <= x <= populationSize.
                                        //A smaller number will make parent selection more random. A higher number will force higher fitness selections.
    private int numChildrenNeeded;   //filled by constructor
    public float eliteFraction;           //number of solutions to save from one generation to the next 0 is none 1 is all saved. (should never be > 1-environmentalPressure then errors) (if > like .2 then GA won't work well)
    public int numToSave;            //filled by constructor 
    private int numReproductionPairs;//filled by constructor
    public IGeneticIndividual[] individuals;//array to hold population

    public GeneticAlgorithm(IGeneticIndividual progenitor, int populationSize, int numParents, float environmentalPressure, float eliteFraction, int numCrossoverPoints, float mutationChance, int tournamentSize)//initialization of arrays and variables
    {
        this.populationSize = populationSize;
        this.numParents = numParents;
        this.environmentalPressure = environmentalPressure;
        this.eliteFraction = eliteFraction;
        this.numCrossoverPoints = numCrossoverPoints;
        this.mutationChance = mutationChance;
        this.tournamentSize = tournamentSize;

        numToSave = (int)(eliteFraction * populationSize);
        numChildrenNeeded = populationSize - numToSave;//each child will have its own set of parents

        individuals = new IGeneticIndividual[populationSize];
        Initialize(progenitor);
    }

    private void Initialize(IGeneticIndividual progenitor)//called by constructor//randomly decide the aspects of each individual in individuals.
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

    private void Mutate(int individualIndex)//a helper method which calls itself recursively to make mutation truly probabalistic
    {
        if(RandHolder.NextDouble() < mutationChance)
        {
            individuals[individualIndex].Mutate();
            Mutate(individualIndex);
        }
    }

    public IGeneticIndividual TrainGeneration(int numGenerations)//execute a number of iterations of the algorithm creating more fit solutions. Return most fit individual
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
            IGeneticIndividual[,] reproductionGroups = new IGeneticIndividual[numChildrenNeeded,numParents];//uses tournament selection process to fill array
            for (int childIter = 0; childIter < reproductionGroups.GetLength(0); childIter++)//iterate once per child
            {
                for (int parentIter = 0; parentIter < reproductionGroups.GetLength(1); parentIter++)//iterate once per parent. This will 
                {
                    IGeneticIndividual[] tournamentSubset = new IGeneticIndividual[tournamentSize];
                    for (int touranmentIter = 0; touranmentIter < tournamentSubset.Length; touranmentIter++)
                    {
                        tournamentSubset[touranmentIter] = individuals[(int)(RandHolder.NextDouble() * populationSize)];//pick a random individual
                    }
                    Array.Sort(tournamentSubset);
                    reproductionGroups[childIter, parentIter] = tournamentSubset[0];
                }
            }
            //generate children
            IGeneticIndividual[] newChildren = new IGeneticIndividual[numChildrenNeeded];//create empty array to hold newly generated children
            for (int childIter = 0; childIter < newChildren.Length; childIter++)
            {
                IGeneticIndividual[] reproductionGroup = new IGeneticIndividual[numParents];//fill the single dimensioned array, reproductionGroup, with its corresponding individuals from the 2 dimensional array, reproductionGroups.
                for (int parentIter = 0; parentIter < reproductionGroup.Length; parentIter++)
                {
                    reproductionGroup[parentIter] = reproductionGroups[childIter, parentIter];
                }
                newChildren[childIter] = reproductionGroup[0].Reproduce(reproductionGroup, numCrossoverPoints, 1)[0];//generate a single child with reproductionGroup as its parents. Method called by reproductionGroup[0] for convenience.
            }
            //mutate over children array
            for (int mutationIter = 0; mutationIter < populationSize; mutationIter++)
            {
                    Mutate(mutationIter);
            }
            //combine elite and children array and copy into individuals array
            for (int eliteIter = 0; eliteIter < eliteIndividuals.Length; eliteIter++)//copy the high fitness individuals that were saved from the previous generation
            {
                individuals[eliteIter] = eliteIndividuals[eliteIter];
            }
            for (int childIter = 0; childIter < newChildren.Length; childIter++)//copy the newly generated children after the elite individuals
            {
                individuals[eliteIndividuals.Length + childIter] = newChildren[childIter];
            }

            FitnessSort();//order the individuals by fitness
        }
        return individuals[0];//the highest fitness
    }

    public IGeneticIndividual TrainFitness(double targetFitness)
    {
        while(individuals[0].Fitness() <= targetFitness)
        {
            TrainGeneration(1);
        }
        return individuals[0];//the highest fitness
    }
}
