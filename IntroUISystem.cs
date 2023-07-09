using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//역할 : Intro 씬에서 UI들의 Interaction(상호작용)을 관리
// "HowToPanel" : 창 활, 비활
// "GameStart" : 게임 시작, 씬 이동
// "GameQuit" : 게임 종료

public class IntroUISystem : MonoBehaviour
{
    public GameObject panel_HowToUse; //HowToUse
    public GameObject canvas_Loading; //LoadingCanvas
    public float loadingTime = 3f; //일정시간(Loading시간) :3초
    
    public void OnClickHowToUseButton()
    {
        panel_HowToUse.SetActive(true);
    }

    public void OnClickHowToUseCloseButton()
    {
        panel_HowToUse.SetActive(false);
    }
    

    public void OnClickGameStart()
    {
        //LoadingScene(LoadingManager.cs)를 통해 다음 Scene으로 이동
        LoadingManager.LoadScene("Navigation");
        
        //LoadingCanvas 활성화
        //canvas_Loading.SetActive(true);
        //Invoke(nameof(LoadScene), loadingTime);
    }

    //private void LoadScene()
    //{
    //    SceneManager.LoadScene("Navigation");
    //}

    public void OnClickGameQuit()
    {
#if UNITY_EDITOR
        //A. Unity Editio 모드 일 때는 PlayerMode 종료
        UnityEditor.EditorApplication.isPlaying = false;
#else
        //B. 실제 Build된 프로그램 종료
        Application.Quit();
#endif
    }
}
