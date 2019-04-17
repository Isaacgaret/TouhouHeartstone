﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace TouhouHeartstone.Frontend.View
{
    public class GlobalView : MonoBehaviour
    {
        public CardPositionCalculator CardPositionCalculator => cardPositionCalculator;
        CardPositionCalculator cardPositionCalculator = new CardPositionCalculator(new Vector2(Screen.width, Screen.height));

        [SerializeField]
        CardImageResources images;

        [SerializeField]
        CardTextResources texts;

        public CardImageResource GetCardImageResource(int id) { return GetCardImageResource(id.ToString()); }
        public CardTextResource GetCardTextResource(int id) { return GetCardTextResource(id.ToString()); }

        public CardImageResource GetCardImageResource(string id)
        {
            return images.Get(id, "zh-CN");
        }

        public CardTextResource GetCardTextResource(string id)
        {
            return texts.Get(id, "zh-CN");
        }
    }

    public class CardPositionCalculator
    {
        Vector2 screenSize;
        public CardPositionCalculator(Vector2 size)
        {
            screenSize = size;
        }

        float cardHandSpacing => screenSize.y * 0.08f;
        float cardCenterSpacing => screenSize.y * 0.2f;
        float cardHandBaseY => screenSize.y * 0.01f;

        float maxHandWidth => screenSize.y * 0.5f;

        float cardHalfHeight => screenSize.y * 0.2f;

        float rotateAngle = 2;

        public PositionWithRotation GetCardCenter(int i, int count)
        {
            if (i >= count)
                throw new ArgumentOutOfRangeException();

            Vector3 center = screenSize / 2;

            center.x += (i - (count - 1) / 2f) * cardCenterSpacing;
            return new PositionWithRotation() { Position = center };
        }

        public PositionWithRotation GetCardHand(int i, int count)
        {
            if (i >= count)
                throw new ArgumentOutOfRangeException();

            // position
            Vector3 basePos = new Vector3(screenSize.x / 2, cardHandBaseY);

            if (count <= 3)
            {
                basePos.x += (i - (count - 1) / 2f) * cardHandSpacing;

                return new PositionWithRotation() { Position = basePos, Rotation = Vector3.zero };
            }
            else
            {
                var width = maxHandWidth + cardHandSpacing * 0.25f * (count - 3);
                var bt = (count - 1);
                var step = width / bt;
                var deg = ((bt / 2f) - i) * 10;

                basePos.x = step * i - width / 2 + cardHalfHeight * Mathf.Sin(deg * Mathf.Deg2Rad);
                basePos.y -= cardHalfHeight * (1 - Mathf.Cos(deg * Mathf.Deg2Rad)) * 3;

                var rot = new Vector3();
                rot.z = deg;
                return new PositionWithRotation() { Position = basePos, Rotation = rot };
            }
        }

        public Vector3 StackPosition => new Vector3(1920, 0, 0);
       
    }
    public struct PositionWithRotation
    {
        public Vector3 Position;
        public Vector3 Rotation;
    }
}
