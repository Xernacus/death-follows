using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    private GameObject _target;
    // Start is called before the first frame update
    void Start()
    {
        _target = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = _target.transform.position;
        Debug.Log(_target.transform.position);
    }
}
