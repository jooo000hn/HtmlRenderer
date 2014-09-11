﻿//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
namespace LayoutFarm.Presentation
{


#if DEBUG
    [System.Diagnostics.DebuggerDisplay("RenderBoxBase {dbugGetCssBoxInfo}")]
#endif
    public abstract partial class RenderBoxBase : RenderElement
    {

        int myviewportX;
        int myviewportY;
        VisualLayerCollection layers;


        public RenderBoxBase(int width, int height, ElementNature nature)
            : base(width, height, nature)
        {
        }
        public VisualLayerCollection Layers
        {
            get { return this.layers; }
            set { this.layers = value; }
        }
        public override void CustomDrawToThisPage(CanvasBase canvasPage, InternalRect updateArea)
        {
            //-------------------

            canvasPage.OffsetCanvasOrigin(-myviewportX, -myviewportY);
            updateArea.Offset(myviewportX, myviewportY);


            this.BoxDrawContent(canvasPage, updateArea);

            canvasPage.OffsetCanvasOrigin(myviewportX, myviewportY);
            updateArea.Offset(-myviewportX, -myviewportY);

            //-------------------
        }
        public void InvalidateContentArrangementFromContainerSizeChanged()
        {
            this.MarkInvalidContentArrangement();
            //foreach (VisualLayer layer in this.GetAllLayerBottomToTopIter())
            //{
            //    layer.InvalidateContentArrangementFromContainerSizeChanged();
            //}
        }
        protected virtual void BoxDrawContent(CanvasBase canvasPage, InternalRect updateArea)
        {
            //sample ***
            //1. draw background
            RenderElementHelper.DrawBackground(this, canvasPage, updateArea.Width, updateArea.Height, Color.White);
            //2. draw each layer
            if (this.layers != null)
            {
                int j = this.layers.LayerCount;
                switch (j)
                {
                    case 0:
                        {

                        } break;
                    case 1:
                        {
                            layers.Layer0.DrawChildContent(canvasPage, updateArea);
#if DEBUG
                            debug_RecordLayerInfo(layers.Layer0);
#endif
                        } break;
                    case 2:
                        {
                            layers.Layer0.DrawChildContent(canvasPage, updateArea);
#if DEBUG
                            debug_RecordLayerInfo(layers.Layer0);
#endif
                            layers.Layer1.DrawChildContent(canvasPage, updateArea);
#if DEBUG
                            debug_RecordLayerInfo(layers.Layer1);
#endif
                        } break;
                    default:
                        {
                            for (int i = 0; i < j; ++i)
                            {
                                var layer = this.layers.GetLayer(i);
                                layer.DrawChildContent(canvasPage, updateArea);
#if DEBUG
                                debug_RecordLayerInfo(layer);
#endif
                            }
                        } break;
                }
            }
        }
        public void PrepareOriginalChildContentDrawingChain(VisualDrawingChain chain)
        {

            if (this.layers != null)
            {
                int j = this.layers.LayerCount;
                switch (j)
                {
                    case 0:
                        {

                        } break;
                    case 1:
                        {
                            layers.Layer0.PrepareDrawingChain(chain);
#if DEBUG
                            debug_RecordLayerInfo(layers.Layer0);
#endif
                        } break;
                    case 2:
                        {
                            layers.Layer0.PrepareDrawingChain(chain);
#if DEBUG
                            debug_RecordLayerInfo(layers.Layer0);
#endif
                            layers.Layer1.PrepareDrawingChain(chain);
#if DEBUG
                            debug_RecordLayerInfo(layers.Layer1);
#endif
                        } break;
                    default:
                        {
                            for (int i = 0; i < j; ++i)
                            {
                                var layer = this.layers.GetLayer(i);
                                layer.PrepareDrawingChain(chain);
#if DEBUG
                                debug_RecordLayerInfo(layer);
#endif
                            }
                        } break;
                }
            }
        }
        public virtual void ChildrenHitTestCore(HitPointChain artHitResult)
        {
            if (this.layers != null)
            {
                layers.ChildrenHitTestCore(artHitResult);
            }

        }



        protected static void InnerDoTopDownReCalculateContentSize(RenderBoxBase containerBase, VisualElementArgs vinv)
        {
            containerBase.TopDownReCalculateContentSize(vinv);
        }
        protected static void InnerTopDownReArrangeContentIfNeed(RenderBoxBase containerBase, VisualElementArgs vinv)
        {
            containerBase.TopDownReArrangeContentIfNeed(vinv);
        }
        public override sealed void TopDownReCalculateContentSize(VisualElementArgs vinv)
        {

            if (!vinv.ForceReArrange && this.HasCalculatedSize)
            {
                return;
            }
#if DEBUG
            vinv.dbug_EnterTopDownReCalculateContent(this);
#endif
            int cHeight = this.Height;
            int cWidth = this.Width;
            Size ground_contentSize = Size.Empty;
            if (layers != null)
            {
                ground_contentSize = layers.TopDownReCalculateContentSize(vinv);
            }
            int finalWidth = ground_contentSize.Width;
            if (finalWidth == 0)
            {
                finalWidth = this.Width;
            }
            int finalHeight = ground_contentSize.Height;
            if (finalHeight == 0)
            {
                finalHeight = this.Height;
            }
            switch (GetLayoutSpecificDimensionType(this))
            {
                case LY_HAS_SPC_HEIGHT:
                    {
                        finalHeight = cHeight;
                    } break;
                case LY_HAS_SPC_WIDTH:
                    {
                        finalWidth = cWidth;
                    } break;
                case LY_HAS_SPC_SIZE:
                    {
                        finalWidth = cWidth;
                        finalHeight = cHeight;
                    } break;
            }


            SetCalculatedDesiredSize(this, finalWidth, finalHeight);
#if DEBUG
            vinv.dbug_ExitTopDownReCalculateContent(this);
#endif

        }



        public void ForceTopDownReArrangeContent(VisualElementArgs vinv)
        {

#if DEBUG
            vinv.dbug_EnterReArrangeContent(this);
            dbug_topDownReArrContentPass++;
            this.dbug_BeginArr++;
            vinv.debug_PushTopDownElement(this);
#endif
            this.MarkValidContentArrangement();
            vinv.IsInTopDownReArrangePhase = true;
            this.layers.ForceTopDownReArrangeContent(vinv);
            // BoxEvaluateScrollBar();

#if DEBUG
            this.dbug_FinishArr++;
            vinv.debug_PopTopDownElement(this);
            vinv.dbug_ExitReArrangeContent();
#endif
        }

        public void TopDownReArrangeContentIfNeed(VisualElementArgs vinv)
        {
#if DEBUG
            bool isIncr = false;
#endif

            if (!vinv.ForceReArrange && !this.NeedContentArrangement)
            {
                if (!this.FirstArrangementPass)
                {
                    this.FirstArrangementPass = true;
#if DEBUG
                    vinv.dbug_WriteInfo(dbugVisitorMessage.PASS_FIRST_ARR);
#endif

                }
                else
                {
#if DEBUG
                    isIncr = true;
                    this.dbugVRoot.dbugNotNeedArrCount++;
                    this.dbugVRoot.dbugNotNeedArrCountEpisode++;
                    vinv.dbug_WriteInfo(dbugVisitorMessage.NOT_NEED_ARR);
                    this.dbugVRoot.dbugNotNeedArrCount--;
#endif
                }
                return;
            }

            ForceTopDownReArrangeContent(vinv);


#if DEBUG
            if (isIncr)
            {
                this.dbugVRoot.dbugNotNeedArrCount--;
            }
#endif
        }

        //-------------------------------------------------------------------------------
        public abstract override void ClearAllChildren();

        public override RenderElement FindOverlapedChildElementAtPoint(RenderElement afterThisChild, Point point)
        {
#if DEBUG
            if (afterThisChild.ParentVisualElement != this)
            {
                throw new Exception("not a parent-child relation");
            }
#endif
            if (afterThisChild.ParentLink.MayHasOverlapChild)
            {
                return afterThisChild.ParentLink.FindOverlapedChildElementAtPoint(afterThisChild, point);
            }
            return null;
        }
        public Size InnerContentSize
        {
            get
            {
                if (this.layers != null && layers.LayerCount > 0)
                {
                    var layer0 = this.layers.Layer0;
                    Size s1 = layer0.PostCalculateContentSize;
                    if (s1.Width < this.Width)
                    {
                        s1.Width = this.Width;
                    }
                    if (s1.Height < this.Height)
                    {
                        s1.Height = this.Height;
                    }
                    return s1;
                }
                else
                {
                    return this.Size;
                }

            }
        }

        public int ViewportBottom
        {
            get
            {
                return this.Bottom + myviewportY;
            }
        }
        public int ViewportRight
        {
            get
            {
                return this.Right + this.myviewportX;
            }
        }
        public int ViewportY
        {
            get
            {
                return this.myviewportY;
            }
            set
            {
                this.myviewportY = value;
            }
        }
        public int ViewportX
        {
            get
            {

                return this.myviewportX;
            }
            set
            {
                this.myviewportX = value;
            }
        }
        public int ClientTop
        {
            get
            {
                return 0;
            }
        }
        public int ClientLeft
        {
            get
            {
                return 0;
            }
        }

        //--------------------------------------------
#if DEBUG
        public override void dbug_DumpVisualProps(dbugLayoutMsgWriter writer)
        {
            base.dbug_DumpVisualProps(writer);
            writer.EnterNewLevel();

            writer.LeaveCurrentLevel();
        }
        void debug_RecordLayerInfo(VisualLayer layer)
        {
            dbugRootElement visualroot = dbugRootElement.dbugCurrentGlobalVRoot;
            if (visualroot.dbug_RecordDrawingChain)
            {
                visualroot.dbug_AddDrawLayer(layer);
            }
        }
        static int dbug_topDownReArrContentPass = 0;
#endif

    }



    public static class RenderBoxBaseHelper
    {
        public static void AddChild(this RenderBoxBase box, RenderElement e)
        {
            if (box.Layers != null)
            {
                box.Layers.Layer0.AddTop(e);

            }
        }
    }
}
