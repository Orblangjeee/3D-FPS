using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//���� : Player�� Hp����
// ���� HP, �ִ� HP, HP ����
//���� : ���� Player�� HP�� ȭ�鿡 ǥ��
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
        hpBar.fillAmount = 1f;//���� HP�� ȭ�鿡 ǥ���Ѵ�
    }
    // Update is called once per frame
   
    //Player Hp�� ������Ų��
    //value = ������ų ��
    public void SetHp(int value)
    {
        
        currentHp += value;//���� HP�� value ���� ���մϴ�
            hpBar.fillAmount = (float)currentHp / (float)maxHp;
        
        //���ŵ� HP�� ǥ��
        //���� HP ���� 0~1�� ǥ���ؾ��Ѵ�.


        if (currentHp < 1)
        {
            print("Die");
        }//���� HP�� 0�� �Ǹ� Player Die
        else if (currentHp > maxHp)
        {
            currentHp = maxHp;
        }
    }

}
