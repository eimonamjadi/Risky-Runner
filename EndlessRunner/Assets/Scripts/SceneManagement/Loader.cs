using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;

public class Loader : MonoBehaviour
{
    protected string PlayerUniqueName;
    protected string[] _playerNames = { "Liam",  "Olivia", "Noah", "Emma", "Oliver",   "Charlotte", "Elijah", "Amelia", "James", "Ava", "William", "Sophia",
                                        "Benjamin", "Isabella",
                                        "Lucas",    "Mia",
                                        "Henry",    "Evelyn",
                                        "Theodore", "Harper"};
    public static Loader Instance;
    public static int MaxSpeed = 28;
    public static int MinSpeed = 12;
    public static int DefaultSpeed = 15;
    protected int[] Speeds = new int[5] { 12, 15, 19, 23, 28 };
    [Tooltip("Range From 2 to 10")]
    public float SpeedChangeSensitivity { get; set; }
    public Slider SpeedChangeSensitivitySlider;
    public bool LevelInitialized = false;
    public static bool LoaderInitialized = false;
    protected CanvasGroup[] groupsToHide;
    protected CustomSettingData FixedLevelCustomData;
    [Tooltip("Return if the 'FixedLevelCustomData' has been reassigned")]
    protected bool CustomDataDirty { get; set; }
    [Tooltip("For FixedSpeed Levels, Each Speed is Played Multiple Times in Random Environments")]
    public bool LoadingFixedSpeedLevel { get; set; }
    /// for Metrics
    public int FixedSpeedIndex { get; set; }
    protected int NumRepetitionsForEachSpeed = 3;
    public int FixedSpeedRepetition { get; set; }

    [HideInInspector] public int SelfPacedTrialCount = 1;
    public int CurrReportNum { get; set; }

    public const string FixedLevel_URL = "https://docs.google.com/forms/u/0/d/e/1FAIpQLSeryWeUS15tvU-tC3HqBwz1XMsyBVuODEgUxF9Munq7K4afQA/formResponse";
    public const string SelfPaced_URL = "https://docs.google.com/forms/u/0/d/e/1FAIpQLSf2HfqObraJgtGqQS-BptpC0YDj6nxCvn6P7Izum3mo954_OA/formResponse";
        // new 5 
    private void Awake()
    {
        Initialization();
    }

    public void Initialization()
    {
        if (Instance == null)
        {
            Instance = this;
            LoaderInitialized = true;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance.gameObject != gameObject)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        /// This case only happens in UNITY_EDITOR
        if (!LevelInitialized)
        {
            InitializeNecessaryData();
            if (SceneManager.GetActiveScene().name == "FixedSpeedLevel")
            {
                LoadingFixedSpeedLevel = true;
                FixedSpeedIndex = 0;
            }
            else if (SceneManager.GetActiveScene().name == "SelfPacedLevel")
            {
                LoadingFixedSpeedLevel = false;
                SelfPacedTrialCount = 1;
            }
        }
        void ShowGUIDToPlayer()
        {
            // Check if we are running in a WebGL build and not in the Unity editor
#if UNITY_WEBGL && !UNITY_EDITOR
    string jsCommand = string.Format("window.prompt('Copy your unique session ID: Ctrl+C, Enter', '{0}');", PlayerUniqueName);
    Application.ExternalEval(jsCommand);
#endif
        }
        ShowGUIDToPlayer();
    }


    public void Load(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void LoadNewFixedSpeedLevel()
    {
        InitializeNecessaryData();
        LoadingFixedSpeedLevel = true;
        NewPlayerController newPlayerController = FindObjectOfType<NewPlayerController>();
        if (newPlayerController)
        {
            FixedLevelData newData = new FixedLevelData(CurrReportNum, FixedSpeedRepetition, Speeds[FixedSpeedIndex], ScoreDisplay.Score, FormatTime.Current.TimeElapsed,
                                                        newPlayerController.DistanceTraveled, TileManager.StraightTilesNum, TileManager.CurvedTilesNum,
                                                        Obstacles.ObstaclesGenerated, StumbleCount.Count, newPlayerController.TurningsPressed, newPlayerController.RandomHeadingsOccurred,
                                                        newPlayerController.RandomHeadingChance, ScoreDisplay.OffPathDuration);
            MetricsManager.WriteFixedLevelMetricsToCSV(newData);
            if (!Application.isEditor) WriteToGoogleForm(newData);
        }
        else if (SceneManager.GetActiveScene().name != "LevelSelection")
        {
            Debug.LogWarning("Missing a <NewPlayerController> class");
        }
        FixedSpeedRepetition++;
        if (FixedSpeedRepetition > NumRepetitionsForEachSpeed)
        {
            FixedSpeedIndex++;
            FixedSpeedRepetition = 1;
            if (FixedSpeedIndex == Speeds.Length)
            {
                // Stop Loading new Levels, spawn some texts
                return;
            }
        }

        // Spawn new levels normally
        StartCoroutine(LoadAsynchronously("FixedSpeedLevel"));
    }

    public void SetFixedLevelSetting(in CustomSettingData data)
    {
        // Check if FixedLevelCustomData has been reassigned
        if (FixedLevelCustomData == null) CustomDataDirty = true;
        else if (FixedLevelCustomData != data || FixedLevelCustomData.IsAssigned != data.IsAssigned || FixedLevelCustomData.NoiseLevel != data.NoiseLevel
                 || FixedLevelCustomData.MaxObstacles != data.MaxObstacles || FixedLevelCustomData.NumRepetitions != data.NumRepetitions) CustomDataDirty = true;
        else CustomDataDirty = false;

        FixedLevelCustomData = data;
    }

    public void LoadSelfPacedLevel()
    {
        InitializeNecessaryData();
        LoadingFixedSpeedLevel = false;
        NewPlayerController newPlayerController = FindObjectOfType<NewPlayerController>();
        if (newPlayerController)
        {
            SelfPacedData newData = new SelfPacedData(CurrReportNum, SelfPacedTrialCount++, newPlayerController.MaxMoveSpeed, newPlayerController.MinMoveSpeed, newPlayerController.NumSpeedChanges, 
                                                      newPlayerController.AvgMoveSpeed, newPlayerController.MedianMoveSpeed, ScoreDisplay.Score, FormatTime.Current.TimeElapsed,
                                                      newPlayerController.DistanceTraveled, TileManager.StraightTilesNum, TileManager.CurvedTilesNum,
                                                      Obstacles.ObstaclesGenerated, StumbleCount.Count, newPlayerController.TurningsPressed, newPlayerController.RandomHeadingsOccurred,
                                                      newPlayerController.AverageRandomHeadingChances, ScoreDisplay.OffPathDuration);
            MetricsManager.WriteSelfPacedMetricsToCSV(newData);
            WriteToGoogleForm(newData);
        }
        else if (SceneManager.GetActiveScene().name != "LevelSelection")
        {
            Debug.LogWarning("Missing a <NewPlayerController> class");
        }
        StartCoroutine(LoadAsynchronously("SelfPacedLevel"));
    }

    protected void InitializeNecessaryData()
    {
        if (!LevelInitialized)
        {
            LevelInitialized = true;
            CurrReportNum = GetCurrReportNumber(MetricsManager.GetFixedLevelFilePath());
            PlayerUniqueName = SystemInfo.deviceUniqueIdentifier + "_" + System.DateTime.UtcNow.ToString("yyyyMMddHHmmssffff");

            // Copy the PlayerUniqueName to the clipboard
            GUIUtility.systemCopyBuffer = PlayerUniqueName;

            Debug.Log("Your session ID has been copied to the clipboard: " + PlayerUniqueName);

            FixedSpeedRepetition = 1;
        }
    }


    protected IEnumerator LoadAsynchronously(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        groupsToHide = FindObjectsOfType<CanvasGroup>();
        foreach (CanvasGroup c in groupsToHide)
        {
            c.gameObject.SetActive(false);
        }
        yield return StartCoroutine(Test(operation));
    }

    protected IEnumerator Test(AsyncOperation op)
    {
        while(true)
        {
            if (op.isDone)
            {
                if (LoadingFixedSpeedLevel) CustomizeFixedSpeedLevels();
                else CustomizeSelfPacedLevels();
                if (LoadingFixedSpeedLevel && CustomSettingManager.HasInstance)
                {
                    CustomSettingManager.Current.Initialization();
                }
                if (TileManager.HasInstance && !TileManager.Current.bInitialized)
                {
                    if (LoadingFixedSpeedLevel && FixedSpeedIndex < Speeds.Length)
                    {
                        TileManager.Current.Initialization(Speeds[FixedSpeedIndex]);
                    }
                    else
                    {
                        TileManager.Current.Initialization(DefaultSpeed);
                    }
                }
                break;
            }
            else
            {
                yield return new WaitForEndOfFrame();
            }
        }
        yield return null;
    }

    protected int GetCurrReportNumber(string filePath)
    {
        if (File.Exists(filePath))
        {
            string[] contents = File.ReadAllLines(filePath);
            if (contents.Length > 1)
            {
                int firstCommaLocation = contents[contents.Length - 1].IndexOf(',');

                if (firstCommaLocation > 0)
                {
                    int result = 0;
                    if (int.TryParse(contents[contents.Length - 1].Substring(0, firstCommaLocation), out result))
                    {
                        return result + 1;
                    }
                    else
                    {
                        Debug.LogWarning("Something went wrong when computing the current report number.");
                    }
                }
            }
            return 1;
        }
        else return 1;
    }

    public int GetSpeed(int index)
    {
        index = Mathf.Clamp(index, 0, Speeds.Length - 1);
        return Speeds[index];
    }

    public void WriteToGoogleForm(FixedLevelData data)
    {
        StartCoroutine(WriteToGoogleFormCoroutine(data));
    }

    public void WriteToGoogleForm(SelfPacedData data)
    {
        StartCoroutine(WriteToGoogleFormCoroutine(data));
    }

    protected IEnumerator WriteToGoogleFormCoroutine(FixedLevelData data)
    {
        WWWForm form = new WWWForm();
        form.AddField("entry.2085101627", PlayerUniqueName);
        form.AddField("entry.827120889", data.LevelCount.ToString());
        form.AddField("entry.277768404", data.MovementSpeed.ToString());
        form.AddField("entry.697610182", data.Score.ToString());
        form.AddField("entry.1799481471", data.FinishTime.ToString("F2"));
        form.AddField("entry.249317089", data.NumStraightTiles.ToString());
        form.AddField("entry.453636336", data.NumCurvedTiles.ToString());
        form.AddField("entry.2107205770", data.TotalDistanceTraveled.ToString("F2"));
        form.AddField("entry.1996597316", data.TotalObstaclesPassed.ToString());
        form.AddField("entry.864373250", data.ObstaclesHit.ToString());
        form.AddField("entry.1706454395", (data.ObstacleMistakePercent * 100).ToString("F2"));
        form.AddField("entry.807324516", data.NumOfTurningKeysPressed.ToString());
        form.AddField("entry.167354389", data.TotalRandomHeadingsOccurred.ToString());
        form.AddField("entry.415354288", (data.RealRandomHeadingsPercent * 100).ToString("F2"));
        form.AddField("entry.985087491", (data.TheoreticalChanceForRandomHeadings * 100).ToString("F2"));
        form.AddField("entry.154528368", data.TotalDurationOffTrack.ToString("F2"));

        using (UnityWebRequest www = UnityWebRequest.Post(FixedLevel_URL, form))
        {
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success) Debug.LogWarning(www.error);
            else Debug.Log("Form Uploaded!");
        }
    }

    protected IEnumerator WriteToGoogleFormCoroutine(SelfPacedData data)
    {
        WWWForm form = new WWWForm();
        form.AddField("entry.1299316548", PlayerUniqueName);
        form.AddField("entry.2106089765", data.TrialNumber.ToString());
        form.AddField("entry.1141894605", data.MaxMovementSpeed.ToString());
        form.AddField("entry.7637182", data.MinMovementSpeed.ToString());
        form.AddField("entry.451707099", data.NumberSpeedChanges.ToString());
        form.AddField("entry.2146113110", data.AverageMovementSpeed.ToString("F2"));
        form.AddField("entry.1140623704", data.MedianMovementSpeed.ToString());
        form.AddField("entry.1682313270", data.Score.ToString());
        form.AddField("entry.350207672", data.FinishTime.ToString("F2"));
        form.AddField("entry.2092505315", data.NumStraightTiles.ToString());
        form.AddField("entry.1004828513", data.NumCurvedTiles.ToString());
        form.AddField("entry.1310381731", data.TotalDistanceTraveled.ToString("F2"));
        form.AddField("entry.1392864547", data.TotalObstaclesPassed.ToString());
        form.AddField("entry.490521960", data.ObstaclesHit.ToString());
        form.AddField("entry.615145773", (data.ObstacleMistakePercent * 100).ToString("F2"));
        form.AddField("entry.1142548569", data.NumOfTurningKeysPressed.ToString());
        form.AddField("entry.1477012241", data.TotalRandomHeadingsOccurred.ToString());
        form.AddField("entry.813373268", (data.RealRandomHeadingsPercent * 100).ToString("F2"));
        form.AddField("entry.1693640290", (data.AverageRandomHeadingChance * 100).ToString("F2"));
        form.AddField("entry.484499632", data.TotalDurationOffTrack.ToString("F2"));

        using (UnityWebRequest www = UnityWebRequest.Post(SelfPaced_URL, form))
        {
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success) Debug.LogWarning(www.error);
            else Debug.Log("Form Uploaded!");
        }
    }

    protected void CustomizeFixedSpeedLevels()
    {
        NewPlayerController controller = FindObjectOfType<NewPlayerController>();
        if (CustomDataDirty && FixedLevelCustomData != null)
        {
            if (controller != null)
            {
                // Don't modify 'controller.NoiseLevel' directly because selfPacedLevels still need the original value
                controller.RandomHeadingChance = FixedLevelCustomData.NoiseLevel;
            }
            Speeds = FixedLevelCustomData.Speeds;
            // Knuth shuffle algorithm :: courtesy of Wikipedia :)
            for (int i = 0; i < Speeds.Length; i++)
            {
                int tmp = Speeds[i];
                int rand = Random.Range(i, Speeds.Length);
                Speeds[i] = Speeds[rand];
                Speeds[rand] = tmp;
            }
            FixedSpeedIndex = 0;
            MaxSpeed = FindMax(Speeds);
            MinSpeed = FindMin(Speeds);
            TileManager.Current.MaxObstaclesPerLevel = FixedLevelCustomData.MaxObstacles;
            NumRepetitionsForEachSpeed = FixedLevelCustomData.NumRepetitions;
            SetRepetitions(1);
            CustomDataDirty = false;
        }
        else
        {
            if (controller != null)
            {
                // Don't modify 'controller.NoiseLevel' directly because selfPacedLevels still need the original value
                controller.RandomHeadingChance = FixedLevelCustomData.NoiseLevel;
            }
            // repetitive but has to be done to update the UI
            TileManager.Current.MaxObstaclesPerLevel = FixedLevelCustomData.MaxObstacles;
            SetRepetitions(FixedSpeedRepetition);
        }
    }

    protected void SetRepetitions(int num)
    {
        FixedSpeedRepetition = num;
        GameObject rep = GameObject.Find("Reps");
        Text RepsText;
        if (rep != null && rep.TryGetComponent(out RepsText))
        {
            RepsText.text = FixedSpeedRepetition.ToString() + " / " + NumRepetitionsForEachSpeed;
        }
    }

    protected void CustomizeSelfPacedLevels()
    {
        SetSliderValueAccordingtoSensitivity();
        NewPlayerController controller = FindObjectOfType<NewPlayerController>();
        CustomSettingData defaultData = CustomSettingManager.Current.GetDefaultData();
        if (defaultData != null)
        {
            MinSpeed = defaultData.Speeds[0];
            MaxSpeed = defaultData.Speeds[CustomSetting.speedCount-1];
            DefaultSpeed = defaultData.Speeds[1];
            if (controller != null) controller.SetOneSpeedLevel();
        }
        else
        {
            // fall back choice
            MaxSpeed = 28;
            MinSpeed = 12;
            DefaultSpeed = 15;
            if (controller != null) controller.SetOneSpeedLevel();
        }
    }

    protected void SetSliderValueAccordingtoSensitivity()
    {
        if (SpeedChangeSensitivitySlider == null) SpeedChangeSensitivitySlider = GameObject.FindGameObjectWithTag("SensitivitySlider").GetComponent<Slider>();
        if (SpeedChangeSensitivity != default)
        {
            SpeedChangeSensitivitySlider.value = Mathf.Clamp01((SpeedChangeSensitivity - 10f) / -8);
            NewPlayerController controller = FindObjectOfType<NewPlayerController>();
            if (controller != null) controller.RotationRate = SpeedChangeSensitivity;
        }
        else
        {
            LoadSensitivitySliderToPlayerController();
        }
    }

    public void LoadSensitivitySliderToPlayerController()
    {
        if (SpeedChangeSensitivitySlider == null) SpeedChangeSensitivitySlider = GameObject.FindGameObjectWithTag("SensitivitySlider").GetComponent<Slider>();
        if (SpeedChangeSensitivitySlider != null)
        {
            SpeedChangeSensitivity = -8 * SpeedChangeSensitivitySlider.value + 10f;
        }
        NewPlayerController controller = FindObjectOfType<NewPlayerController>();
        if (controller != null) controller.RotationRate = SpeedChangeSensitivity;
    }

    protected int FindMin(int[] list)
    {
        int min = int.MaxValue;
        foreach(int num in list)
        {
            if (num < min) min = num;
        }
        return min;
    }

    protected int FindMax(int[] list)
    {
        int max = int.MaxValue;
        foreach (int num in list)
        {
            if (num > max) max = num;
        }
        return max;
    }

}
