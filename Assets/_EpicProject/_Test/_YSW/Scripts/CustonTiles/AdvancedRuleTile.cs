// AdvancedRuleTile.cs

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Advanced Rule Tile", menuName = "Tiles/Advanced Rule Tile")]
public class AdvancedRuleTile : TileBase
{
    // 규칙에 사용할 타일을 지정. 일반 Tile, Animated Tile 모두 가능.
    public TileBase m_DefaultTile;

    [System.Serializable]
    public class Rule
    {
        public Neighbor[] m_Neighbors;
        public TileBase[] m_Tiles; // 여러 타일을 등록해 랜덤으로 출력 가능
        public float m_AnimationSpeed;
        public float m_PerlinScale;
        public RuleTile.TilingRule.Transform m_RuleTransform;
        public OutputSprite m_Output;
        public Tile.ColliderType m_ColliderType;

        public Rule()
        {
            m_Output = OutputSprite.Single;
            m_AnimationSpeed = 1f;
            m_PerlinScale = 0.5f;
            m_ColliderType = Tile.ColliderType.None;
        }

        public enum Neighbor { DontCare, This, NotThis }
        public enum OutputSprite { Single, Random, Animation }
    }

    [HideInInspector] public List<Rule> m_TilingRules = new List<Rule>();

    public override bool GetTileAnimationData(Vector3Int position, ITilemap tilemap, ref TileAnimationData tileAnimationData)
    {
        int index = GetRuleIndex(position, tilemap);
        if (index > -1 && m_TilingRules[index].m_Output == Rule.OutputSprite.Animation && m_TilingRules[index].m_Tiles.Length > 1)
        {
            var rule = m_TilingRules[index];
            int tileCount = rule.m_Tiles.Length;

            // 모든 프레임의 스프라이트를 가져옴
            Sprite[] sprites = new Sprite[tileCount];
            for (int i = 0; i < tileCount; i++)
            {
                TileData tempTileData = new TileData();
                rule.m_Tiles[i].GetTileData(position, tilemap, ref tempTileData);
                sprites[i] = tempTileData.sprite;
            }

            tileAnimationData.animatedSprites = sprites;
            tileAnimationData.animationSpeed = rule.m_AnimationSpeed;
            return true;
        }
        return false;
    }

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        int index = GetRuleIndex(position, tilemap);
        if (index > -1)
        {
            // ApplyRule을 호출할 때 position 값을 넘겨주도록 수정
            ApplyRule(position, index, ref tileData);
        }
        else if (m_DefaultTile)
        {
            m_DefaultTile.GetTileData(position, tilemap, ref tileData);
        }
    }

    private void ApplyRule(Vector3Int position, int index, ref TileData tileData)
    {
        var rule = m_TilingRules[index];

        // 규칙에 맞는 타일의 데이터를 가져옴
        if (rule.m_Output != Rule.OutputSprite.Animation && rule.m_Tiles.Length > 0)
        {
            // tileData.position 대신 파라미터로 받은 position 사용
            int tileIndex = (rule.m_Output == Rule.OutputSprite.Random) ?
                Mathf.FloorToInt(GetPerlinValue(position, rule.m_PerlinScale, 100000f) * rule.m_Tiles.Length) : 0;

            if (tileIndex < rule.m_Tiles.Length)
            {
                // 여기도 position으로 수정
                rule.m_Tiles[tileIndex].GetTileData(position, null, ref tileData);
            }
        }

        tileData.colliderType = rule.m_ColliderType;

        var m = tileData.transform;
        m.SetTRS(Vector3.zero, GetRotation(rule.m_RuleTransform), GetScale(rule.m_RuleTransform));
        tileData.transform = m;
    }

    private int GetRuleIndex(Vector3Int position, ITilemap tilemap)
    {
        for (int i = 0; i < m_TilingRules.Count; i++)
        {
            if (RuleMatches(m_TilingRules[i], position, tilemap))
                return i;
        }
        return -1;
    }

    private bool RuleMatches(Rule rule, Vector3Int position, ITilemap tilemap)
    {
        for (int y = -1; y <= 1; y++)
        {
            for (int x = -1; x <= 1; x++)
            {
                if (x == 0 && y == 0) continue;

                var offset = new Vector3Int(x, y, 0);
                var checkPos = position + offset;
                var neighbor = rule.m_Neighbors[y * 3 + x + (y > 0 ? -1 : 0) + (x > 0 ? -1 : 0)];

                if (neighbor == Rule.Neighbor.DontCare) continue;

                TileBase neighborTile = tilemap.GetTile(checkPos);
                bool isThis = (neighborTile == this || (m_DefaultTile && neighborTile == m_DefaultTile));
                if ((neighbor == Rule.Neighbor.This && !isThis) || (neighbor == Rule.Neighbor.NotThis && isThis))
                {
                    return false;
                }
            }
        }
        return true;
    }

    private static float GetPerlinValue(Vector3Int position, float scale, float offset)
    {
        return Mathf.PerlinNoise((position.x + offset) * scale, (position.y + offset) * scale);
    }

    private static Quaternion GetRotation(RuleTile.TilingRule.Transform transform)
    {
        switch (transform)
        {
            case RuleTile.TilingRule.Transform.Rotated: return Quaternion.Euler(0f, 0f, -90f);
            case RuleTile.TilingRule.Transform.MirrorX: return Quaternion.Euler(0f, 0f, 0f); // Handled by GetScale
            case RuleTile.TilingRule.Transform.MirrorY: return Quaternion.Euler(0f, 0f, 0f); // Handled by GetScale
        }
        return Quaternion.identity;
    }

    private static Vector3 GetScale(RuleTile.TilingRule.Transform transform)
    {
        switch (transform)
        {
            case RuleTile.TilingRule.Transform.MirrorX: return new Vector3(-1f, 1f, 1f);
            case RuleTile.TilingRule.Transform.MirrorY: return new Vector3(1f, -1f, 1f);
        }
        return Vector3.one;
    }
}