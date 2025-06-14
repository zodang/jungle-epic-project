// AdvancedRuleTileEditor.cs

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

[CustomEditor(typeof(AdvancedRuleTile))]
public class AdvancedRuleTileEditor : Editor
{
    private AdvancedRuleTile tile { get { return (target as AdvancedRuleTile); } }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.Space();

        EditorGUI.BeginChangeCheck();
        tile.m_DefaultTile = (TileBase)EditorGUILayout.ObjectField("Default Tile", tile.m_DefaultTile, typeof(TileBase), false);
        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(tile);

        EditorGUILayout.Space();

        if (tile.m_TilingRules == null)
            tile.m_TilingRules = new List<AdvancedRuleTile.Rule>();

        for (int i = 0; i < tile.m_TilingRules.Count; i++)
        {
            DrawRule(tile.m_TilingRules[i]);
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Add New Rule"))
        {
            var newRule = new AdvancedRuleTile.Rule();
            newRule.m_Neighbors = new AdvancedRuleTile.Rule.Neighbor[8];
            newRule.m_Tiles = new TileBase[1];
            tile.m_TilingRules.Add(newRule);
        }
    }

    private void DrawRule(AdvancedRuleTile.Rule rule)
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUI.indentLevel++;

        // Rule header
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Rule", EditorStyles.boldLabel);
        if (GUILayout.Button("Remove", GUILayout.Width(60)))
        {
            tile.m_TilingRules.Remove(rule);
            return;
        }
        EditorGUILayout.EndHorizontal();

        // Neighbor Grid
        EditorGUILayout.LabelField("Neighbors");
        Rect gridRect = EditorGUILayout.GetControlRect(false, 60);
        DrawNeighborGrid(gridRect, rule.m_Neighbors);

        // Rule settings
        rule.m_RuleTransform = (RuleTile.TilingRule.Transform)EditorGUILayout.EnumPopup("Transform", rule.m_RuleTransform);
        rule.m_Output = (AdvancedRuleTile.Rule.OutputSprite)EditorGUILayout.EnumPopup("Output", rule.m_Output);
        rule.m_ColliderType = (Tile.ColliderType)EditorGUILayout.EnumPopup("Collider", rule.m_ColliderType);

        if (rule.m_Output == AdvancedRuleTile.Rule.OutputSprite.Animation)
        {
            rule.m_AnimationSpeed = EditorGUILayout.FloatField("Speed", rule.m_AnimationSpeed);
        }
        if (rule.m_Output == AdvancedRuleTile.Rule.OutputSprite.Random)
        {
            rule.m_PerlinScale = EditorGUILayout.FloatField("Scale", rule.m_PerlinScale);
        }

        // Output Tiles list
        var list = new List<TileBase>(rule.m_Tiles);
        int newCount = Mathf.Max(0, EditorGUILayout.IntField("Size", list.Count));
        while (newCount < list.Count)
            list.RemoveAt(list.Count - 1);
        while (newCount > list.Count)
            list.Add(null);

        for (int i = 0; i < list.Count; i++)
        {
            list[i] = (TileBase)EditorGUILayout.ObjectField("Tile " + i, list[i], typeof(TileBase), false);
        }
        rule.m_Tiles = list.ToArray();

        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();

        EditorUtility.SetDirty(tile);
    }

    private void DrawNeighborGrid(Rect rect, AdvancedRuleTile.Rule.Neighbor[] neighbors)
    {
        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                if (x == 1 && y == 1) continue;

                Rect r = new Rect(rect.x + x * 20, rect.y + y * 20, 18, 18);
                int index = y * 3 + x + (y > 0 ? -1 : 0) + (x > 0 ? -1 : 0);

                switch (neighbors[index])
                {
                    case AdvancedRuleTile.Rule.Neighbor.This: GUI.color = Color.green; break;
                    case AdvancedRuleTile.Rule.Neighbor.NotThis: GUI.color = Color.red; break;
                    default: GUI.color = Color.gray; break;
                }

                if (GUI.Button(r, ""))
                {
                    neighbors[index] = (AdvancedRuleTile.Rule.Neighbor)(((int)neighbors[index] + 1) % 3);
                }
            }
        }
        GUI.color = Color.white;
    }
}