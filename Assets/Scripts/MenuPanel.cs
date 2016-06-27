using UnityEngine;
using System.Collections;

public class MenuPanel : MonoBehaviour {

    private TweenScale myTweenScale;

    void Awake()
    {
        this.myTweenScale = this.GetComponent<TweenScale>();
        this.gameObject.SetActive(false);
    }

    void OnDisable()
    {
        this.myTweenScale.Reset();
    }
}
