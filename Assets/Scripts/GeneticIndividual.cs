using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGeneticIndividual : IComparable//if IComparable isn't availible then make sure "using System;"
{
    double Fitness();//return how fit the individual is

    void Randomize();

    IGeneticIndividual[] Reproduce(IGeneticIndividual[] parents, int crossoverPoints, int numChildren);//take an array of individuals and return an array of children

    void Mutate();//change a bit of the genes

    int CompareTo(IGeneticIndividual individual); //for help implementing: https://docs.microsoft.com/en-us/dotnet/api/system.icomparable-1.compareto?view=netframework-4.7.2
}
