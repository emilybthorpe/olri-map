using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ToggleHandler : MonoBehaviour
{
    public GameObject panel;
    public Vector3 openPosition  = new Vector3(0f,423f, 0f);
    public Vector3 closePosition = new Vector3(0f, -409f, 0f);
    // Start is called before the first frame update
    void Start()
    {
        
        RectTransform rt = panel.GetComponent<RectTransform>();
        rt.anchoredPosition = closePosition;
    }
    public void open(){
        panel.GetComponent<RectTransform>().anchoredPosition = openPosition;
    }
    public void close(){
        panel.GetComponent<RectTransform>().anchoredPosition = closePosition;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
