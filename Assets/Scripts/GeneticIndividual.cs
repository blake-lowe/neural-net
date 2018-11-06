using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface GeneticIndividual<T>
{
    double fitness();//return how fit the individual is

    IList<T> reproduce(IList<T> parents, int crossoverPoints, int numChildren);//take an array of individuals and return an array of children

    T mutate();//change a bit of the genes
}
