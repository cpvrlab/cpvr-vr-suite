using System;

public interface ISceneHandler
{
    public event Action SceneChangeStarted;
    public event Action SceneChangeCompleted;

    public void ChangeScene(int index);
}
