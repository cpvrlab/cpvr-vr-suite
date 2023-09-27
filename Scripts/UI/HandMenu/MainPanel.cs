using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPanel : MenuPanel
{
    public void OnChangePanel(int index) => _handMenuController.OpenPanel(index);
}
