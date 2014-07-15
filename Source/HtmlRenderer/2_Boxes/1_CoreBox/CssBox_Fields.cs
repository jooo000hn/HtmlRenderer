﻿//BSD 2014, WinterDev
//ArthurHub

// "Therefore those skilled at the unorthodox
// are infinite as heaven and earth,
// inexhaustible as the great rivers.
// When they come to an end,
// they begin again,
// like the days and months;
// they die and are reborn,
// like the four seasons."
// 
// - Sun Tsu,
// "The Art of War"

using System;
using System.Collections.Generic;


namespace HtmlRenderer.Boxes
{

    //collection features

    partial class CssBox
    {

        readonly object _controller;

        //----------------------------------------------------
        /// <summary>
        /// the html tag that is associated with this css box, null if anonymous box
        /// </summary> 
        int _boxCompactFlags;
        //html rowspan: for td,th 
        int _rowSpan;
        int _colSpan;
        //---------------------------------------------------- 

        //condition 1 :this Box is BlockBox
        //1.1 contain lineBoxes for my children and  other children (share)
        LinkedList<CssLineBox> _clientLineBoxes;
        //1.2 contains box collection for my children
        CssBoxCollection _aa_boxes;
        //----------------------------------------------------    
        //condition 2 :this Box is InlineBox          
        List<CssRun> _aa_contentRuns;
        char[] _buffer;
        //----------------------------------------------------   

        //----------------------------------------------------  
        //for other subbox , list item , shadow... 
        SubBoxCollection _subBoxes;
        //----------------------------------------------------   

        //state
        protected int _prop_pass_eval;

        /// <summary>
        /// Gets the childrenn boxes of this box
        /// </summary>      
        CssBoxCollection Boxes
        {
            get { return _aa_boxes; }
        }

        internal int RunCount
        {
            get
            {
                return this._aa_contentRuns != null ? this._aa_contentRuns.Count : 0;
            }
        }
        public IEnumerable<CssBox> GetChildBoxIter()
        {
            return this._aa_boxes.GetChildBoxIter();
        }
        internal CssBox GetNextNode()
        {
            if (_linkedNode != null && _linkedNode.Next != null)
            {
                return _linkedNode.Next.Value;
            }
            return null;
        }
        internal CssBox GetPrevNode()
        {
            if (_linkedNode != null && _linkedNode.Previous != null)
            {
                return _linkedNode.Previous.Value;
            }
            return null;
        }
        public IEnumerable<CssRun> GetRunIter()
        {
            if (this._aa_contentRuns != null)
            {
                var tmpRuns = this._aa_contentRuns;
                int j = tmpRuns.Count;
                for (int i = 0; i < j; ++i)
                {
                    yield return tmpRuns[i];
                }
            }
        }

        public IEnumerable<CssRun> GetRunBackwardIter()
        {
            if (this._aa_contentRuns != null)
            {
                var tmpRuns = this._aa_contentRuns;
                int j = tmpRuns.Count;
                for (int i = tmpRuns.Count - 1; i >= 0; --i)
                {
                    yield return tmpRuns[i];
                }
            }
        }

        public int ChildCount
        {
            get
            {
                return this._aa_boxes.Count;
            }
        }
        //-----------------------------------
        public CssBox GetFirstChild()
        {
            return this._aa_boxes.GetFirstChild();
        }
        public CssBox GetLastChild()
        {
            return this._aa_boxes.GetLastChild();
        }
        public void AppendChild(CssBox box)
        {
            this.Boxes.AddChild(this, box);
        }
        public void InsertChild(CssBox beforeBox, CssBox box)
        {
            this.Boxes.InsertBefore(this, beforeBox, box);
        }
        //-------------------------------------
        internal void ResetLineBoxes()
        {
            if (this._clientLineBoxes != null)
            {
                _clientLineBoxes.Clear();
            }
            else
            {
                _clientLineBoxes = new LinkedList<CssLineBox>();
            }
        }
        //-------------------------------------
        internal int RowSpan
        {
            get
            {
                return this._rowSpan;
            }
        }
        internal int ColSpan
        {
            get
            {
                return this._colSpan;
            }
        }
        internal void SetRowColSpan(int rowSpan, int colSpan)
        {
            this._rowSpan = rowSpan;
            this._colSpan = colSpan;
        }
        /// <summary>
        /// The margin top value if was effected by margin collapse.
        /// </summary>
        float CollapsedMarginTop
        {
            get;
            set;
        }

        internal SubBoxCollection SubBoxes
        {
            get
            {
                return this._subBoxes;
            }
            set
            {
                this._subBoxes = value;
            }
        }



#if DEBUG
        internal void dbugChangeSiblingOrder(int siblingIndex)
        {
            if (siblingIndex < 0)
            {
                throw new Exception("before box doesn't exist on parent");
            }
            this._parentBox.Boxes.dbugChangeSiblingIndex(_parentBox,this, siblingIndex);
        }
#endif

    }

}