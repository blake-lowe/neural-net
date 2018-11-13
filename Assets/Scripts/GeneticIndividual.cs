using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface GeneticIndividual : IComparable//if IComparable isn't availible then make sure "using System;"
{
    double fitness();//return how fit the individual is

    GeneticIndividual randomize();

    GeneticIndividual[] reproduce(GeneticIndividual[] parents, int crossoverPoints, int numChildren);//take an array of individuals and return an array of children

    GeneticIndividual mutate();//change a bit of the genes

    int CompareTo(GeneticIndividual individual); //for help implementing: https://docs.microsoft.com/en-us/dotnet/api/system.icomparable-1.compareto?view=netframework-4.7.2
}
