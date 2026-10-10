using System.Collections;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
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

    [Header("Configuración General")]
    [SerializeField] private LightMode mode = LightMode.Static;
    [SerializeField] private Color lightColor = Color.white;
    [SerializeField] private float baseIntensity = 1.0f;

    [Header("Ajustes de Parpadeo (Flicker)")]
    [SerializeField] private float minIntensity = 0.0f;
    [SerializeField] private float maxIntensity = 2.0f;
    [SerializeField] private float flickerSpeed = 0.08f;

    [Header("Ajustes de Intervalo")]
    [SerializeField] private float timeOn = 3.0f;
    [SerializeField] private float timeOff = 2.0f;

    [Header("Ajustes de Apagado por Tiempo")]
    [SerializeField] private float timeToTurnOff = 5.0f;

    private Light _lightComponent;
    private Coroutine _activeRoutine;
    private float _lastFlickerTime;

    private void Awake()
    {
        _lightComponent = GetComponent<Light>();
    }

    private void OnEnable()
    {
        _lightComponent = GetComponent<Light>();
        ApplyBaseSettings();

#if UNITY_EDITOR
        // Nos suscribimos al evento de actualización del Editor
        if (!Application.isPlaying)
        {
            EditorApplication.update -= EditorUpdate;
            EditorApplication.update += EditorUpdate;
        }
#endif

        if (Application.isPlaying)
        {
            StartLightBehavior();
        }
    }

    private void OnDisable()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            EditorApplication.update -= EditorUpdate;
        }
#endif
    }

    private void ApplyBaseSettings()
    {
        if (_lightComponent != null)
        {
            _lightComponent.color = lightColor;
        }
    }

    // --- LÓGICA PARA EL EDITOR (SIN PLAY MODE) ---
    private void EditorUpdate()
    {
        if (Application.isPlaying || _lightComponent == null) return;

        if (mode == LightMode.FlickerLoop || mode == LightMode.IntervalFlicker)
        {
            if (Time.realtimeSinceStartup - _lastFlickerTime >= flickerSpeed)
            {
                _lastFlickerTime = Time.realtimeSinceStartup;
                _lightComponent.intensity = Random.Range(minIntensity, maxIntensity);
                
                // Forzamos a la ventana Scene a redibujarse para ver el cambio de luz
                SceneView.RepaintAll();
            }
        }
        else if (mode == LightMode.Static)
        {
            _lightComponent.intensity = baseIntensity;
        }
    }

    // --- LÓGICA EN TIEMPO DE EJECUCIÓN (PLAY MODE) ---
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