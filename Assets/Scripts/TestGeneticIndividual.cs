using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGeneticIndividual : IGeneticIndividual
{
    public int[] genes;

    public TestGeneticIndividual(int geneSize)
    {
        genes = new int[geneSize];
    }

    public void Randomize()
    {
        for (int i = 0; i < genes.Length; i++)
        {
            if(RandHolder.NextDouble()>0.5)
            {
                genes[i] = 1;
            }
            else
            {
                genes[i] = 0;
            }
        }
    }

    public double Fitness()
    {
        int fitness = 0;
        for (int i = 0; i < genes.Length; i++)
        {
            if(genes[i] == 1)
            {
                fitness += 1;
            }
        }
        return fitness;
    }

    public void Mutate()
    {
        int indexToMutate = (int)(RandHolder.NextDouble() * genes.Length);
        if (RandHolder.NextDouble() > 0.5)
        {
            genes[indexToMutate] = 1;
        }
        else
        {
            genes[indexToMutate] = 0;
        }
    }

    public IGeneticIndividual[] Reproduce(IGeneticIndividual[] IParents, int numCrossoverPoints, int numChildren)
    {
        int numParents = IParents.Length;//convenient variable to have
        //convert array of type IGeneticIndividual to type TestGeneticIndividual
        TestGeneticIndividual[] parents = new TestGeneticIndividual[numParents];
        for (int i = 0; i < numParents; i++)
        {
            parents[i] = IParents[i] as TestGeneticIndividual;
        }
        TestGeneticIndividual[] children = new TestGeneticIndividual[numChildren];//variable to hold generated children. Will be output by method
        for (int childIter = 0; childIter < numChildren; childIter++)//iterate once for each child to be generated
        {
            int[] crossoverPoints = new int[numCrossoverPoints];
            //todo fill x points
            int activeParentIndex = 0;
            //todo generate children use NeuralNet.cs as reference
        }
        //convert array of type TestGeneticIndividual to type IGeneticIndividual
        IGeneticIndividual[] IChildren = new IGeneticIndividual[numChildren];
        for (int i = 0; i < numChildren; i++)
        {
            IChildren[i] = children[i] as IGeneticIndividual;
        }
        return IChildren;
    }

    public int CompareTo(IGeneticIndividual individual)
    {
        if (this.Fitness() > individual.Fitness())
        {
            return -1;//precede in sort order
        }
        else if (this.Fitness() < individual.Fitness())
        {
            return 1;//succeed in sort order
        }
        else
        {
            return 0;//equal in sort
        }
    }

    public int CompareTo(object obj)
    {
        if (obj is IGeneticIndividual)
        {
            IGeneticIndividual GAObj = (IGeneticIndividual)obj;
            if (this.Fitness() > GAObj.Fitness())
            {
                return -1;//precede in sort order
            }
            else if (this.Fitness() < GAObj.Fitness())
            {
                return 1;//succeed in sort order
            }
            else
            {
                return 0;//equal in sort
            }
        }
        else
        {
            throw new System.NotImplementedException();
        }
    }
}
