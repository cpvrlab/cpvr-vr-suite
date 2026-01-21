using System;

public interface ISceneHandler
{
    event Action SceneChangeStarted;
    event Action SceneChangeCompleted;

    void ChangeScene(int index);
}
