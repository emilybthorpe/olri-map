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
    public bool isOpen = false;
    void Start()
    {
        
        RectTransform rt = panel.GetComponent<RectTransform>();
        rt.anchoredPosition = closePosition;
    }
    public void changePanelState(){
        if (isOpen){
            close();
        }else{
            open();
        }
    }
    public void open(){
        panel.GetComponent<RectTransform>().anchoredPosition = openPosition;
        isOpen = true;
    }
    public void close(){
        panel.GetComponent<RectTransform>().anchoredPosition = closePosition;
        isOpen = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
