using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameClose : MonoBehaviour
{
    private AudioSource source;
    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }
    private void Update()
    {
        if (!source.isPlaying)
        {
            Application.Quit();
        }
    }
}
