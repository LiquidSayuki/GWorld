﻿using Assets.Scripts.Managers;
using Services;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{

    public GameObject UITips;
    public GameObject UILoading;
    public GameObject UILogin;

    public Slider progressBar;
    public Text progressText;
    public Text progressNumber;

    // Use this for initialization
    IEnumerator Start()
    {
        log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo("log4net.xml"));
        UnityLogger.Init();
        Common.Log.Init("Unity");
        Common.Log.Info("LoadingManager start");

        UITips.SetActive(true);
        UILoading.SetActive(false);
        UILogin.SetActive(false);
        yield return new WaitForSeconds(2f);
        UILoading.SetActive(true);
        yield return new WaitForSeconds(1f);
        UITips.SetActive(false);

        yield return DataManager.Instance.LoadData();
        // DataManager.Instance.Load();

        //Init basic services
        MapService.Instance.Init();
        UserService.Instance.Init();
        StatusService.Instance.Init();
        FriendService.Instance.Init();
        TeamService.Instance.Init();
        GuildService.Instance.Init();
        ChatService.Instance.Init();
        SoundManager.Instance.Playmusic(SoundDefine.Music_Login);


        // Fake Loading Simulate
        for (float i = 50; i < 100;)
        {
            i += Random.Range(0.1f, 1.5f);
            progressBar.value = i;
            yield return new WaitForEndOfFrame();
        }

        UILoading.SetActive(false);
        UILogin.SetActive(true);
        yield return null;
    }
}
