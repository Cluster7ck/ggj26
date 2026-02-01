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
        Debug.Log("Do reset");
        var shape = currentLevel.player.GetComponent<Shape>();
        var endPos = currentLevel.player.transform.position.z;
        var playerOffScreen = currentLevel.player.transform.position.z - 10f;
        Sequence.Create()
          .Chain(Tween.PositionZ(currentLevel.player.transform, playerOffScreen, 1.5f)
            .OnComplete(() => shape.Reset()))
          .Chain(Tween.PositionZ(currentLevel.player.transform, endPos, 1.5f));
      }
      else
      {
        Debug.Log("Do next level");
        var playerOffScreen = currentLevel.player.transform.position.z + 10f;
        yield return Sequence.Create()
          .Chain(Tween.PositionZ(currentLevel.player.transform, playerOffScreen + 20f, 4f))
          .ToYieldInstruction();
        currentLevel.player.gameObject.SetActive(false);
      }
    }

    currentLevel = level;

    if (reset)
    {
      yield return wall.AnimateToReset().ToYieldInstruction();
    }
    else
    {
      var targetPos = currentLevel.player.transform.position.z;
      currentLevel.player.transform.position += Vector3.back * 10f;
      currentLevel.player.SetActive(true);
      
      yield return Sequence.Create()
        .Group(Tween.PositionZ(currentLevel.player.transform, targetPos, 2f))
        .Group(wall.AnimateToNext(level))
        .ToYieldInstruction();
    }

    OnLevelChange?.Invoke(this, currentLevel);
    currentLevelIndex++;
    InputAllowed = true;
  }


  public event EventHandler<Level> OnLevelChange;
}