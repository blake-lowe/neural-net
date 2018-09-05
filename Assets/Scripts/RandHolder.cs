using System;
using System.Collections;
using System.Collections.Generic;


public static class RandHolder {

    public static Random rand = new Random();

    public static double NextDouble()
    {
        return rand.NextDouble();
    }
}
