using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CongratulationMessages : MonoBehaviour
{
    public List<GameObject> congrats;
    void Start()
    {
        GameEvents.ShowCongratulation += ShowGratulations;
    }

    private void OnDisable()
    {
        GameEvents.ShowCongratulation -= ShowGratulations;

    }

    private void ShowGratulations()
    {
        var index = UnityEngine.Random.Range(0, congrats.Count);
        congrats[index].SetActive(true);
    }
}
