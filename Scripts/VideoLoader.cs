using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System.Collections;

public class VideoLoader : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public string nextSceneName;

    void Start()
    {
        // Unlock cursor for video scene UI interaction
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Start video playback and listen for end event
        videoPlayer.Play();
        videoPlayer.loopPointReached += VideoEnded;
    }

    void VideoEnded(VideoPlayer vp)
    {
        // Load next scene when video finishes
        StartCoroutine(LoadSceneAsync());
    }

    IEnumerator LoadSceneAsync()
    {
        // Async scene loading for smoother transition
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(nextSceneName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    public void SkipScene()
    {
        // Immediate skip to next scene
        SceneManager.LoadScene(nextSceneName);
    }
}