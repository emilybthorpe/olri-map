using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class startButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void changeToInstruction(){
        SceneManager.LoadScene("Instruction Scene");
    }
    public void search(){
        
    }
}
