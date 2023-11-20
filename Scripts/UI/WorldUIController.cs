using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorldUIController : MonoBehaviour
{
    [Range(1f, 10f)] public float autoTurnOffDistance = 3f;
    public Button button;
    public TMP_Text title;
    public TMP_Text content;
    public string titleText;
    [TextArea] public string contentText;
    private Transform _camera;

    void Start()
    {
        _camera = Camera.main.transform;
        if (title != null)
            title.text = titleText;
        if (content != null)
            content.text = contentText;
        if (button != null)
            button.onClick.AddListener(() => { gameObject.SetActive(false); });
    }

    void Update()
    {
        if (_camera != null)
        {
            var direction = -1 * (_camera.position - transform.position);
            transform.rotation = Quaternion.LookRotation(direction);

            if (direction.magnitude > autoTurnOffDistance)
                gameObject.SetActive(false);
        }
    }
}
