using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResourceCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI counterText;
    private EntityManager entityManager;
    private EntityQuery counterQuery;

    private void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        counterQuery = entityManager.CreateEntityQuery(
            ComponentType.ReadOnly<ResourceCount>()
        );

        if (counterText == null)
            CreateCounterUI();
    }

    private void CreateCounterUI()
    {
        var canvasGO = new GameObject("ResourceCanvas");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        var textGO = new GameObject("ResourceText");
        textGO.transform.SetParent(canvasGO.transform);

        counterText = textGO.AddComponent<TextMeshProUGUI>();
        counterText.fontSize = 36;
        counterText.color = Color.white;
        counterText.alignment = TextAlignmentOptions.TopLeft;
        counterText.text = "Resources: 0";

        var rect = counterText.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(0, 1);
        rect.pivot = new Vector2(0, 1);
        rect.anchoredPosition = new Vector2(20, -20);
        rect.sizeDelta = new Vector2(300, 100);
    }

    private void Update()
    {
        if (counterQuery.IsEmptyIgnoreFilter)
        {
            counterText.text = "Resources: 0";
            return;
        }

        var counter = counterQuery.GetSingleton<ResourceCount>();
        counterText.text = $"<b>RESOURCES</b>\n{counter.Value}";
    }
}