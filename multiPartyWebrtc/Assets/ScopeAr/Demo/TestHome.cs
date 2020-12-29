﻿using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
#if(UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
using UnityEngine.Android;
#endif

/// <summary>
///    TestHome serves a game controller object for this application.
/// </summary>
public class TestHome : MonoBehaviour
{

    // Use this for initialization
#if (UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
    private ArrayList permissionList = new ArrayList();
#endif
    static IVideoChatClient app = null;

    private string HomeSceneName = "SceneHome";
    private string PlaySceneName = "SceneHelloVideo";
    private string ARSceneName = "SceneARClient";

  
    [SerializeField]
    private string AppID = "453c971d028242668ac2ef7919ff986d";

    void Awake()
    {
#if (UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
        permissionList.Add(Permission.Microphone);
        permissionList.Add(Permission.Camera);
#endif

        // keep this alive across scenes
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        CheckAppId();
    }

    void Update()
    {
        CheckPermissions();
    }

    private void CheckAppId()
    {
       // Debug.Assert(AppID.Length > 10, "Please fill in your AppId first on Game Controller object.");
    }

    /// <summary>
    ///   Checks for platform dependent permissions.
    /// </summary>
    private void CheckPermissions()
    {
#if (UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
        foreach (string permission in permissionList)
        {
            if (!Permission.HasUserAuthorizedPermission(permission))
            {
                Permission.RequestUserPermission(permission);
            }
        }
#endif
    }

    public void onJoinButtonClicked()
    {
		Debug.LogError("onJoinButtonClicked");
        // get parameters (channel name, channel profile, etc.)
        GameObject go = GameObject.Find("ChannelName");
        InputField field = go.GetComponent<InputField>();

        // create app if nonexistent
        if (ReferenceEquals(app, null))
        {
            app = new TestHelloUnityVideo(); // create app
            app.loadEngine(AppID); // load engine
        }

        // join channel and jump to next scene
        app.join(field.text);
        SceneManager.sceneLoaded += OnLevelFinishedLoading; // configure GameObject after scene is loaded
        SceneManager.LoadScene(PlaySceneName, LoadSceneMode.Single);
    }


    public void onJoinButton2Clicked()
    {
		Debug.LogError("onJoinButton2Clicked");
        // get parameters (channel name, channel profile, etc.)
        GameObject go = GameObject.Find("ChannelName");
        InputField field = go.GetComponent<InputField>();

        // create app if nonexistent
        if (ReferenceEquals(app, null))
        {
            app = new TestUnityARClient(); // create app
            app.loadEngine(AppID); // load engine
        }

        // join channel and jump to next scene
        app.join(field.text);
        SceneManager.sceneLoaded += OnLevelFinishedLoading; // configure GameObject after scene is loaded
        SceneManager.LoadScene(ARSceneName, LoadSceneMode.Single);
    }

    public void onLeaveButtonClicked()
    {
        if (!ReferenceEquals(app, null))
        {
            app.leave(); // leave channel
            app.unloadEngine(); // delete engine
            app = null; // delete app
            SceneManager.LoadScene(HomeSceneName, LoadSceneMode.Single);
        }
        Destroy(gameObject);
    }

    public void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
       // if (scene.name == PlaySceneName)
        {
            if (!ReferenceEquals(app, null))
            {
                app.onSceneLoaded(); // call this after scene is loaded
            }
            SceneManager.sceneLoaded -= OnLevelFinishedLoading;
        }
    }

    void OnApplicationPause(bool paused)
    {
        Debug.LogError("OnApplicationPause");

        if (!ReferenceEquals(app, null))
        {
            app.EnableVideo(paused);
        }
    }

    void OnApplicationQuit()
    {
        if (!ReferenceEquals(app, null))
        {
            app.unloadEngine();
        }
    }
}