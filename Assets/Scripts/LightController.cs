using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightController : MonoBehaviour
{
    public enum LightMode
    {
        Static,
        FlickerLoop,
        IntervalFlicker,
        TimedTurnOff
    }
    [SerializeField] private LightMode mode = LightMode.Static;
    [SerializeField] private Color lightColor = Color.white;
    [SerializeField] private float baseIntensity = 1.0f;

    [SerializeField] private float minIntensity = 0.0f;
    [SerializeField] private float maxIntensity = 2.0f;
    [SerializeField] private float flickerSpeed = 0.08f;

    [SerializeField] private float timeOn = 3.0f;
    [SerializeField] private float timeOff = 2.0f;

    [SerializeField] private float timeToTurnOff = 5.0f;

    private Light _lightComponent;
    private Coroutine _activeRoutine;

    private void Awake()
    {
        _lightComponent = GetComponent<Light>();
    }

    private void OnEnable()
    {
        _lightComponent = GetComponent<Light>();
        ApplyBaseSettings();

        if (Application.isPlaying)
        {
            StartLightBehavior();
        }
    }

    private void ApplyBaseSettings()
    {
        if (_lightComponent != null)
        {
            _lightComponent.color = lightColor;
        }
    }
    
    public void StartLightBehavior()
    {
        if (_activeRoutine != null) StopCoroutine(_activeRoutine);

        switch (mode)
        {
            case LightMode.Static:
                _lightComponent.intensity = baseIntensity;
                _lightComponent.enabled = true;
                break;

            case LightMode.FlickerLoop:
                _activeRoutine = StartCoroutine(FlickerLoopRoutine());
                break;

            case LightMode.IntervalFlicker:
                _activeRoutine = StartCoroutine(IntervalFlickerRoutine());
                break;

            case LightMode.TimedTurnOff:
                _activeRoutine = StartCoroutine(TimedTurnOffRoutine());
                break;
        }
    }

    private IEnumerator FlickerLoopRoutine()
    {
        while (true)
        {
            _lightComponent.intensity = Random.Range(minIntensity, maxIntensity);
            yield return new WaitForSeconds(flickerSpeed);
        }
    }

    private IEnumerator IntervalFlickerRoutine()
    {
        while (true)
        {
            float timer = 0f;
            while (timer < timeOn)
            {
                _lightComponent.intensity = Random.Range(minIntensity, maxIntensity);
                yield return new WaitForSeconds(flickerSpeed);
                timer += flickerSpeed;
            }

            _lightComponent.intensity = 0f;
            yield return new WaitForSeconds(timeOff);
        }
    }

    private IEnumerator TimedTurnOffRoutine()
    {
        _lightComponent.intensity = baseIntensity;
        yield return new WaitForSeconds(timeToTurnOff);
        _lightComponent.intensity = 0f;
        _lightComponent.enabled = false;
    }
}