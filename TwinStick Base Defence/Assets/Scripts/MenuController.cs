using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    [SerializeField] Canvas[] menus;
    List<string> history = new List<string>();

    void Start()
    {
        
    }
    
    void Update()
    {
        
    }

    public void SetMenu(string name)
    {
        SetMenu(name,true);
    }

    public void SetMenu(string name, bool save, bool clear = false, bool bounce = true)
    {
        if (clear)
        {
            history.Clear();
        }
        foreach (Canvas menu in menus)
        {
            if (menu.name == name)
            {
                if (menu.enabled == true)
                {
                    save = false;
                    if (bounce)
                    {
                        Return();
                    }
                }
                else
                {
                    menu.enabled = true;
                }
            }
            else
            {
                menu.enabled = false;
            }
        }
        if (save)
        {
            history.Add(name);
        }
    }

    public void Return(int index, bool clear)
    {
        SetMenu(history[index], true, clear, false);
    }

    public void Return()
    {
        history.Remove(history[history.Count - 1]);
        SetMenu(history[history.Count - 1], false);
    }
}
