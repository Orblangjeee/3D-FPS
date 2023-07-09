using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//���� : Intro ������ UI���� Interaction(��ȣ�ۿ�)�� ����
// "HowToPanel" : â Ȱ, ��Ȱ
// "GameStart" : ���� ����, �� �̵�
// "GameQuit" : ���� ����

public class IntroUISystem : MonoBehaviour
{
    public GameObject panel_HowToUse; //HowToUse
    public GameObject canvas_Loading; //LoadingCanvas
    public float loadingTime = 3f; //�����ð�(Loading�ð�) :3��
    
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
        //LoadingScene(LoadingManager.cs)�� ���� ���� Scene���� �̵�
        LoadingManager.LoadScene("Navigation");
        
        //LoadingCanvas Ȱ��ȭ
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
        //A. Unity Editio ��� �� ���� PlayerMode ����
        UnityEditor.EditorApplication.isPlaying = false;
#else
        //B. ���� Build�� ���α׷� ����
        Application.Quit();
#endif
    }
}
