using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;

namespace cpvr_vr_suite.Scripts.Runtime.Core
{
    [Serializable]
    public class InteractorControl
    {
        public GameObject interactor;
        public XRInputButtonReader controlAction;
        public bool IsActive => !blocked && controlAction.ReadIsPerformed();
        public bool blocked;
    }
}
