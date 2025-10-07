using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillInventory : MonoBehaviour
{
    public Dictionary<int, bool> skills = new Dictionary<int, bool>();

    public bool skill_Move1 = false;
    public bool skill_Move2 = false;

    private void Awake()
    {
        // 初始有多少技能都先false
        skills[1] = false;
        skills[2] = false;
        Debug.Log("skills[2]是：" + skills[2]);
    }

    // 通用解鎖方法
    public void UnlockSkill(int id)
    {
        if (skills.ContainsKey(id))
        {
            skills[id] = true;
            Debug.Log("技能 " + id + " 已解鎖");
        }
        else
        {
            Debug.LogWarning("技能ID不存在: " + id);
        }
    }
}
