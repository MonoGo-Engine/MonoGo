﻿using Microsoft.Xna.Framework;
using MonoGo.Engine.EC;
using MonoGo.Engine.UI.Defs;
using MonoGo.Engine.UI.Utils;
using System;
using System.Runtime.CompilerServices;

namespace MonoGo.Engine.UI.Controls
{
    /// <summary>
    /// A panel is a container for other controls.
    /// </summary>
    public class Panel : Control
    {
        // so that scrollbars will work
        /// <inheritdoc/>
        protected override bool WalkInternalChildren => true;

        /// <summary>
        /// Vertical scrollbar entity, if set.
        /// </summary>
        public Slider? VerticalScrollbar { get; private set; }

        /// <inheritdoc/>
        protected override bool HaveScrollbars => VerticalScrollbar != null;

        /// <summary>
        /// If true, when the scrollbar value changes the offset of the child controls will smoothly interpolate to the new offset.
        /// If false, offset will be set immediately.
        /// </summary>
        public bool InterpolateScrollbarOffset = true;

        /// <summary>
        /// Interpolation speed, if scrollbar position is interpolated.
        /// </summary>
        public float ScrollbarInterpolationSpeed = 10f;

        // scrollbar interpolation value
        float _scrollbarOffset = 0f;

        /// <summary>
        /// Create the panel.
        /// </summary>
        /// <param name="stylesheet">Panel stylesheet.</param>
        public Panel(StyleSheet? stylesheet, Entity? owner = null) : base(stylesheet, owner) 
        {
        }

        /// <summary>
        /// Create the panel with default stylesheets.
        /// </summary>
        public Panel(Entity? owner = null) : this(UISystem.DefaultStylesheets.Panels, owner)
        {
        }

        /// <summary>
        /// Scroll to end if this panel has scrollbars.
        /// </summary>
        public void ScrollToEnd()
        {
            VerticalScrollbar?.ScrollToEnd();
        }

        /// <inheritdoc/>
        internal override void PerformMouseWheelScroll(int val)
        {
            if ((VerticalScrollbar != null) && (VerticalScrollbar.Visible))
            {
                VerticalScrollbar.PerformMouseWheelScroll(val);
            }
            else
            {
                base.PerformMouseWheelScroll(val);
            }
        }

        /// <inheritdoc/>
        protected override void Update(float dt)
        {
            base.Update(dt);

            // update scrollbar
            if ((VerticalScrollbar != null) && IsCurrentlyVisible())
            {
                // scrollbar max value
                if (_autoSetScrollbarMax)
                {
                    var maxScrollbarHeight = CalculateMaxScrollbarValue();
                    if (maxScrollbarHeight > 1)
                    {
                        VerticalScrollbar.MaxValue = maxScrollbarHeight;
                        VerticalScrollbar.MouseWheelStep = -Math.Clamp(maxScrollbarHeight / 10, 1, 100);
                        VerticalScrollbar.Enabled = true;
                    }
                }

                // current scroll value
                float scrollbarNewValue = -VerticalScrollbar.Value;
                _scrollbarOffset = InterpolateScrollbarOffset ? MathUtils.Lerp(_scrollbarOffset, scrollbarNewValue, dt * ScrollbarInterpolationSpeed) : scrollbarNewValue;
            }
        }

        /// <summary>
        /// Called after drawing a child.
        /// </summary>
        protected override void PostDrawingChild(DrawMethodResult? drawResult)
        {
            if (_autoSetScrollbarMax &&  (VerticalScrollbar != null) && (drawResult != null))
            {
                _maxHeightForScrollbar = Math.Max(_maxHeightForScrollbar, drawResult.Value.BoundingRect.Bottom - LastInternalBoundingRect.Top - LastInternalBoundingRect.Height);
            }
        }
        int _maxHeightForScrollbar = 1;

        /// <summary>
        /// Calculate scrollbar max value.
        /// </summary>
        protected virtual int CalculateMaxScrollbarValue()
        {
            return _maxHeightForScrollbar;
        }

        /// <inheritdoc/>
        protected override MeasureVector GetDefaultControlTypeSize()
        {
            var ret = new MeasureVector();
            ret.SetPixels(400, 400);
            return ret;
        }

        /// <inheritdoc/>
        protected override Point GetScrollOffset()
        {
            if (VerticalScrollbar != null)
            {
                return new Point(0, (int)_scrollbarOffset);
            }
            return Point.Zero;
        }

        /// <inheritdoc/>
        protected override Sides GetScrollExtraPadding()
        {
            if (VerticalScrollbar != null && VerticalScrollbar.Visible)
            {
                if (VerticalScrollbar.Anchor == Anchor.TopLeft || VerticalScrollbar.Anchor == Anchor.CenterLeft || VerticalScrollbar.Anchor == Anchor.BottomLeft)
                {
                    return new Sides(VerticalScrollbar.LastBoundingRect.Width + VerticalScrollbar.GetMarginAfter().X, 0, 0, 0);
                }
                else if (VerticalScrollbar.Anchor == Anchor.TopRight || VerticalScrollbar.Anchor == Anchor.CenterRight || VerticalScrollbar.Anchor == Anchor.BottomRight)
                {
                    return new Sides(0, VerticalScrollbar.LastBoundingRect.Width + VerticalScrollbar.GetMarginBefore().X, 0, 0);
                }
            }
            return Sides.Zero;
        }

        /// <summary>
        /// Create a vertical scrollbar for this panel with default stylesheet.
        /// </summary>
        /// <param name="autoSetScrollbarMax">If true, will set the scrollbar max value automatically based on panel height vs. most-bottom entity.</param>
        public void CreateVerticalScrollbar(bool autoSetScrollbarMax = true)
        {
            CreateVerticalScrollbar(
                UISystem.DefaultStylesheets.VerticalScrollbars ?? UISystem.DefaultStylesheets.VerticalSliders,
                UISystem.DefaultStylesheets.VerticalScrollbarsHandle ?? UISystem.DefaultStylesheets.VerticalSlidersHandle);
        }

        /// <summary>
        /// Create a vertical scrollbar for this panel.
        /// </summary>
        /// <param name="stylesheet">Vertical scrollbar style.</param>
        /// <param name="handleStylesheet">Vertical scrollbar handle style.</param>
        /// <param name="autoSetScrollbarMax">If true, will set the scrollbar max value automatically based on panel height vs. most-bottom entity.</param>
        public void CreateVerticalScrollbar(StyleSheet? stylesheet, StyleSheet? handleStylesheet, bool autoSetScrollbarMax = true)
        {
            VerticalScrollbar = new(stylesheet, handleStylesheet, Orientation.Vertical, Owner)
            {
                Anchor = Anchor.TopRight,
                IncludeInInternalAutoAnchorCalculation = false,
                Value = 0,
                MaxValue = 0,
                FlippedDirection = true,
                MouseWheelStep = -1,
                IgnoreScrollOffset = true
            };
            _autoSetScrollbarMax = autoSetScrollbarMax;
            AddChildInternal(VerticalScrollbar, true);
            VerticalScrollbar.IgnoreScrollOffset = true;
        }

        // should we auto-set scrollbar max value?
        bool _autoSetScrollbarMax;

        /// <summary>
        /// Remove the vertical scrollbar for this panel, if set.
        /// </summary>
        public void RemoveVerticalScrollbar()
        {
            if (VerticalScrollbar != null)
            {
                RemoveChildInternal(VerticalScrollbar);
                VerticalScrollbar = null;
            }
        }

        /// <inheritdoc/>
        internal override void DoInteractions(InputState inputState)
        {
            base.DoInteractions(inputState);

            // if got scrollbar, apply wheel to it
            if ((inputState.MouseWheelChange != 0) && (VerticalScrollbar != null))
            {
                VerticalScrollbar.PerformMouseWheelScroll(inputState.MouseWheelChange);
            }
        }
    }
}
