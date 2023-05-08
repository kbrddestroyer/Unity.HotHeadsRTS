using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class UI_Controller : MonoBehaviour
{
    [SerializeField] private string[] scenes;

    public void ExitGame()
    {
        Application.Quit();
    }

    public void StartGame(TMP_Dropdown menu)
    {
        if (menu.value > scenes.Length)
        {
            Debug.LogError("UI настроен неверно! Значение Dropdown не соответствует переданному массиву сцен!");
            Application.Quit(1);
            return;
        }

        SceneManager.LoadScene(scenes[menu.value]);
    }
}
