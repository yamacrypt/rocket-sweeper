#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ScoreRankingView), true)]
 public class AlignScoreRankingEditor : Editor
 {
    ScoreRankingView alignChunk;
    private void OnEnable()
     {    
        alignChunk = target as ScoreRankingView;
      }

      public override void OnInspectorGUI()
      {
        base.OnInspectorGUI();
        if (GUILayout.Button("整列"))
        {
            alignChunk.Align();
        }
      }
 }
 #endif