﻿//BSD 2014,WinterDev
using LayoutFarm.Drawing;

namespace HtmlRenderer.Boxes
{
    class CssBlockRun : CssRun
    {

        public CssBlockRun(CssBox blockBox)
            : base(CssRunKind.BlockRun)
        {
            this.SetOwner(blockBox);
        }
        public CssBox BlockBox
        {
            get { return this.OwnerBox; }
        }

    }
}