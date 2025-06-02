using System.Collections;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private GameObject panelGameStart;
    public bool IsGameStart { get; private set; } = false;

    private IEnumerator Start()
    {
        while (true) 
        {
            if (Input.GetMouseButtonDown(0))
            {
                GameStart();

                yield break;
            }
            yield return null;
        }
    }

    private void GameStart()
    {
        IsGameStart = true;
        panelGameStart.SetActive(false);
    }
}
