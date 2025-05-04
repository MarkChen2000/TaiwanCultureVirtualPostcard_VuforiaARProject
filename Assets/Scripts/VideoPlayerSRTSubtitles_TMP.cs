using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Video;
using TMPro;
using UnityEngine.Networking;
using System.Collections;

public class VideoPlayerSRTSubtitles_TMP : Singleton<VideoPlayerSRTSubtitles_TMP>   
{
    [Header("References")]
    public VideoPlayer videoPlayer;         // 指到你的VideoPlayer
    public TMP_Text subtitleText;    // 指到你的TMP Text
    
    string srtFileName;               // 檔案名稱，例如 "subtitle.srt"

    private List<SubtitleEntry> subtitles = new List<SubtitleEntry>();
    private int currentSubtitleIndex = 0;

    private void Start()
    {
        // LoadSRTFile();
        subtitleText.text = "";
    }

    private void Update()
    {
        if (videoPlayer.isPlaying)
        {
            UpdateSubtitle(videoPlayer.time);
        }
    }

    public void SetSRTFileName(string fileName)
    {
        srtFileName = fileName;
        LoadSRTFile();
    }

    void LoadSRTFile()
    {
        StartCoroutine(LoadSRTCoroutine());
    }

    IEnumerator LoadSRTCoroutine()
    {
        // reset
        subtitles.Clear();
        currentSubtitleIndex = 0;
        subtitleText.text = "";

        string path = Path.Combine(Application.streamingAssetsPath, srtFileName) + ".srt";

        // 只能在Editor中執行 File.ReadAllLines ?
        /*if (!File.Exists(path))
        {
            Debug.LogError("SRT file not found: " + path);
            return;
        }

        string[] lines = File.ReadAllLines(path);
        int i = 0;*/

        //
        // GPT提出之解方。
        // 
        UnityWebRequest request = UnityWebRequest.Get(path);
        yield return request.SendWebRequest();
#if UNITY_2020_1_OR_NEWER
        if (request.result != UnityWebRequest.Result.Success)
#else
    if (request.isNetworkError || request.isHttpError)
#endif
        {
            Debug.LogError("Failed to load SRT file: " + request.error);
            yield break;
        }
        string[] lines = request.downloadHandler.text.Split(new[] { "\r\n", "\n" }, System.StringSplitOptions.None);
        int i = 0;
        // GPT提出之解方。
        //

        while (i < lines.Length)
        {
            if (string.IsNullOrWhiteSpace(lines[i]))
            {
                i++;
                continue;
            }

            // Subtitle index (ignore)
            i++;

            if (i >= lines.Length) break;

            // Time line
            string timeLine = lines[i];
            string[] times = timeLine.Split(new string[] { " --> " }, System.StringSplitOptions.None);
            float startTime = TimeStringToSeconds(times[0]);
            float endTime = TimeStringToSeconds(times[1]);
            i++;

            // Subtitle text
            string text = "";
            while (i < lines.Length && !string.IsNullOrWhiteSpace(lines[i]))
            {
                text += lines[i] + "\n";
                i++;
            }
            text = text.Trim();

            subtitles.Add(new SubtitleEntry { startTime = startTime, endTime = endTime, text = text });
        }
    }

    private void UpdateSubtitle(double currentTime)
    {
        if (currentSubtitleIndex < subtitles.Count)
        {
            SubtitleEntry entry = subtitles[currentSubtitleIndex];

            if (currentTime >= entry.startTime && currentTime <= entry.endTime)
            {
                subtitleText.text = entry.text;
            }
            else if (currentTime > entry.endTime)
            {
                currentSubtitleIndex++;
                subtitleText.text = "";
            }
        }
    }

    private float TimeStringToSeconds(string time)
    {
        // Format: 00:01:02,500
        Regex regex = new Regex(@"(\d+):(\d+):(\d+),(\d+)");
        Match match = regex.Match(time);

        if (match.Success)
        {
            int hours = int.Parse(match.Groups[1].Value);
            int minutes = int.Parse(match.Groups[2].Value);
            int seconds = int.Parse(match.Groups[3].Value);
            int milliseconds = int.Parse(match.Groups[4].Value);

            return hours * 3600 + minutes * 60 + seconds + milliseconds / 1000f;
        }
        return 0;
    }

    private class SubtitleEntry
    {
        public float startTime;
        public float endTime;
        public string text;
    }
}