using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class IEmumaratorExample : MonoBehaviour
{
    public void Awake()
    {
        StartCoroutine(Start());
    }
    IEnumerator Start()
    {
        yield return null;
        yield return StartCoroutine(Test1());
        yield return StartCoroutine(Test2());
        int k = 100;
        Debug.Log(k);
        StopCoroutine(Test1());
        StopCoroutine(Test2());        
    }
    IEnumerator Test1()
    {        
        Debug.Log("Test1");        
        yield return 1;
    }
    IEnumerator Test2()
    {        
        Debug.Log("Test2");        
        yield return 2;
    }
    void Update()
    {
        Debug.Log("Update");
        StopCoroutine(Start());
    }
}
