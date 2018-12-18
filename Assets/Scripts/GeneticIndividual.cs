using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGeneticIndividual : IComparable //an interface designed to be implemented by classes which are used with GeneticAlgorithm.
{
    double Fitness();//return how fit the individual is/how good of a solution the individual is

    void Randomize();//randomly decide all aspects of the individual

    IGeneticIndividual[] Reproduce(IGeneticIndividual[] parents, int crossoverPoints, int numChildren);//take an array of individuals and return an array of children
                                                                                                       //Doesn't necessarily need to be called by one of the parents but that's probably convenient. 
                                                                                                       //numCrossover points SHOULD be > numParents-1 (to use all parents at least once), but it's fine if not
    void Mutate();//change a bit of the genes

    int CompareTo(IGeneticIndividual individual); //for help implementing: https://docs.microsoft.com/en-us/dotnet/api/system.icomparable-1.compareto?view=netframework-4.7.2
}
