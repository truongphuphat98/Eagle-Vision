using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scannable : MonoBehaviour
{
    [Header("Scannable Object")]
    public Material normalState;
    public Material scanState;
    public bool IsScaned = false;

    private Renderer rend;

    public void Start()
    {
        rend = this.GetComponent<Renderer>();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            IsScaned = false;
        }
    }

    public void Ping()
    {
        IsScaned = true;
        rend.material = scanState;
        StartCoroutine(StartCountdown());
    }

    public IEnumerator StartCountdown(float countdownValue = 10)
    {
        while (IsScaned)
        {
            yield return new WaitForSeconds(2);
            IsScaned = false;
            rend.material = normalState;
        }
    }

}
