using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommandConsole : MonoBehaviour
{
    InputField field;
    GameController gameController;

    string previousCommand;

    void Start()
    {
        field = GetComponentInChildren<InputField>(true);
        gameController = FindObjectOfType<GameController>();
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Slash))
        {
            field.gameObject.SetActive(true);
            field.Select();
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            field.text = previousCommand;
        }
    }

    public void Command(string input)
    {
        string[] parts = input.Split(new char[] { ' ' }, 2);
        if (parts.Length == 2)
        {
            SendMessage(SetCase(parts[0]), parts[1]);
        }
        previousCommand = field.text;
        field.text = null;
        field.gameObject.SetActive(false);
    }

    void Log(string input)
    {
        Debug.Log(input);
    }

    void Get(string input)
    {
        string[] parts = input.Split(new char[] { ' ' }, 2);
        if (parts.Length == 2)
            switch (SetCase(parts[0]))
            {
                case ("Weapon"):
                    int id;
                    if (int.TryParse(parts[1], out id))
                    {
                        gameController.SpawnWeapon(id, 0);
                    }
                    return;
                case ("Resource"):
                    int value;
                    if (int.TryParse(parts[1], out value))
                    {
                        gameController.AddResource(value);
                    }
                    return;
            }

    }

    void Spawn(string input)
    {

    }

    string SetCase(string input)
    {
        return input.Remove(1).ToUpper() + input.Remove(0, 1).ToLower();
    }
}
