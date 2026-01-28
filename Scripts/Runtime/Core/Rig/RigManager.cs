using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cpvr_vr_suite.Scripts.Runtime.Util;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace cpvr_vr_suite.Scripts.Runtime.Core
{
    public class RigManager : Singleton<RigManager>
    {
        public event Action OnHeightCalibrationStarted;
        public event Action<float> OnHeightCalibrationEnded;

        public bool HeightCalculated { get; private set; }
        public float Height { get; private set; }

        [SerializeField] List<MonoBehaviour> m_servicesToRegister = new();
        [SerializeField] Image m_fadeImage;

        bool m_isCalibrating;
        readonly Dictionary<Type, object> m_services = new();

        protected override void Awake()
        {
            foreach (var behaviour in m_servicesToRegister)
            {
                RegisterLocal(behaviour);
                Debug.Log($"Service {behaviour.GetType()} registered");
            }

            base.Awake();
        }

        void Start()
        {
            CalibrateHeight();

            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                var scenename = System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i)).ToString();
                if (scenename.Contains("Bootstrap", StringComparison.OrdinalIgnoreCase))
                    continue;
                SceneManager.LoadSceneAsync(i);
                break;
            }
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            var seen = new HashSet<Type>();

            foreach (var service in m_servicesToRegister)
            {
                if (service == null)
                    continue;

                var type = service.GetType();

                if (!seen.Add(type))
                    Debug.LogError($"Duplicate rig service of type {type.Name} on RigManager.", this);
            }
        }
#endif

        public async Task Fade(Color endColor, float duration = 1f)
        {
            var startColor = m_fadeImage.color;
            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                m_fadeImage.color = Color.Lerp(startColor, endColor, t);
                await Task.Yield();
            }
            m_fadeImage.color = endColor;
        }

        public async void CalibrateHeight()
        {
            if (m_isCalibrating) return;
            if (!TryGet<XROrigin>(out var origin)) return;

            //Debug.Log("Started calibration.");
            OnHeightCalibrationStarted?.Invoke();
            m_isCalibrating = true;

            var cameraTransform = origin.Camera.transform;
            var heightData = new float[75];
            for (int i = 0; i < heightData.Length; i++)
            {
                heightData[i] = origin.transform.InverseTransformPoint(cameraTransform.position).y + .1f;
                await Task.Delay(10);
            }

            Height = heightData.Average();
            HeightCalculated = true;
            m_isCalibrating = false;

            //Debug.Log("Finished calibration.");
            OnHeightCalibrationEnded?.Invoke(Height);
        }

        public void Register<T>(T service) where T : class
        {
            var type = typeof(T);

            if (!m_services.TryAdd(type, service))
            {
                Debug.LogWarning($"Service {type.Name} already registered. Overwriting.");
                m_services[type] = service;
            }
        }

        void RegisterLocal(MonoBehaviour service)
        {
            var type = service.GetType();

            if (!m_services.TryAdd(type, service))
            {
                Debug.LogWarning($"Service {type.Name} already registered. Overwriting.");
                m_services[type] = service;
            }
        }

        public void Unregister<T>(T service) where T : class
        {
            var type = typeof(T);

            if (m_services.TryGetValue(type, out var registered) &&
                ReferenceEquals(registered, service))
            {
                m_services.Remove(type);
            }
        }

        public T Get<T>() where T : class
        {
            if (m_services.TryGetValue(typeof(T), out var service))
                return service as T;

            Debug.LogError($"Service {typeof(T).Name} not registered.");
            return null;
        }

        public bool TryGet<T>(out T service) where T : class
        {
            if (m_services.TryGetValue(typeof(T), out var obj))
            {
                service = obj as T;
                return true;
            }

            service = null;
            return false;
        }
    }
}
