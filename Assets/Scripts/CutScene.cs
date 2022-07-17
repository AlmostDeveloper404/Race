using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class CutScene : Singleton<CutScene>
{
    [SerializeField] private PlayableDirector _playableDirector;

    [SerializeField] private GameObject[] _cutSceneObj;

    private void OnEnable()
    {
        GameManager.OnCutSceneStarted += PlayCutScene;
        GameManager.OnGameStarted += DisableCutScene;
    }

    public void PlayCutScene()
    {
        for (int i = 0; i < _cutSceneObj.Length; i++)
        {
            _cutSceneObj[i].SetActive(true);
        }
        _playableDirector.Play();
    }

    public void DisableCutScene()
    {
        gameObject.SetActive(false);
        //StartCoroutine(WaitForSec());
    }

    private IEnumerator WaitForSec()
    {
        yield return Helpers.Helper.GetWait(1f);
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        GameManager.OnCutSceneStarted -= PlayCutScene;
        GameManager.OnGameStarted -= DisableCutScene;
    }
}
