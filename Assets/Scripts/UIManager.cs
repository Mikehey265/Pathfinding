using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    
    [Header("Sliders")]
    [SerializeField] private Slider gridWidthSlider;
    [SerializeField] private Slider gridHeightSlider;
    [SerializeField] private Slider cellSpacingSlider;
    [SerializeField] private Slider randomizeSlider;

    [Header("Text Values")]
    [SerializeField] private TextMeshProUGUI gridWidthText;
    [SerializeField] private TextMeshProUGUI gridHeightText;
    [SerializeField] private TextMeshProUGUI cellSpacingText;
    [SerializeField] private TextMeshProUGUI randomizeText;

    [Header("Buttons")]
    [SerializeField] private Button resetButton;
    
    private int defaultGridWidth = 5;
    private int defaultGridHeight = 5;
    private float defaultCellSpacing = 1.0f;

    private void Start()
    {
        gridWidthSlider.value = defaultGridWidth;
        gridHeightSlider.value = defaultGridHeight;
        cellSpacingSlider.value = defaultCellSpacing;
        randomizeSlider.value = 1.0f;
        
        gridWidthSlider.onValueChanged.AddListener(delegate { OnGridSettingsChanged(); });
        gridHeightSlider.onValueChanged.AddListener(delegate { OnGridSettingsChanged(); });
        cellSpacingSlider.onValueChanged.AddListener(delegate { OnGridSettingsChanged(); });
        
        randomizeSlider.onValueChanged.AddListener(delegate { RandomizeGrid(); });
        
        resetButton.onClick.AddListener(ResetGrid);
        OnGridSettingsChanged();
    }

    private void OnGridSettingsChanged()
    {
        UpdateUIText();
        UpdateGrid();
    }
    
    private void UpdateUIText()
    {
        gridWidthText.text = Mathf.RoundToInt(gridWidthSlider.value).ToString();
        gridHeightText.text = Mathf.RoundToInt(gridHeightSlider.value).ToString();
        cellSpacingText.text = cellSpacingSlider.value.ToString("F1");
        randomizeText.text = randomizeSlider.value.ToString("F1");
    }

    private void UpdateGrid()
    {
        int width = Mathf.RoundToInt(gridWidthSlider.value);
        int height = Mathf.RoundToInt(gridHeightSlider.value);
        float spacing = cellSpacingSlider.value;
        
        gridManager.SetGridParameters(width, height, spacing);
    }

    private void RandomizeGrid()
    {
        float walkablePercentage = randomizeSlider.value;
        gridManager.RandomizeGrid(walkablePercentage);
        UpdateUIText();
    }
    
    private void ResetGrid()
    {
        gridWidthSlider.value = defaultGridWidth;
        gridHeightSlider.value = defaultGridHeight;
        cellSpacingSlider.value = defaultCellSpacing;
        randomizeSlider.value = 1.0f;
        
        OnGridSettingsChanged();
    }
}
