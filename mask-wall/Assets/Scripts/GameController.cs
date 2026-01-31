using System;
using UnityEngine;
using Object = UnityEngine.Object;

[ExecuteInEditMode]
public class GameController : MonoBehaviour
{
    public Level[] levels;

    private Level _currentLevel;
    public float PlayerZ = -2.32f;

    private Level currentLevel;
    private int index = 0;

    public static GameController Instance { get; private set; }

    public Shape Shape => Object.FindFirstObjectByType<Shape>();

    public Score Score => Object.FindFirstObjectByType<Score>();

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
            Debug.Log("changed level " + levels.GetHashCode());
            _currentLevel.player.SetActive(true); // TODO animate
            OnLevelChange?.Invoke(this, _currentLevel);
            Debug.Log("changed level to index " + index);
        }
    }

    public event EventHandler<Level> OnLevelChange;
}