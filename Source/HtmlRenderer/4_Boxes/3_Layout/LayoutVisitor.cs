﻿//BSD 2014, WinterDev

using System;
using System.Collections.Generic;

using HtmlRenderer.Css;
using LayoutFarm.Drawing;

namespace HtmlRenderer.Boxes
{

    public class LayoutVisitor : BoxVisitor
    {
        RootVisualBox visualRootBox;
        float totalMarginLeftAndRight;


        Queue<Dictionary<CssBox, PartialBoxStrip>> dicStripPool;
        Queue<List<PartialBoxStrip>> listStripPool;

        Dictionary<CssBox, PartialBoxStrip> readyDicStrip = new Dictionary<CssBox, PartialBoxStrip>();
        List<PartialBoxStrip> readyListStrip = new List<PartialBoxStrip>();



        static int totalLayoutIdEpisode = 0;
        readonly int episodeId = ++totalLayoutIdEpisode;

        internal LayoutVisitor(IGraphics gfx, RootVisualBox visualRootBox)
        {
            this.Gfx = gfx;
            this.visualRootBox = visualRootBox;

            if (episodeId == ushort.MaxValue - 1)
            {
                //reset
                totalLayoutIdEpisode = 1;
                episodeId = totalLayoutIdEpisode++;
            }
        }

        internal IGraphics Gfx
        {
            get;
            private set;
        }
        protected override void OnPushDifferentContaingBlock(CssBox box)
        {
            this.totalMarginLeftAndRight += (box.ActualMarginLeft + box.ActualMarginRight);
        }
        protected override void OnPopDifferentContaingBlock(CssBox box)
        {
            this.totalMarginLeftAndRight -= (box.ActualMarginLeft + box.ActualMarginRight);
        }
        //-----------------------------------------
        internal CssBox LatestSiblingBox
        {
            get;
            set;
        }
        internal void UpdateRootSize(CssBox box)
        {

            float candidateRootWidth = Math.Max(box.CalculateMinimumWidth(this.episodeId) + CalculateWidthMarginTotalUp(box),
                         (box.SizeWidth + this.ContainerBlockGlobalX) < CssBoxConstConfig.BOX_MAX_RIGHT ? box.SizeWidth : 0);

            this.visualRootBox.UpdateSizeIfWiderOrHeigher(
                this.ContainerBlockGlobalX + candidateRootWidth,
                this.ContainerBlockGlobalY + box.SizeHeight);
        }
        /// <summary>
        /// Get the total margin value (left and right) from the given box to the given end box.<br/>
        /// </summary>
        /// <param name="box">the box to start calculation from.</param>
        /// <returns>the total margin</returns>
        float CalculateWidthMarginTotalUp(CssBox box)
        {

            if ((box.SizeWidth + this.ContainerBlockGlobalX) > CssBoxConstConfig.BOX_MAX_RIGHT ||
                (box.ParentBox != null && (box.ParentBox.SizeWidth + this.ContainerBlockGlobalX) > CssBoxConstConfig.BOX_MAX_RIGHT))
            {
                return (box.ActualMarginLeft + box.ActualMarginRight) + totalMarginLeftAndRight;
            }
            return 0;
        }

        internal bool AvoidImageAsyncLoadOrLateBind
        {
            get { return this.visualRootBox.AvoidAsyncImagesLoading || this.visualRootBox.AvoidImagesLateLoading; }
        }

        internal void RequestImage(ImageBinder binder, CssBox requestFrom)
        {
            RootVisualBox.RaiseRequestImage(
                this.visualRootBox,
                binder,
                requestFrom,
                false);
        }

        internal float MeasureWhiteSpace(CssBox box)
        {
            //depends on Font of this box           
            float w = this.Gfx.MeasureWhitespace(box.ActualFont);

            if (!(box.WordSpacing.IsEmpty || box.WordSpacing.IsNormalWordSpacing))
            {
                w += CssValueParser.ConvertToPxWithFontAdjust(box.WordSpacing, 0, box);
            }
            return w;
        }
        internal FontInfo GetFontInfo( Font f)
        {
            return this.Gfx.GetFontInfo(f);
        }
        internal float MeasureStringWidth(char[] buffer, int startIndex, int len,  Font f)
        {
            return this.Gfx.MeasureString2(buffer, startIndex, len, f).Width;
        }

        //---------------------------------------------------------------
        internal Dictionary<CssBox, PartialBoxStrip> GetReadyStripDic()
        {
            if (readyDicStrip == null)
            {
                if (this.dicStripPool == null || this.dicStripPool.Count == 0)
                {
                    return new Dictionary<CssBox, PartialBoxStrip>();
                }
                else
                {
                    return this.dicStripPool.Dequeue();
                }
            }
            else
            {
                var tmpReadyStripDic = this.readyDicStrip;
                this.readyDicStrip = null;
                return tmpReadyStripDic;
            }
        }
        internal void ReleaseStripDic(Dictionary<CssBox, PartialBoxStrip> dic)
        {
            //clear before add to pool
            dic.Clear();

            if (this.readyDicStrip == null)
            {
                this.readyDicStrip = dic;
            }
            else
            {
                if (this.dicStripPool == null)
                {
                    this.dicStripPool = new Queue<Dictionary<CssBox, PartialBoxStrip>>();
                }


                this.dicStripPool.Enqueue(dic);
            }
        }
        //---------------------------------------------------------------
        internal List<PartialBoxStrip> GetReadyStripList()
        {
            if (readyListStrip == null)
            {
                if (this.dicStripPool == null || this.dicStripPool.Count == 0)
                {
                    return new List<PartialBoxStrip>();
                }
                else
                {
                    return this.listStripPool.Dequeue();
                }
            }
            else
            {
                var tmpReadyListStrip = this.readyListStrip;
                this.readyListStrip = null;
                return tmpReadyListStrip;
            }
        }
        internal void ReleaseStripList(List<PartialBoxStrip> list)
        {
            //clear before add to pool
            list.Clear();

            if (this.readyListStrip == null)
            {
                this.readyListStrip = list;
            }
            else
            {
                if (this.listStripPool == null)
                {
                    this.listStripPool = new Queue<List<PartialBoxStrip>>();
                }
                this.listStripPool.Enqueue(list);
            }
        }

        //---------------------------------------------------------------

        internal int EpisodeId
        {
            get { return this.episodeId; }
        }
        //---------------------------------------------------------------
    }


}