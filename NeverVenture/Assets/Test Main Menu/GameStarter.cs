using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking.Match;


public class GameStarter : MonoBehaviour
{
    public GameObject Unit;
    public NetworkManager Network1;
    public Text LanAddressE;
    public string LanAddressT;
    public Text MatchNameE;
    public string MatchNameT;
    public Text MatchNameJ;
    public string MatchNameJT;
    public List<MatchInfoSnapshot> test = new List<MatchInfoSnapshot>();


    // Use this for initialization
    void Start()
    {
       Network1 = GetComponent<NetworkManager>();
       // Network1.StartMatchMaker();
    }

    // Update is called once per frame
    void Update()
    {
        if (NetworkServer.active == true)
        {
        }
        else if (NetworkClient.active == true)
        {
        }
       
    }
    void LanAddressFind()
    {

        Text LanAddressE = GameObject.FindGameObjectWithTag("LanAddress").GetComponent<Text>();
        LanAddressT = LanAddressE.text;
        Network1.networkAddress = LanAddressT;

        ///Network1.SetMatchHost(LanAddressT, 7777, false); 
    }
    void MatchName()
    {
        Text MatchNameE = GameObject.FindGameObjectWithTag("MatchName").GetComponent<Text>();
        MatchNameT = MatchNameE.text;
        NetworkManager.singleton.ServerChangeScene("DungeonNonOnline");
        //Network1.SetMatchHost(LanAddressT, 443, true);
    }
    void MatchNameJoin()
    {
        Text MatchNameJ = GameObject.FindGameObjectWithTag("MatchNameJ").GetComponent<Text>();
        MatchNameJT = MatchNameJ.text;
        NetworkManager.singleton.ServerChangeScene("DungeonNonOnline");
        //Network1.SetMatchHost(LanAddressT, 443, true);
    }

    void QuitGame()
    {
        Application.Quit();
    }
    void SoloGame()
    {
        SceneManager.LoadScene("DungeonNonOnline", LoadSceneMode.Single);
    }
    void LanHost()
    {
        Network1.StopHost();
        Network1.matchSize = 4;
        Network1.networkAddress = LanAddressT;
        StartCoroutine(HostStartT(1));

    }
    private IEnumerator HostStartT(int t)
    {
        yield return new WaitForSeconds(t);
        Network1.StartHost();
    }
    void LanJoin()
    {
        print("Im Retared");
        Network1.networkAddress = LanAddressT;
        Network1.StartClient();
        NetworkManager.singleton.ServerChangeScene("DungeonNonOnline");
        //Network1.ServerChangeScene("DungeonNonOnline");
        //if (Network1.clientLoadedScene)
        //{
        //    print("ILOADED");
        //}
    }
    void Network1Host()
    {
        Network1.StopHost();
        Network1.StartMatchMaker();
        Network1.matchMaker.CreateMatch(MatchNameT,4,true,"","","",0,1,Network1.OnMatchCreate);
        //foreach (var match in Network1.matches)
        //{
            
        //    if (match.name == MatchNameT)
        //    {
        //        Network1.matchName = match.name;
        //        Network1.matchSize = (uint)match.currentSize;
        //        Network1.matchMaker.JoinMatch(match.networkId, "", "", "", 0, 1, Network1.OnMatchJoined);
        //    }
        //}

    }
    void Network1Join()
    {
        Network1.StopHost();
        Network1.StartMatchMaker();
        NetworkManager.singleton.matchMaker.ListMatches(0, 10, MatchNameJT, false, 0, 1, OnInternetMatchList);
        NetworkManager.singleton.ServerChangeScene("DungeonNonOnline");
        print(Network1.matches);
    }

    private void OnInternetMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matchList)
    {
        if (success)
        {
            if (matchList.Count != 0)
            {
                Debug.Log("Matches Found");
                NetworkManager.singleton.matchMaker.JoinMatch(matchList[0].networkId, "", "", "", 0, 1, OnMatchJoined);
            }
        }
        else
        {
            Debug.Log("ERROR : Match Search Failure");
        }
    }
    public virtual void OnMatchJoined(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        if (success)
        {
            Debug.Log("Match Joined");
            MatchInfo hostInfo = matchInfo;
            NetworkManager.singleton.StartClient(hostInfo);
        }
        else
        {
            Debug.Log("ERROR : Match Join Failure");
        }

    }

}