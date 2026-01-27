using System;
using System.Collections.Generic;
using UnityEngine;

namespace cpvr_vr_suite.Scripts.Runtime.Core
{
    /// <summary>
    /// Lists the different possible parts of an avatar.
    /// </summary>
    [Serializable]
    public class AvatarDefinition {
        public List<GameObject> hairs = new();
        public List<GameObject> heads = new();
        public List<GameObject> bodys = new();
        public List<GameObject> pants = new();
        public List<GameObject> shoes = new();

        public void DisableAll() {
            foreach(GameObject hair in hairs) {
                hair.SetActive(false);
            }

            foreach(GameObject head in heads) {
                head.SetActive(false);
            }

            foreach(GameObject body in bodys) {
                body.SetActive(false);
            }

            foreach(GameObject pant in pants) {
                pant.SetActive(false);
            }

            foreach(GameObject shoe in shoes) {
                shoe.SetActive(false);
            }
        }
    }
}