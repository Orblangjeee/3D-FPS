using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// ���� : Player�� ������ �ִ� Weapon �� ���� (Ȱ/��Ȱ)
// ������ ���� ��������(Ȱ��ȭ, ��Ȱ��ȭ), �������⼱��/�������⼱��, ������� �Է� (���콺 ��ũ�� ��)

//[����] ó�� ����, ������ ���⸦ ������ �ÿ� ���õ� ���Ⱑ ���� ��Ȳ
//[�ذ�] selectedWeaponIndex �� ������ �����ؼ� �ذ��Ѵ�.
//Try : (���� ��ȯ) ù��° ���⿡�� ���� ���� �����ϸ� ������ ����� ����/ ������ ���⿡�� ���� ���� ���ý� ó�� ����� ����

public class SwitchWeapon : MonoBehaviour
{
    public int selectedWeaponIndex = 0;//���� ������ ��
    public GameObject[] weapons;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //A. ���� ������ ������ Index ��ȣ�� ����
        int preWeaponIndex = selectedWeaponIndex;

      //B-a. ���⺯�� : ���콺 ��ũ�� �� -- Up
      if (Input.GetAxis("Mouse ScrollWheel") >0 )
        {
           
            ChangePreWeapon();
        }
      //B-b. ���⺯�� : ���콺 ��ũ�� �� -- Down
      else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            ChangeNextWeapon();
        }

      //C. (B)���� ��ũ���ؼ�, (A)���� ������ ���� ��ȣ�� �޶����ٸ�, ������ ���⸦ Ȱ��ȭ���� ��ȯ
      if (preWeaponIndex != selectedWeaponIndex)
        {
            SwitchingWeapon();
        }
    }
    


    //���� ���� ����
    private void ChangeNextWeapon()
    {
        //���� ������ ���� ��ȣ +1
        selectedWeaponIndex++;
        //���� ���� ������ ������ Index ��ȣ�� �Ѿ�� ���ϰ� ����
        //selectedWeaponIndex�� �� ������ ���� Index���� Ŀ���� ������ ���� Index�� ����
        if (selectedWeaponIndex > weapons.Length -1)
        {
            selectedWeaponIndex = 0;
        } 
    }
    //���� ���� ����
    private void ChangePreWeapon()
    {
        //���� ������ ���� ��ȣ -1
        selectedWeaponIndex--;
        if (selectedWeaponIndex < 0)
        {
            selectedWeaponIndex = weapons.Length -1;
        }
    }

    private void SwitchingWeapon()
    {
        //���� ������ �ִ� Weapon�� �߿��� Ȱ��ȭ�� Weapon Index(��ȣ)�� ������ Weapon Index(selectedWeaponIndex)�� ���ٸ�, ��ȣ�� ���� ���� Ȱ��ȭ
        // Weapon Index �ٸ���, ��ȣ�� �ٸ� ���� ��Ȱ��ȭ
        
        for (int i=0; i <weapons.Length; i++)
        {
            if (i == selectedWeaponIndex) 
            {
                weapons[i].SetActive(true);
            }
            else if (i != selectedWeaponIndex)
            {
                weapons[i].SetActive(false);
            }
        }
        
    }

    public void AddWeapon(GameObject weapon)
    {
        weapons.AddRange(weapons);
    }

    public void RemoveWeapon(int weaponIndex)
    {
      // weapons.(weaponIndex);
    }
}
