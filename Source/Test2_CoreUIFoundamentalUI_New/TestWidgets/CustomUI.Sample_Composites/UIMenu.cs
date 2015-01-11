﻿// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;


using LayoutFarm.UI;
using LayoutFarm.RenderBoxes;
namespace LayoutFarm.CustomWidgets
{

    public class UIMenuItem : UIBox
    {

        CustomRenderBox primElement;//background 
        Color backColor = Color.LightGray;
        bool isOpen;
        //1. land part
        UIBox landPart;

        //2. float part   
        UIMenuBox floatPart;
        RenderElement floatPartRenderElement;
        HingeFloatPartStyle floatPartStyle;

        List<UIMenuItem> childItems;

        public UIMenuItem(int width, int height)
            : base(width, height)
        {

        }

        protected override bool HasReadyRenderElement
        {
            get { return this.primElement != null; }
        }
        protected override RenderElement CurrentPrimaryRenderElement
        {
            get { return this.primElement; }
        }
        public Color BackColor
        {
            get { return this.backColor; }
            set
            {
                this.backColor = value;
                if (HasReadyRenderElement)
                {
                    this.primElement.BackColor = value;
                }
            }
        }
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (primElement == null)
            {
                var renderE = new CustomRenderBox(rootgfx, this.Width, this.Height);
                RenderElement.DirectSetVisualElementLocation(renderE, this.Left, this.Top);
                renderE.BackColor = backColor;
                renderE.SetController(this);
                renderE.HasSpecificSize = true;
                //------------------------------------------------
                //create visual layer
                var layers = new VisualLayerCollection();
                var layer0 = new PlainLayer(renderE);
                layers.AddLayer(layer0);
                renderE.Layers = layers;

                //layer0

                //layer0 
                //int layerCount = this.layers.Count;
                //for (int m = 0; m < layerCount; ++m)
                //{
                //    PlainLayerElement plain = (PlainLayerElement)this.layers[m];
                //    var groundLayer = new VisualPlainLayer(renderE);
                //    renderE.Layers.AddLayer(groundLayer);

                //    //---------------------------------
                //    int j = plain.Count;
                //    for (int i = 0; i < j; ++i)
                //    {
                //        groundLayer.AddUI(plain.GetElement(i));
                //    }
                //}
                //---------------------------------
                if (this.landPart != null)
                {
                    layer0.AddChild(this.landPart.GetPrimaryRenderElement(rootgfx));
                }
                if (this.floatPart != null)
                {

                }

                //---------------------------------
                primElement = renderE;
            }
            return primElement;
        }
        //----------------------------------------------------
        protected override void OnMouseDown(UIMouseEventArgs e)
        {
            if (this.MouseDown != null)
            {
                this.MouseDown(this, e);
            }
        }
        protected override void OnDragBegin(UIMouseEventArgs e)
        {
            if (this.DragStart != null)
            {
                this.DragStart(this, e);
            }
            base.OnDragBegin(e);
        }
        protected override void OnDragEnd(UIMouseEventArgs e)
        {
            if (this.DragStop != null)
            {
                this.DragStop(this, e);
            }
            base.OnDragEnd(e);
        }
        protected override void OnMouseUp(UIMouseEventArgs e)
        {
            if (this.MouseUp != null)
            {
                MouseUp(this, e);
            }
            base.OnMouseUp(e);
        }
        protected override void OnDragging(UIMouseEventArgs e)
        {
            if (this.Dragging != null)
            {
                Dragging(this, e);
            }
            base.OnDragging(e);
        }
        //----------------------------------------------------

        public event EventHandler<UIMouseEventArgs> MouseDown;
        public event EventHandler<UIMouseEventArgs> MouseUp;

        public event EventHandler<UIMouseEventArgs> Dragging;
        public event EventHandler<UIMouseEventArgs> DragStart;
        public event EventHandler<UIMouseEventArgs> DragStop;

        //----------------------------------------------------  
        public UIBox LandPart
        {
            get { return this.landPart; }
            set
            {
                this.landPart = value;
                if (value != null)
                {
                    //if new value not null
                    //check existing land part
                    if (this.landPart != null)
                    {
                        //remove existing landpart

                    }

                    if (primElement != null)
                    {
                        //add 
                        var visualPlainLayer = primElement.Layers.GetLayer(0) as PlainLayer;
                        if (visualPlainLayer != null)
                        {
                            visualPlainLayer.AddChild(value.GetPrimaryRenderElement(primElement.Root));
                        }

                    }

                }
                else
                {
                    if (this.landPart != null)
                    {
                        //remove existing landpart

                    }
                }
            }
        }
        public UIMenuBox FloatPart
        {
            get { return this.floatPart; }
            set
            {
                this.floatPart = value;
                if (value != null)
                {
                    //attach float part

                }
            }
        }
        //---------------------------------------------------- 
        public bool IsOpen
        {
            get { return this.isOpen; }
        }
        //----------------------------------------------------  
        public void OpenHinge()
        {
            if (isOpen) return;
            this.isOpen = true;

            //-----------------------------------
            if (this.primElement == null) return;
            if (floatPart == null) return;


            switch (floatPartStyle)
            {
                default:
                case HingeFloatPartStyle.Popup:
                    {
                        //add float part to top window layer
                        var topRenderBox = primElement.GetTopWindowRenderBox();
                        if (topRenderBox != null)
                        {
                            Point globalLocation = primElement.GetGlobalLocation();
                            floatPart.SetLocation(globalLocation.X, globalLocation.Y + primElement.Height);
                            this.floatPartRenderElement = this.floatPart.GetPrimaryRenderElement(primElement.Root);
                            topRenderBox.AddChild(floatPartRenderElement);
                        }

                    } break;
                case HingeFloatPartStyle.Embeded:
                    {

                    } break;
            }
        }
        public void CloseHinge()
        {
            if (!isOpen) return;
            this.isOpen = false;

            if (this.primElement == null) return;
            if (floatPart == null) return;

            switch (floatPartStyle)
            {
                default:
                    {
                    } break;
                case HingeFloatPartStyle.Popup:
                    {
                        if (floatPartRenderElement != null)
                        {
                            //temp
                            var parentContainer = floatPartRenderElement.ParentRenderElement as CustomRenderBox;
                            if (parentContainer.Layers != null)
                            {
                                PlainLayer plainLayer = (PlainLayer)parentContainer.Layers.GetLayer(0);
                                plainLayer.RemoveChild(floatPartRenderElement);

                            }
                        }

                    } break;
                case HingeFloatPartStyle.Embeded:
                    {
                    } break;

            }
        }

        public HingeFloatPartStyle FloatPartStyle
        {
            get { return this.floatPartStyle; }
            set
            {
                this.floatPartStyle = value;
            }
        }


        public void AddSubMenuItem(UIMenuItem childItem)
        {
            if (childItems == null)
            {
                childItems = new List<UIMenuItem>();
            }
            this.childItems.Add(childItem);
            floatPart.AddChildBox(childItem);
        }
    }

    public class UIMenuBox : UIPanel
    {
        public UIMenuBox(int w, int h)
            : base(w, h)
        {
        }
    }
}