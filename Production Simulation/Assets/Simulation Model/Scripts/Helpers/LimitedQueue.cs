using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LimitedQueue<T> : Queue<T>
{
    public int Limit { get; set; }

    public LimitedQueue(int limit) : base(limit)
    {
        Limit = limit;
    }
}
