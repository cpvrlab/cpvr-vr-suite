using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace cpvr_vr_suite.Scripts.UI
{
    public class MenuController : MonoBehaviour
    {
        [SerializeField] private bool _showMenuOnStart;
        [SerializeField] private Transform xrOrigin;
        
        [Header("UI Elements")] 
        [SerializeField] private List<GameObject> panels;

        public UnityEvent onMenuOpened;
        public UnityEvent onMenuClosed;

        private Animator _animator;
        private Transform _headTransform;
        private bool _inMenu;
        private float _menuLastOpened = -1f;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            
            _headTransform = xrOrigin.transform.GetChild(0).GetChild(0);
            if (!_headTransform.CompareTag("MainCamera"))
                Debug.LogError("Camera is not positioned setup or placed correctly!");
        }

        private void Start()
        {
            if (_showMenuOnStart)
                OpenMenu();
            else
                CloseMenu();
        }

        private void OnEnable() => SceneManager.activeSceneChanged += (_, _) => {
            if (!_showMenuOnStart) return;
            OpenMenu();
        };

        public void OpenPanel(int index)
        {
            CloseAllPanels();
            try
            {
                panels[index].SetActive(true);
            }
            catch (ArgumentOutOfRangeException e)
            {
                panels.First().SetActive(true);
                throw new IndexOutOfRangeException(
                    $"Panel index {index} not within bounds of panel list with length {panels.Count}.\n{e.Message}");
            }
        }

        public void OpenMainPanel()
        {
            CloseAllPanels();
            OpenPanel(0);
        }

        private void CloseAllPanels()
        {
            foreach (var panel in panels) panel.SetActive(false);
        }

        private void UpdatePositionAndRotation()
        {
            var headPosition = _headTransform.position;
            var origin = new Vector3(headPosition.x, xrOrigin.transform.position.y, headPosition.z);
            var position = origin + 1.5f * Vector3.ProjectOnPlane(_headTransform.forward, Vector3.up).normalized + new Vector3(0,1,0);
            var rotation = Quaternion.Euler(0, _headTransform.rotation.eulerAngles.y, 0);
            transform.SetPositionAndRotation(position, rotation);
        }
        
        private void ResetPositionAndRotation()
        {
            var position = Vector3.zero + 1.5f * Vector3.ProjectOnPlane(_headTransform.forward, Vector3.up).normalized + new Vector3(0,1,0);
            var rotation = Quaternion.Euler(0, _headTransform.rotation.eulerAngles.y, 0);
            transform.SetPositionAndRotation(position, rotation);
        }
        
        public async void ToggleMenu()
        {
            if (Time.time - _menuLastOpened < 0.25f) return;
            _menuLastOpened = Time.time;
            if (_inMenu)
            {
                _animator.Play("MenuClose");
                await Task.Delay(250);
                CloseMenu();
            }
            else
            {
                OpenMenu();
                _animator.Play("MenuOpen");
                await Task.Delay(250);
            }
        }

        private void OpenMenu()
        {
            OpenPanel(0);
            UpdatePositionAndRotation();
            onMenuOpened?.Invoke();
            _inMenu = true;
        }

        private void CloseMenu()
        {
            OpenPanel(0);
            onMenuClosed?.Invoke();
            CloseAllPanels();
            _inMenu = false;
        }
    }
}
