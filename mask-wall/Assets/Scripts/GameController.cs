using System;
using UnityEngine;

[ExecuteInEditMode]
public class GameController : MonoBehaviour
{
    public Level[] levels;

    private Level _currentLevel;
    private int index = 0;

    public static GameController Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        if (levels.Length > 0)
        {
            CurrentLevel = levels[0];
        }
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Q))
        {
            index = (index + 1) % levels.Length;
            CurrentLevel = levels[index];
        }
    }

    public Level CurrentLevel
    {
        get => _currentLevel;
        set
        {
            _currentLevel = value;
            OnLevelChange?.Invoke(this, _currentLevel);
            Debug.Log("changed level to index " + index);
        }
    }

    public event EventHandler<Level> OnLevelChange;
}