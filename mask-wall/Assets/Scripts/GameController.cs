using System;
using UnityEngine;

[ExecuteInEditMode]
public class GameController : MonoBehaviour
{
    public Level[] levels;

    private Level _currentLevel;

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

    public Level CurrentLevel
    {
        get => _currentLevel;
        set
        {
            _currentLevel = value;
            OnLevelChange?.Invoke(this, _currentLevel);
        }
    }

    public event EventHandler<Level> OnLevelChange;
}