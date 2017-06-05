using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GTInputManager : GTMonoSingleton<GTInputManager>
{
    public override void SetDontDestroyOnLoad(Transform parent)
    {
        base.SetDontDestroyOnLoad(parent);
    }

    void Update()
    {
        OnEscape();
        OnKeyDown();
        OnUpdate();
    }

    void OnEscape()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        if(Application.isPlaying==false)
        {
            GTWindowManager.Instance.Release();
        }
    }

    void OnKeyDown()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            CharacterService.Instance.TryCastSkill(ESkillPos.Skill_0);       
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            CharacterService.Instance.TryCastSkill(ESkillPos.Skill_1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            CharacterService.Instance.TryCastSkill(ESkillPos.Skill_2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            CharacterService.Instance.TryCastSkill(ESkillPos.Skill_3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            CharacterService.Instance.TryCastSkill(ESkillPos.Skill_4);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            CharacterService.Instance.TryCastSkill(ESkillPos.Skill_5);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            CharacterService.Instance.TryCastSkill(ESkillPos.Skill_6);
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            CharacterService.Instance.TryCastSkill(ESkillPos.Skill_7);
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            CharacterService.Instance.TryCastSkill(ESkillPos.Skill_8);
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            CharacterService.Instance.TryCastSkill(ESkillPos.Skill_9);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
      
        }
    }

    void OnUpdate()
    {
        if(Input.GetMouseButton(0))
        {
            GTEventCenter.FireEvent(GTEventID.TYPE_MOUSE_BUTTON);
        }
        if (Input.GetMouseButtonUp(0))
        {
            GTEventCenter.FireEvent(GTEventID.TYPE_MOUSE_BUTTON_UP);
        }
        if (Input.GetMouseButtonDown(0))
        {
            GTEventCenter.FireEvent(GTEventID.TYPE_MOUSE_BUTTON_DOWN);
        }
        if(Input.touchCount>0)
        {
            GTEventCenter.FireEvent(GTEventID.TYPE_MOUSE_BUTTON_DOWN);
        }
        else
        {
            GTEventCenter.FireEvent(GTEventID.TYPE_MOUSE_BUTTON_UP);
        }
    }
}