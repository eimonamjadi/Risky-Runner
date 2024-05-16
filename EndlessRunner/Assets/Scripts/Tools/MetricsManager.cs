using UnityEngine;
using System.IO;

/// <summary>
/// This class writes data to a csv file
/// </summary>
public static class MetricsManager
{

    // Generate the report that will be saved out to a file.
    public static void WriteFixedLevelMetricsToCSV(FixedLevelData data)
    {
        string totalReport = "";
        //totalReport += ConvertMetricsToStringRepresentation();
        string FilePath = GetFixedLevelFilePath();

        if (!File.Exists(FilePath))
        {
            totalReport += "A New Report Generated On " + System.DateTime.Now + "\n";
            totalReport += "Report Number, Level Count, Movement Speed, Score, Finish Time(s), StraightTiles Number, CurvedTiles Number," +
                             "Total Distance Traveled, Total Obstacles Passed, Obstacles Hit, Obstacle Mistake Percent(%), Number of Turning Keys Pressed," +
                             "Total Random Headings, Chance for Random Headings(%), Real Random Headings Percent(%), Time Off-track(s)\n";
        }
        
        totalReport +=  data.RecordNumber.ToString() + "," + data.LevelCount.ToString() + "," + data.MovementSpeed.ToString() + "," + data.Score.ToString() + "," + data.FinishTime.ToString("F2") +
                        "," + data.NumStraightTiles.ToString() + "," + data.NumCurvedTiles.ToString() + "," + data.TotalDistanceTraveled.ToString("F2") + "," + data.TotalObstaclesPassed.ToString() +
                        "," + data.ObstaclesHit.ToString() + "," + (data.ObstacleMistakePercent * 100).ToString("F2") + "," + data.NumOfTurningKeysPressed.ToString() + "," + data.TotalRandomHeadingsOccurred.ToString() +
                        "," + (data.TheoreticalChanceForRandomHeadings * 100).ToString("F2") + "," + (data.RealRandomHeadingsPercent * 100).ToString("F2") + "," + data.TotalDurationOffTrack.ToString("F2") + "\n";
        totalReport = totalReport.Replace("\n", System.Environment.NewLine);

#if !UNITY_WEBPLAYER
        if (!Application.isMobilePlatform)
        {
            File.AppendAllText(FilePath, totalReport);
        }
#endif
    }

    public static void WriteSelfPacedMetricsToCSV(SelfPacedData data)
    {
        string totalReport = "";
        //totalReport += ConvertMetricsToStringRepresentation();
        string FilePath = GetSelfPacedFilePath();

        if (!File.Exists(FilePath))
        {
            totalReport += "A New Report Generated On " + System.DateTime.Now + "\n";
            totalReport += "Report Number, Trial Number, Max Move Speed, Min Move Speed, Number Speed Changes, Average Move Speed, Median Move Speed, Score, Finish Time(s), StraightTiles Number, CurvedTiles Number," +
                             "Total Distance Traveled, Total Obstacles Passed, Obstacles Hit, Obstacle Mistake Percent(%), Number of Turning Keys Pressed," +
                             "Total Random Headings, Chance for Random Headings(%), Real Random Headings Percent(%), Time Off-track(s)\n";
        }

        totalReport +=  data.RecordNumber.ToString() + "," + data.TrialNumber.ToString() + "," + data.MaxMovementSpeed.ToString() + "," + data.MinMovementSpeed.ToString() + "," + data.NumberSpeedChanges.ToString() +
                        "," + data.AverageMovementSpeed.ToString("F2") + "," + data.MedianMovementSpeed.ToString() + "," + data.Score.ToString() + "," + data.FinishTime.ToString("F2") +
                        "," + data.NumStraightTiles.ToString() + "," + data.NumCurvedTiles.ToString() + "," + data.TotalDistanceTraveled.ToString("F2") + "," + data.TotalObstaclesPassed.ToString() +
                        "," + data.ObstaclesHit.ToString() + "," + (data.ObstacleMistakePercent * 100).ToString("F2") + "," + data.NumOfTurningKeysPressed.ToString() + "," + data.TotalRandomHeadingsOccurred.ToString() +
                        "," + (data.AverageRandomHeadingChance * 100).ToString("F2") + "," + (data.RealRandomHeadingsPercent * 100).ToString("F2") + "," + data.TotalDurationOffTrack.ToString("F2") + "\n";
        totalReport = totalReport.Replace("\n", System.Environment.NewLine);
#if !UNITY_WEBPLAYER
        if (!Application.isMobilePlatform)
        {
            File.AppendAllText(FilePath, totalReport);
        }
        
#endif
    }

    public static string GetFixedLevelFilePath()
    {
        return "FixedLevelUserData.csv";
    }

    public static string GetSelfPacedFilePath()
    {
        return "SelfPacedUserData.csv";
    }
}

[System.Serializable]
public class FixedLevelData
{
    public FixedLevelData(int playNum, int levelCount, int moveSpeed, int score, float finishTime, float distanceTraveled, 
                          int numStraightTiles, int numCurvedTiles,
                          int obstaclesPassed, int obstaclesHit, int numTurningKeysPressed, int totalRandomHeadingsOccurred,
                          float theoreticalChanceForRandomHeadings, float durationOffTrack)
    {
        RecordNumber = playNum;
        LevelCount = levelCount;
        MovementSpeed = moveSpeed;
        Score = score;
        FinishTime = finishTime;
        NumStraightTiles = numStraightTiles;
        NumCurvedTiles = numCurvedTiles;
        TotalDistanceTraveled = distanceTraveled;
        TotalObstaclesPassed = obstaclesPassed;
        ObstaclesHit = obstaclesHit;
        ObstacleMistakePercent = 1.0f * ObstaclesHit / TotalObstaclesPassed;
        NumOfTurningKeysPressed = numTurningKeysPressed;
        TotalRandomHeadingsOccurred = totalRandomHeadingsOccurred;
        TheoreticalChanceForRandomHeadings = theoreticalChanceForRandomHeadings;
        RealRandomHeadingsPercent = 1.0f * totalRandomHeadingsOccurred / numTurningKeysPressed;
        TotalDurationOffTrack = durationOffTrack;
    }

    /// <summary>
    /// Each time the application is run, the count increases by [ONE].
    /// </summary>
    public int RecordNumber;
    /// <summary>
    /// For FixedSpeed Levels, there are about [FIVE] levels in total, this number is thus ranged from [ONE] to [FIVE].
    /// </summary>
    public int LevelCount;
    /// <summary>
    /// Each FixedSpeed Level, the movement speed is fixed.
    /// </summary>
    public int MovementSpeed;
    public int Score;
    public float FinishTime;
    public int NumStraightTiles;
    public int NumCurvedTiles;
    public float TotalDistanceTraveled;
    /// <summary>
    /// The number of obstacles generated in the entire level.
    /// </summary>
    public int TotalObstaclesPassed;
    public int ObstaclesHit;
    public float ObstacleMistakePercent;
    public int NumOfTurningKeysPressed;
    public int TotalRandomHeadingsOccurred;
    public float TheoreticalChanceForRandomHeadings;
    public float RealRandomHeadingsPercent;
    public float TotalDurationOffTrack;
}


[System.Serializable]
public class SelfPacedData
{
    public SelfPacedData(int playNum, int levelCount, int maxMoveSpeed, int minMoveSpeed, int numSpeedChanges, float avgMovementSpeed, int mediumMovementSpeed,
                         int score, float finishTime, float distanceTraveled, int numStraightTiles, int numCurvedTiles,
                         int obstaclesPassed, int obstaclesHit, int numTurningKeysPressed, int totalRandomHeadingsOccurred,
                         float avgRandomHeadingChance, float durationOffTrack)
    {
        RecordNumber = playNum;
        TrialNumber = levelCount;
        MaxMovementSpeed = maxMoveSpeed;
        MinMovementSpeed = minMoveSpeed;
        NumberSpeedChanges = numSpeedChanges;
        AverageMovementSpeed = avgMovementSpeed;
        MedianMovementSpeed = mediumMovementSpeed;
        Score = score;
        FinishTime = finishTime;
        NumStraightTiles = numStraightTiles;
        NumCurvedTiles = numCurvedTiles;
        TotalDistanceTraveled = distanceTraveled;
        TotalObstaclesPassed = obstaclesPassed;
        ObstaclesHit = obstaclesHit;
        ObstacleMistakePercent = 1.0f * ObstaclesHit / TotalObstaclesPassed;
        NumOfTurningKeysPressed = numTurningKeysPressed;
        TotalRandomHeadingsOccurred = totalRandomHeadingsOccurred;
        AverageRandomHeadingChance = avgRandomHeadingChance;
        RealRandomHeadingsPercent = 1.0f * totalRandomHeadingsOccurred / numTurningKeysPressed;
        TotalDurationOffTrack = durationOffTrack;
    }

    /// <summary>
    /// Each time the application is run, the count increases by [ONE].
    /// </summary>
    public int RecordNumber;
    /// <summary>
    /// Each time the game is played, this number increases by [ONE].
    /// </summary>
    public int TrialNumber;
    /// <summary>
    /// Each FixedSpeed Level, the movement speed is fixed.
    /// </summary>
    public int MaxMovementSpeed;
    public int MinMovementSpeed;
    public int NumberSpeedChanges;
    public float AverageMovementSpeed;
    public int MedianMovementSpeed;
    public int Score;
    public float FinishTime;
    public int NumStraightTiles;
    public int NumCurvedTiles;
    public float TotalDistanceTraveled;
    /// <summary>
    /// The number of obstacles generated in the entire level.
    /// </summary>
    public int TotalObstaclesPassed;
    public int ObstaclesHit;
    public float ObstacleMistakePercent;
    public int NumOfTurningKeysPressed;
    public int TotalRandomHeadingsOccurred;
    public float AverageRandomHeadingChance;
    public float RealRandomHeadingsPercent;
    public float TotalDurationOffTrack;
}
