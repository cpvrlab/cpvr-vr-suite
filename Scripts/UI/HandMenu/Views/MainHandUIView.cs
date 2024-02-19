using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainHandUIView : View
{
    public override void Initialize(CanvasManager canvasManager)
    {
        var controller = new MainHandUIController(this, canvasManager);
        Controller = controller;
    }
}
