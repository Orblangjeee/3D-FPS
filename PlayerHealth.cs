using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//역할 : Player의 Hp관리
// 현재 HP, 최대 HP, HP 증감
//역할 : 현재 Player의 HP를 화면에 표시
public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance;
    public int currentHp;
    public int maxHp = 10;
    public Image hpBar;
    private void Awake()
    {
        if (Instance == null) { Instance = this; }
    }

    private void Start()
    {
        
        
        currentHp = maxHp;
        hpBar.fillAmount = 1f;//현재 HP를 화면에 표시한다
    }
    // Update is called once per frame
   
    //Player Hp를 증감시킨다
    //value = 증감시킬 양
    public void SetHp(int value)
    {
        
        currentHp += value;//현재 HP에 value 값을 더합니다
            hpBar.fillAmount = (float)currentHp / (float)maxHp;
        
        //갱신된 HP값 표시
        //현재 HP 양을 0~1로 표시해야한다.


        if (currentHp < 1)
        {
            print("Die");
        }//현재 HP가 0이 되면 Player Die
        else if (currentHp > maxHp)
        {
            currentHp = maxHp;
        }
    }

}
