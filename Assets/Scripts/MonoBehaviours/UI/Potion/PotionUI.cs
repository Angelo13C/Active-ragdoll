using System;
using UnityEngine;
using UnityEngine.UI;

public class PotionUI : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private Image _timer;

    private float _timerDuration = 1f;
    private float _timerCurrentTime = 0f;
    
    public void SetIcon(Sprite sprite, float timerDuration)
    {
        _icon.sprite = sprite;
        _timerDuration = timerDuration;
        _timerCurrentTime = timerDuration;
    }

    private void Update()
    {
        _timerCurrentTime -= Time.deltaTime;
        _timer.fillAmount = _timerCurrentTime / _timerDuration;
        if(_timerCurrentTime <= 0f)
            gameObject.SetActive(false);
    }
}
