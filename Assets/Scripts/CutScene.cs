using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class CutScene : Singleton<CutScene>
{
    [SerializeField] private PlayableDirector _playableDirector;

    [SerializeField] private GameObject[] _cutSceneObj;
    [SerializeField] private GameObject _blockCanvas;

    private void OnEnable()
    {
        GameManager.OnCutSceneStarted += PlayCutScene;
        //GameManager.OnGameStarted += DisableCutScene;
        GameManager.OnCutSceneEnded += CutSceneEnded;
    }
    public void PlayCutScene()
    {
        _blockCanvas.SetActive(true);

        for (int i = 0; i < _cutSceneObj.Length; i++)
        {
            _cutSceneObj[i].SetActive(true);
        }
        _playableDirector.Play();
    }

    private void CutSceneEnded()
    {
        _blockCanvas.SetActive(false);
        GameManager.Instance.ChangeGameState(GameState.Game);
        StartCoroutine(WaitForSec());
    }


    //public void DisableCutScene()
    //{
    //    gameObject.SetActive(false);
    //}

    private IEnumerator WaitForSec()
    {
        yield return Helpers.Helper.GetWait(2f);
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        GameManager.OnCutSceneStarted -= PlayCutScene;
        //GameManager.OnGameStarted -= DisableCutScene;
        GameManager.OnCutSceneEnded -= CutSceneEnded;
    }
}
