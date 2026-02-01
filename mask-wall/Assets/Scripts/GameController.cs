using System;
using System.Collections;
using System.Numerics;
using PrimeTween;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using Object = UnityEngine.Object;

//[ExecuteInEditMode]
public class GameController : MonoBehaviour
{
  public Level[] levels;
  public Wall wall;

  public float PlayerZ = -2.32f;
  public bool InputAllowed;

  private Level currentLevel;
  private int currentLevelIndex = 0;
  private Coroutine currentSetLevelCoro;

  public static GameController Instance { get; private set; }
  
  public Shape Shape;

  public Score Score;

  private void Awake()
  {
    Score = Object.FindFirstObjectByType<Score>();
    if (Instance == null)
    {
      Instance = this;
    }
  }

  private void Start()
  {
    if (levels.Length > 0)
    {
      StartCoroutine(SetLevel(levels[0], false));
    }
  }

  private void Update()
  {
    if (Input.GetKeyDown(KeyCode.Q))
    {
      NextLevel();
    }
  }

  public void NextLevel()
  {
    if (currentSetLevelCoro != null)
    {
      StopCoroutine(currentSetLevelCoro);
    }

    var reset = Score.CalculateAccuracy() < 40;
    var nextLevel = reset ? currentLevel : levels[(currentLevelIndex + 1) % levels.Length];

    currentSetLevelCoro = StartCoroutine(SetLevel(nextLevel, reset));
  }

  private IEnumerator SetLevel(Level level, bool reset)
  {
    InputAllowed = false;
    if (currentLevel != null)
    {
      if (reset)
      {
        var playerOffScreen = currentLevel.player.transform.position.z - 10f;
        yield return Tween.PositionZ(currentLevel.player.transform, playerOffScreen, 3f).ToYieldInstruction();
        var shape = currentLevel.player.GetComponent<Shape>();
        shape.Reset();
      }
      else
      {
        var playerOffScreen = currentLevel.player.transform.position.z + 10f;
        yield return Sequence.Create()
          .Chain(Tween.PositionZ(currentLevel.player.transform, playerOffScreen, 3f))
          .Chain(Tween.ShakeLocalPosition(currentLevel.player.transform, new Vector3(0.4f, 0.4f, 0), 2f))
          .Chain(Tween.PositionZ(currentLevel.player.transform, playerOffScreen + 20f, 3f))
          .ToYieldInstruction();
        currentLevel.player.gameObject.SetActive(false);
      }
    }

    currentLevel = level;
    var targetPos = currentLevel.player.transform.position.z;
    currentLevel.player.transform.position += Vector3.back * 10f;
    currentLevel.player.SetActive(true);

    yield return Sequence.Create()
      .Group(Tween.PositionZ(currentLevel.player.transform, targetPos, 2f))
      .Group(wall.AnimateToNext(level))
      .ToYieldInstruction();

    OnLevelChange?.Invoke(this, currentLevel);
    InputAllowed = true;
  }


  public event EventHandler<Level> OnLevelChange;
}