using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StatsPerDrain
{
    public Module module;
    private List<float> drainrate = new List<float>();
    private List<float> timePerProduct = new List<float>();

    public StatsPerDrain(Module name, float _drainrate, float _timePerProduct)
    {
        module = name;
        drainrate.Add(_drainrate);
        timePerProduct.Add(_timePerProduct);
    }

    public void Add(float _drainrate, float _timePerProduct)
    {
        drainrate.Add(_drainrate);
        timePerProduct.Add(_timePerProduct);
    }

    public List<float> getValuesInPosition(int i)
    {
        return new List<float> { drainrate[i], timePerProduct[i]};
    }

    public int getCount()
    {
        return drainrate.Count;
    }

    public List<float> getAverage()
    {
        return new List<float> { drainrate.Average(), timePerProduct.Average() };
    }
}
