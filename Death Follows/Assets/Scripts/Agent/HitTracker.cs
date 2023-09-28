using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class HitTracker : ScriptableObject
{
    public List<GameObject> _objectsHit = new List<GameObject>();

}
