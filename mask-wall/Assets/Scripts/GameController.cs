using System;
using UnityEngine;

[ExecuteInEditMode]
public class GameController : MonoBehaviour
{
    public Level[] levels;
    
    private Level currentLevel;

    private void Start()
    {
        if (levels.Length > 0)
        {
            CurrentLevel = levels[0];
        }
    }

    public Level CurrentLevel
    {
        get => currentLevel;
        set
        {
            currentLevel = value;
            OnLevelChange?.Invoke(this, currentLevel);
        }
    }

    public event EventHandler<Level> OnLevelChange;
}