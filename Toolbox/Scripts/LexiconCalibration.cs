// Copyright (c) 2018 Mixspace Technologies, LLC. All rights reserved.

using System.Collections;
using System.Collections.Generic;
using System.Text;
using Mixspace.Lexicon;
using UnityEngine;
using UnityEngine.UI;

namespace Mixspace.Lexicon.Samples
{
    public class LexiconCalibration : MonoBehaviour
    {
        /*
         * 2 seconds to read instructions
         * 5 seconds to record baseline silence
         * name each cube color 2 or 3 times
         * (discard outliers)
         * record max level during this process
         * find silence cutoff (10% over min level?)
         * save two values to player prefs
         */

        public List<GameObject> targets;
        public List<string> keywords;
        public int matchesRequired = 12;

        public Text outputText;

        private LexiconRuntime lexiconRuntime;
        private LexiconFocusManager focusManager;

        private GameObject currentTarget;
        private float startTime;

        private List<TargetInfo> targetInfo = new List<TargetInfo>();

        private float maxLevel;

        private List<float> matchOffsets = new List<float>();

        void OnEnable()
        {
            LexiconRuntime.OnKeywordDetected += OnKeywordDetected;

            foreach (string keyword in keywords)
            {
                lexiconRuntime.Keywords.Add(keyword);
            }
            lexiconRuntime.Restart();

            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                transform.position = mainCamera.transform.position + mainCamera.transform.forward;
                transform.rotation = Quaternion.LookRotation(transform.transform.position - mainCamera.transform.position);
            }
        }

        void OnDisable()
        {
            LexiconRuntime.OnKeywordDetected -= OnKeywordDetected;

            foreach (string keyword in keywords)
            {
                lexiconRuntime.Keywords.Remove(keyword);
            }
            lexiconRuntime.Restart();
        }

        void Awake()
        {
            lexiconRuntime = FindObjectOfType<LexiconRuntime>();
            focusManager = LexiconFocusManager.Instance;

            if (targets.Count != keywords.Count)
            {
                Debug.LogError("LexiconCalibration: keyword count must match target count!");
            }

            for (int i = 0; i < targets.Count; i++)
            {
                TargetInfo info = new TargetInfo();
                info.target = targets[i];
                info.keyword = keywords[i];
                targetInfo.Add(info);

                GameObject target = targets[i];
                if (target.GetComponent<LexiconSelectable>() == null)
                {
                    target.AddComponent<LexiconSelectable>();
                }
            }
        }

        void Start()
        {
            StartCoroutine(RunCalibration());
        }

        private IEnumerator RunCalibration()
        {
            outputText.text = "Silence Please";
            yield return new WaitForSeconds(2.0f);
            lexiconRuntime.Workspace.WatsonSpeechToTextManager.StartCalibrating();
            yield return new WaitForSeconds(5.0f);
            float averageLevel = lexiconRuntime.Workspace.WatsonSpeechToTextManager.StopCalibrating();

            outputText.text = "Look at each cube and say its color";

            while (matchOffsets.Count < matchesRequired)
            {
                if (matchOffsets.Count > 0)
                {
                    outputText.text = matchOffsets.Count + " / " + matchesRequired;
                }
                maxLevel = Mathf.Max(maxLevel, lexiconRuntime.Workspace.WatsonSpeechToTextManager.CurrentLevel);
                yield return 0;
            }

            float silenceCutoff = averageLevel + 0.1f * (maxLevel - averageLevel);

            Debug.Log("Average Level: " + averageLevel);
            Debug.Log("Max Level: " + maxLevel);
            Debug.Log("Silence Cutoff: " + silenceCutoff);

            float offsetAvg = OffsetAverage();
            float stddev = OffsetStandardDeviation(averageLevel);

            for (int j = 0; j < 3; j++)
            {
                for (int i = matchOffsets.Count - 1; i >= 0; i--)
                {
                    float dist = Mathf.Abs(matchOffsets[i] - offsetAvg);
                    if (dist >= (stddev))
                    {
                        matchOffsets.RemoveAt(i);
                    }
                }

                offsetAvg = OffsetAverage();
                stddev = OffsetStandardDeviation(offsetAvg);
            }

            outputText.text = string.Format("Silence Cutoff: {0:F4}\nOffset Avg: {1:F2}\nOffset StdDev: {2:F2}", silenceCutoff, offsetAvg, stddev);

            PlayerPrefs.SetFloat("SilenceThreshold", silenceCutoff);
            PlayerPrefs.SetFloat("TimestampOffset", offsetAvg);
        }

        private float OffsetAverage()
        {
            float sum = 0;
            foreach (float sample in matchOffsets)
            {
                sum += sample;
            }
            return sum / matchOffsets.Count;
        }

        private float OffsetStandardDeviation(float average)
        {
            float sum = 0;
            foreach (float sample in matchOffsets)
            {
                float s = sample - average;
                sum += s * s;
            }
            return Mathf.Sqrt(sum / matchOffsets.Count);
        }

        void Update()
        {
            FocusSelection focusObject = focusManager.GetFocusData<FocusSelection>(Time.realtimeSinceStartup);
            if (focusObject != null && focusObject.SelectedObject != null && focusObject.SelectedObject != currentTarget)
            {
                foreach (TargetInfo info in targetInfo)
                {
                    if (info.target == focusObject.SelectedObject)
                    {
                        FocusDwellPosition dwellPosition = focusManager.GetFocusDataAfter<FocusDwellPosition>(focusObject.Timestamp);
                        if (dwellPosition != null)
                        {
                            currentTarget = focusObject.SelectedObject;

                            info.selectedTime = dwellPosition.Timestamp;
                            info.dwelled = true;

                            LexiconSelectable selectable = currentTarget.GetComponent<LexiconSelectable>();
                            if (selectable != null) selectable.Select();
                        }
                    }
                }
            }
            else if (focusObject == null || focusObject.SelectedObject == null)
            {
                currentTarget = null;
            }
        }

        void OnKeywordDetected(LexiconSpeechResult.KeywordResult keywordResult)
        {
            foreach (TargetInfo info in targetInfo)
            {
                if (info.dwelled)
                {
                    if (info.keyword == keywordResult.Keyword)
                    {
                        float delta = keywordResult.RealtimeStart - info.selectedTime;

                        info.matched = true;
                        info.delta = delta;

                        info.dwelled = false;

                        LexiconSelectable selectable = info.target.GetComponent<LexiconSelectable>();
                        if (selectable != null) selectable.Deselect();

                        matchOffsets.Add(delta);
                    }
                }
            }
        }

        private void UpdateOutput()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine(string.Format("Calibration: {0:F2}", lexiconRuntime.TimestampOffset));

            foreach (TargetInfo info in targetInfo)
            {
                builder.AppendLine(string.Format("{0}: {1:F2}", info.keyword, info.delta));
            }

            outputText.text = builder.ToString();
        }

        private class TargetInfo
        {
            public GameObject target;
            public string keyword;
            public float selectedTime;

            public bool dwelled;
            public bool matched;
            public float delta;
        }
    }
}
