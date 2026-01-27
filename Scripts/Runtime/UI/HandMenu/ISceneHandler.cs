using System;

namespace cpvr_vr_suite.Scripts.Runtime.UI
{
    public interface ISceneHandler
    {
        event Action SceneChangeStarted;
        event Action SceneChangeCompleted;

        void ChangeScene(int index);
    }
}
