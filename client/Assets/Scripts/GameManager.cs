using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public static Dictionary<int, PlayerManager> players = new Dictionary<int, PlayerManager>();
    public List<GameObject> rockets;
    public GameObject scoreboard;
    public GameObject ping;

    public GameObject localPlayerPrefab;
    public GameObject playerPrefab;
    public GameObject rocketPrefab;
    public GameObject explosionPrefab;
    

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
            Destroy(this);
        }
        //DontDestroyOnLoad(transform.gameObject);
    }

    public void SpawnPlayer(int _id, string _username, Vector3 _position, Quaternion _rotation)
    {
        GameObject player;
        if (_id == Client.instance.myId)
        {
            player = Instantiate(localPlayerPrefab, _position, _rotation);
        }
        else
        {
            player = Instantiate(playerPrefab, _position, _rotation);
        }

        player.GetComponent<PlayerManager>().Initialize(_id, _username);
        players.Add(_id, player.GetComponent<PlayerManager>());
    }

    public Dictionary<int, PlayerManager> GetPlayers()
    {
        return players;
    }

    public void SpawnRocket(int _pid, int _rid, Vector3 _position, Quaternion _rotation)
    {
        Debug.Log("Attemping to spawn rocket");
        GameObject rocket = Instantiate(rocketPrefab, _position, _rotation);
        rocket.GetComponent<RocketController>().Initialize(_pid, _rid);
        rockets.Add(rocket);

        if (_pid == PlayerController.instance.ID())
        {
            PlayerController.instance.PingRecived();// updating poing using the rocket shots
        }

    }

    public void RocketExplode(int _pid, int _rid)
    {
        foreach (GameObject rock in rockets)
        {
            if (rock.GetComponent<RocketController>().id == _rid && rock.GetComponent<RocketController>().playerid == _pid)
            {
                Instantiate(explosionPrefab, rock.transform.position, rock.transform.rotation);
                rock.GetComponent<RocketController>().Disable();
                return;
            }
        }
    }

    public void DisableRocket(GameObject _rocket)
    {
        rockets.Remove(_rocket);
    }

    public void SetScore(int _id, int _score)
    {
        Debug.Log($"Player {_id} scored: {_score}");
        players[_id].score = _score;
        UpdateScore();
    }
    public void UpdateScore()
    {
        scoreboard.GetComponent<ScoreBoard>().UpdateTable();
    }

    public void UpdatePing(int _ping)
    {
        ping.GetComponent<Text>().text = _ping + " (ms)";
    }
}
