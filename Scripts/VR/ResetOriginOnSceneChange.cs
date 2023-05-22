using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetOriginOnSceneChange : MonoBehaviour
{
    private void OnEnable() =>
        SceneManager.activeSceneChanged += (_, _) => transform.position = Vector3.zero;
}
