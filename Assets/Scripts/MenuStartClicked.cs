using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuStartClicked : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void StartNavigation(){
        SceneManager.LoadScene("Navigation");
    }
    public void search(){
        
    }
}
