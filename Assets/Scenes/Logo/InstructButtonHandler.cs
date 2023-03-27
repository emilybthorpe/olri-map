using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InstructButtonHandler : MonoBehaviour
{
    public void LoadNextScene()
    {
        SceneManager.LoadScene("InstructionScene");
    }
}