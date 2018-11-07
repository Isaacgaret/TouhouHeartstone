﻿using UnityEngine;

using System.Collections.Generic;
using System;

namespace TouhouHeartstone.Frontend.Manager
{
    public class HandCardManager : FrontendSubManager
    {
        [SerializeField]
        float mouseYRate = 0.15f;

        List<CardFace> cards = new List<CardFace>();

        bool isCardInHand => Input.mousePosition.y < Screen.height * mouseYRate;

        DeskEntityManager desk => getSiblingManager<DeskEntityManager>();

        /// <summary>
        /// 向手牌中添加一张卡
        /// </summary>
        /// <param name="card"></param>
        public void AddCard(CardFace card)
        {
            int origCnt = cards.Count;
            var pos = getCardPosInfo(cards.Count + 1, origCnt);
            addCardInner(card, pos);
            adjustCardPos(origCnt);
        }

        /// <summary>
        /// 向手牌中添加N张卡
        /// </summary>
        /// <param name="card"></param>
        public void AddCard(CardFace[] card)
        {
            int origCnt = cards.Count;

            for (int i = 0; i < card.Length; i++)
            {
                var pos = getCardPosInfo(origCnt + card.Length, origCnt + i);
                addCardInner(card[i], pos);
            }

            adjustCardPos(origCnt);
        }

        private void addCardInner(CardFace card, CardPosInfo pos)
        {
            cards.Add(card);
            cardRegistEvent(card);
            card.CardAniController.CardMoveToHand(pos.Position, pos.Rotation);
        }

        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="card"></param>
        private void cardRegistEvent(CardFace card)
        {
            card.OnMouseIn += cardOnMouseIn;
            card.OnMouseOut += cardOnMouseOut;
            card.OnClick += cardOnClick;
            card.OnDrag += cardOnDrag;
            card.OnRelease += cardOnRelease;
        }

        struct CardPosInfo
        {
            public Vector3 Position;
            public Vector3 Rotation;
        }

        /// <summary>
        /// 计算第n张卡的位置信息
        /// </summary>
        /// <param name="count">卡片数目</param>
        /// <param name="n">第x张（0开始）</param>
        /// <returns></returns>
        CardPosInfo getCardPosInfo(int count, int n)
        {
            var basePos = new Vector3(0, -1.4f, -0.1f);
            const float normapSpacing = 0.5f;
            const float maxWidth = 1.8f;
            const float cardHalfHeight = 0.36f;


            if (count <= 3)
            {
                var offset = normapSpacing * (count - 1) / 2;
                basePos.x = normapSpacing * n - offset;

                return new CardPosInfo() { Position = basePos, Rotation = Vector3.zero };
            }
            else
            {
                var width = maxWidth + normapSpacing * 0.25f * (count - 3);
                var bt = (count - 1);
                var step = width / bt;
                var deg = ((bt / 2f) - n) * 10;

                basePos.x = step * n - width / 2 + cardHalfHeight * Mathf.Sin(deg * Mathf.Deg2Rad);
                basePos.y -= cardHalfHeight * (1 - Mathf.Cos(deg * Mathf.Deg2Rad)) * 3;
                basePos.z -= 0.02f * n;

                var rot = new Vector3();
                rot.z = deg;
                return new CardPosInfo() { Position = basePos, Rotation = rot };
            }
        }


        private void cardOnMouseOut(CardFace obj)
        {
            if (activeCard != null) return;
            if (obj.State == CardState.Active)
            {
                var p = getCardPosInfo(cards.Count, cards.IndexOf(obj));
                obj.CardAniController.UnShowCard(p.Position, p.Rotation);
            }
        }

        private void cardOnMouseIn(CardFace obj)
        {
            if (activeCard != null) return;
            if (obj.State == CardState.Hand)
            {
                var p = getCardPosInfo(cards.Count, cards.IndexOf(obj));
                p.Position *= 0.75f;
                obj.CardAniController.ShowCard(p.Position);
            }
        }

        private void cardOnClick(CardFace card)
        {
            switch (card.State)
            {
                case CardState.Pickup:
                    cardOnRelease(card);
                    break;
                case CardState.Active:
                    cardOnDrag(card);
                    break;
            }
        }

        private void cardOnDrag(CardFace card)
        {
            if (card.State == CardState.Active)
            {
                activeCard = card;
                card.State = CardState.Pickup;

                card.CardAniController.StopAll();

                // 设置拿起后的位置
                var p = getCardPosInfo(cards.Count, cards.IndexOf(card));
                p.Position.z -= 0.05f;
                card.transform.localPosition = p.Position;
                card.transform.localRotation = Quaternion.Euler(Vector3.zero);
            }
        }

        /// <summary>
        /// 卡片被释放的事件
        /// </summary>
        /// <param name="card"></param>
        private void cardOnRelease(CardFace card)
        {
            if (card.State == CardState.Pickup)
            {
                // 检测是否在安全界限内
                if (isCardInHand)
                {
                    cardBackHand(card);
                    activeCard = null;
                    return;
                }

                // 根据卡的类型来做
                switch (card.Type)
                {
                    // 无目标法术卡，直接使用
                    case CardType.DriftlessSpell:
                        useCard(card);
                        break;
                    // 无目标实体卡，加入到位置中
                    case CardType.Entity:
                        var de = getSiblingManager<DeskEntityManager>();
                        var p = de.ReserveInsertSpace(Input.mousePosition);
                        de.Insert(card, p);
                        useCard(card);
                        break;
                }

            }
        }

        CardFace activeCard;

        private void Update()
        {
            if (activeCard != null)
            {
                switch (activeCard.Type)
                {
                    case CardType.Entity:
                        cardFollowMouse();
                        if (isCardInHand) desk.UpdateEntityPos();
                        else desk.ReserveInsertSpace(Input.mousePosition);
                        break;

                    case CardType.DirectedEntity:
                        cardFollowMouse();
                        break;

                    case CardType.DriftlessSpell:
                        cardFollowMouse();
                        break;
                    case CardType.DirectedSpell:
                        // todo: target selector
                        break;

                }

            }
        }

        /// <summary>
        /// 卡片跟随鼠标
        /// </summary>
        private void cardFollowMouse()
        {
            var t = activeCard.gameObject.transform;
            var orgpos = t.position;

            var mousePos = Input.mousePosition;
            mousePos.z = Camera.main.transform.position.y - t.position.y;
            var world = Camera.main.ScreenToWorldPoint(mousePos);

            orgpos.x = world.x;
            orgpos.z = world.z;

            t.position = orgpos;
        }

        /// <summary>
        /// 销毁卡片
        /// </summary>
        /// <param name="card"></param>
        void cardDestroy(CardFace card)
        {
            card.State = CardState.Destory;
            if (cards.Contains(card)) cards.Remove(card);

            adjustCardPos(cards.Count);
        }

        /// <summary>
        /// 使用卡片
        /// </summary>
        /// <param name="card"></param>
        /// <param name="arg"></param>
        private void useCard(CardFace card, object arg = null)
        {
            card.Use(arg);
            cardDestroy(card);
            activeCard = null;
        }

        /// <summary>
        /// 卡牌回到手牌
        /// </summary>
        /// <param name="card"></param>
        private void cardBackHand(CardFace card)
        {
            var p = getCardPosInfo(cards.Count, cards.IndexOf(card));
            p.Position *= 0.75f;
            card.CardAniController.ShowCard(p.Position);
        }


        /// <summary>
        /// 调整卡片位置
        /// </summary>
        /// <param name="ignoreLast">牌的数目</param>
        void adjustCardPos(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var pos = getCardPosInfo(cards.Count, i);
                cards[i].CardAniController.TweakPosition(pos.Position, pos.Rotation);
            }
        }
    }
}
