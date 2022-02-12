using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


[RequireComponent(typeof(Collider2D))]
public class FinishLine : MonoBehaviour
{

    [SerializeField] float _reloadDelay = 1f;
    [SerializeField] ParticleSystem _finishEffect;

    ///public event Action OnFinish;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            //  OnFinish();
            _finishEffect.Play();
            Debug.Log("You finished");



            GameManager.Instance.ReloadLevel(_reloadDelay);

        }
    }
}
