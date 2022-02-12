using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrashDetector : MonoBehaviour
{
    [SerializeField] float _reloadDelay = 1f;
    [SerializeField] ParticleSystem _crashEffect;

    private bool _crashed;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_crashed)
            return;
        if (collision.gameObject.tag == "Ground")
        {
            _crashed = true;
            Debug.Log("Auch");
            _crashEffect.Play();
            GameManager.Instance.ReloadLevel(_reloadDelay);
        }
    }


}
