using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace cpvrlab_vr_suite.Scripts.UI
{
    public class MenuController : MonoBehaviour
    {
        [Header("UI Elements")] 
        [SerializeField] private List<GameObject> panels;

        private Animator _animator;

        private void Awake() => _animator = GetComponent<Animator>();

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
        
        public async void CloseMenu()
        {
            OpenPanel(0);
            _animator.Play("MenuClose");
            await Task.Delay(250);
            gameObject.SetActive(false);
        }
    }
}
