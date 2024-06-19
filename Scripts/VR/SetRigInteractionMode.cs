using UnityEngine;

public class SetRigInteractionMode : MonoBehaviour
{
    public void SetInteractionMode(int value)
    {
        if (RigManager.Instance != null &&
            RigManager.Instance.RigOrchestrator.TryGetInteractorManager<HandManager>(out var manager))
        {
            switch (value)
            {
                case 0:
                    manager.InteractionMode = InteractionMode.Ray;
                    break;
                case 1:
                    manager.InteractionMode = InteractionMode.Teleport;
                    break;
                case 2:
                    manager.InteractionMode = InteractionMode.None;
                    break;
            }
        }
    }
}
