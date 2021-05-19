using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public InputField usernameField;
    private void Awake()
    {
        //ensure there is only 1 instance
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(transform.gameObject);
    }

    public void ConnectToServer()
    {
        if (usernameField.text.Length > 3)
        {
            Debug.Log("Next");
            //switch scene then connect to server.
            LoadScene(1);
            //Client.instance.ConnectToServer();
        }
        else
        {
            //usernameField.placeholder.GetComponent<Text>().text = "Please type username";
            usernameField.image.color = new Color(1f, 0.9f, .5f);
        }
    }

    public void LoadScene(int _i)
    {
        SceneManager.LoadScene(_i);
    }

    public void Settings()
    {
        //load settings scene
    }

    public void Ping()
    { 
        
    }

    public void Quit()
    {
#if UNITY_EDITOR
        // Application.Quit() does not work in the editor so
        // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }
}
