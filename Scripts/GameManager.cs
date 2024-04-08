using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject MapGen2;

    void Awake()
    {
        
    }

    void Start()
    {
        MapGen2.GetComponent<MapGen2>().Gen();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}