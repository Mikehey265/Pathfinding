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

    [Header("Checkboxes")] 
    [SerializeField] private Toggle gridCoordinatesToggle;

    [Header("Buttons")] 
    [SerializeField] private Button startButton;
    [SerializeField] private Button resetButton;

    [Header("Warning Text")] 
    [SerializeField] private TextMeshProUGUI warningText;
    
    private int defaultGridWidth = 5;
    private int defaultGridHeight = 5;
    private float defaultCellSpacing = 1.0f;

    private bool bIsSimulationRunning;
    private bool areCoordinatesVisible = true;

    private void Start()
    {
        InitializeDefaultValues();
        
        gridWidthSlider.onValueChanged.AddListener(delegate { OnGridSettingsChanged(); });
        gridHeightSlider.onValueChanged.AddListener(delegate { OnGridSettingsChanged(); });
        cellSpacingSlider.onValueChanged.AddListener(delegate { OnGridSettingsChanged(); });
        randomizeSlider.onValueChanged.AddListener(delegate { RandomizeGrid(); });
        
        OnGridSettingsChanged();
    }
    
    private void InitializeDefaultValues()
    {
        gridWidthSlider.value = defaultGridWidth;
        gridHeightSlider.value = defaultGridHeight;
        cellSpacingSlider.value = defaultCellSpacing;
        randomizeSlider.value = 1.0f;

        gridCoordinatesToggle.isOn = areCoordinatesVisible;
        gridCoordinatesToggle.onValueChanged.AddListener(OnToggleCoordinates);
        
        resetButton.onClick.AddListener(ResetGrid);
        startButton.onClick.AddListener(StartSimulation);
        
        warningText.gameObject.SetActive(false);
    }

    private void OnToggleCoordinates(bool isOn)
    {
        areCoordinatesVisible = isOn;
        gridCoordinatesToggle.isOn = areCoordinatesVisible;
        gridManager.SetGridCoordinatesVisibility(isOn);
    }

    private void OnGridSettingsChanged()
    {
        UpdateUIText();
        UpdateGrid();
    }

    private void StartSimulation()
    {
        if (!gridManager.IsStartAndGoalSelected())
        {
            DisplayWarning("Select starting and goal cell before running the simulation!");
            return;
        }
        
        warningText.gameObject.SetActive(false);
        bIsSimulationRunning = true;
        DisableAllInteractions();
        
        gridManager.SpawnPlayer();
        
        if (!gridManager.ExecutePathfinding())
        {
            DisplayWarning("No viable path found");
            EnableAllInteractions();
            bIsSimulationRunning = false;
        }
    }
    
    private void UpdateUIText()
    {
        gridWidthText.text = Mathf.RoundToInt(gridWidthSlider.value).ToString();
        gridHeightText.text = Mathf.RoundToInt(gridHeightSlider.value).ToString();
        cellSpacingText.text = cellSpacingSlider.value.ToString("F1");
        randomizeText.text = randomizeSlider.value.ToString("F1");
    }

    private void DisplayWarning(string text)
    {
        warningText.text = text;
        warningText.gameObject.SetActive(true);
    }

    private void UpdateGrid()
    {
        if(bIsSimulationRunning) return;
        
        int width = Mathf.RoundToInt(gridWidthSlider.value);
        int height = Mathf.RoundToInt(gridHeightSlider.value);
        float spacing = cellSpacingSlider.value;
        
        gridManager.SetGridParameters(width, height, spacing);
        
        RandomizeGrid();
    }

    private void RandomizeGrid()
    {
        if(bIsSimulationRunning) return;
        
        float walkablePercentage = randomizeSlider.value;
        gridManager.RandomizeGrid(walkablePercentage);
        UpdateUIText();
        OnToggleCoordinates(areCoordinatesVisible);
    }
    
    private void ResetGrid()
    {
        gridWidthSlider.value = defaultGridWidth;
        gridHeightSlider.value = defaultGridHeight;
        cellSpacingSlider.value = defaultCellSpacing;
        randomizeSlider.value = 1.0f;
        
        EnableAllInteractions();
        OnGridSettingsChanged();
        OnToggleCoordinates(true);
        
        warningText.gameObject.SetActive(false);
        
        gridManager.RemovePlayer();
    }

    private void DisableAllInteractions()
    {
        DisableSliders();
        gridManager.DisableCellInteraction();
        startButton.interactable = false;
    }

    private void EnableAllInteractions()
    {
        EnableSliders();
        gridManager.EnableCellInteraction();
        bIsSimulationRunning = false;
        startButton.interactable = true;
    }

    private void DisableSliders()
    {
        gridWidthSlider.interactable = false;
        gridHeightSlider.interactable = false;
        cellSpacingSlider.interactable = false;
        randomizeSlider.interactable = false;
    }
    
    private void EnableSliders()
    {
        gridWidthSlider.interactable = true;
        gridHeightSlider.interactable = true;
        cellSpacingSlider.interactable = true;
        randomizeSlider.interactable = true;
    }
}
