using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBoard : MonoBehaviour
{    
    private Text text;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
        text.text = " ";
    }

    public void UpdateTable()
    {
        string t = null;
        foreach (KeyValuePair<int, PlayerManager> p in GameManager.instance.GetPlayers())
        {
            Debug.Log(p.Value.username);
            t += p.Value.username + ": " + p.Value.score + System.Environment.NewLine;
        }
        text.text = t;
    }
}
