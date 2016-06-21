using UnityEngine;
using System;
using System.Diagnostics;

public class CShapTEST : MonoBehaviour {
    int iA = 10;
    int iB = 20;
    uint uiA = 10;
    uint uiB = 20;
    long lA = 10;
    long lB = 20;
    ulong ulA = 10;
    ulong ulB = 20;
    bool bTest;
    Stopwatch sw = new Stopwatch();

    void Awake()
    {

    }

    // Use this for initialization
    void Start () {
        //sw.Start();
        //bTest = iA == iB;
        //bTest = uiA == uiB;
        //bTest = lA == lB;
        //bTest = ulA == ulB;
        //sw.Stop();
        //print("== 경과시간 : " + sw.Elapsed.ToString());
        //sw.Reset();

        //sw.Start();
        //bTest = iA.Equals(iB);
        //bTest = uiA.Equals(uiB);
        //bTest = lA.Equals(lB);
        //bTest = ulA.Equals(ulB);
        //sw.Stop();
        //print("object.Equals 경과시간 : " + sw.Elapsed.ToString());
        //sw.Reset();

        //sw.Start();
        //bTest = ValueType.Equals(iA, iB);
        //bTest = ValueType.Equals(uiA, uiB);
        //bTest = ValueType.Equals(lA,lB);
        //bTest = ValueType.Equals(ulA,ulB);
        //sw.Stop();
        //print("System.ValueType.Equals 경과시간 : " + sw.Elapsed.ToString());
        //sw.Reset();

        //sw.Start();
        //bTest = ReferenceEquals(iA, iB);
        //bTest = ReferenceEquals(uiA, uiB);
        //bTest = ReferenceEquals(lA, lB);
        //bTest = ReferenceEquals(ulA, ulB);
        //sw.Stop();
        //print("System.ReferenceEquals 경과시간 : " + sw.Elapsed.ToString());
        //sw.Reset();

        int iTempA = iA;
        uint uiTempA = uiA;
        long lTempA = lA;
        ulong ulTempA = ulA;

        sw.Start();
        bTest = iTempA == iB;
        bTest = uiTempA == uiB;
        bTest = lTempA == lB;
        bTest = ulTempA == ulB;
        sw.Stop();
        print("== 경과시간 : " + sw.Elapsed.ToString());
        sw.Reset();

        sw.Start();
        bTest = iTempA.Equals(iB);
        bTest = uiTempA.Equals(uiB);
        bTest = lTempA.Equals(lB);
        bTest = ulTempA.Equals(ulB);
        sw.Stop();
        print("object.Equals 경과시간 : " + sw.Elapsed.ToString());
        sw.Reset();

        //sw.Start();
        //bTest = ValueType.Equals(iA, iB);
        //bTest = ValueType.Equals(uiA, uiB);
        //bTest = ValueType.Equals(lA, lB);
        //bTest = ValueType.Equals(ulA, ulB);
        //sw.Stop();
        //print("System.ValueType.Equals 경과시간 : " + sw.Elapsed.ToString());
        //sw.Reset();

        sw.Start();
        bTest = ReferenceEquals(iTempA, iB);
        bTest = ReferenceEquals(uiTempA, uiB);
        bTest = ReferenceEquals(lTempA, lB);
        bTest = ReferenceEquals(ulTempA, ulB);
        sw.Stop();
        print("System.ReferenceEquals 경과시간 : " + sw.Elapsed.ToString());
        sw.Reset();
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
