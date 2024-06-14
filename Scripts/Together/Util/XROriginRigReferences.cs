using System;
using UnityEngine;
using UnityEngine.XR.Hands.Samples.VisualizerSample;

namespace Util {
    /// <summary>
    /// Used by the XRRig to give reference to the hands and the head as a Singleton
    /// </summary>
    public class XROriginRigReferences : Singleton<XROriginRigReferences> {
        [NonSerialized] public Transform Origin;
        [SerializeField] public Transform head;
        [SerializeField] public SkinnedMeshRenderer leftHand;
        [SerializeField] public SkinnedMeshRenderer rightHand;
        [SerializeField] public HandVisualizer handVisualizer;

        protected override void Awake() {
            base.Awake();
            Origin = transform;
        }
    }
}
