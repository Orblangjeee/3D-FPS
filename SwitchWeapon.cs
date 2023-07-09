using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// 역할 : Player가 가지고 있는 Weapon 을 변경 (활/비활)
// 선택한 총이 무엇인지(활성화, 비활성화), 이전무기선택/다음무기선택, 사용자의 입력 (마우스 스크롤 휠)

//[문제] 처음 무기, 마지막 무기를 선택할 시에 선택된 무기가 없는 상황
//[해결] selectedWeaponIndex 의 범위를 제한해서 해결한다.
//Try : (무기 순환) 첫번째 무기에서 이전 무기 선택하면 마지막 무기로 변경/ 마지막 무기에서 다음 무기 선택시 처음 무기로 변경

public class SwitchWeapon : MonoBehaviour
{
    public int selectedWeaponIndex = 0;//내가 선택한 총
    public GameObject[] weapons;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //A. 내가 선택한 무기의 Index 번호를 저장
        int preWeaponIndex = selectedWeaponIndex;

      //B-a. 무기변경 : 마우스 스크롤 휠 -- Up
      if (Input.GetAxis("Mouse ScrollWheel") >0 )
        {
           
            ChangePreWeapon();
        }
      //B-b. 무기변경 : 마우스 스크롤 휠 -- Down
      else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            ChangeNextWeapon();
        }

      //C. (B)에서 스크롤해서, (A)에서 선택한 무기 번호가 달라졌다면, 변경한 무기를 활성화시켜 전환
      if (preWeaponIndex != selectedWeaponIndex)
        {
            SwitchingWeapon();
        }
    }
    


    //다음 무기 선택
    private void ChangeNextWeapon()
    {
        //현재 선택한 무기 번호 +1
        selectedWeaponIndex++;
        //내가 가진 무기의 마지막 Index 번호를 넘어가지 못하게 제한
        //selectedWeaponIndex가 내 마지막 무기 Index보다 커지면 마지막 무기 Index로 제한
        if (selectedWeaponIndex > weapons.Length -1)
        {
            selectedWeaponIndex = 0;
        } 
    }
    //이전 무기 선택
    private void ChangePreWeapon()
    {
        //현재 선택한 무기 번호 -1
        selectedWeaponIndex--;
        if (selectedWeaponIndex < 0)
        {
            selectedWeaponIndex = weapons.Length -1;
        }
    }

    private void SwitchingWeapon()
    {
        //내가 가지고 있는 Weapon들 중에서 활성화된 Weapon Index(번호)와 선택한 Weapon Index(selectedWeaponIndex)가 같다면, 번호가 같은 무기 활성화
        // Weapon Index 다르면, 번호가 다른 무기 비활성화
        
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
