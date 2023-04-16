using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BucketRandom<T>
{
    [SerializeField]
    private T[] defaultBucket;
    
    private List<T> bucket = new List<T>();

    public T GetRandom()
    {
        if (bucket.Count == 0)
        {
            if (defaultBucket.Length == 0)
            {
                Debug.LogError("defaultBucket is empty!");
                return default;
            }

            bucket.AddRange(defaultBucket);
        }

        var i = UnityEngine.Random.Range(0, bucket.Count);
        var result = bucket[i];
        bucket.RemoveAt(i);

        return result;
    }
}