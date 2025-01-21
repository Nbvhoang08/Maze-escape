using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour, IObserver
{
    // Start is called before the first frame update
    [SerializeField] private GameObject effect;
    void Awake()
    {
        Subject.RegisterObserver(this);
    }
    void OnDestroy()
    {
        Subject.UnregisterObserver(this);
    }
    public void OnNotify(string eventName, object eventData)
    {
        if(eventName == "TakeSword")
        {
            effect.SetActive(true);    
        }
    }
    void Start()
    {
        effect.SetActive(false);    
    }
}
