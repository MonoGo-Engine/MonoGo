﻿using MonoGo.Engine.EC;
using MonoGo.Engine.UI.Defs;
using MonoGo.Engine.UI.Utils;

namespace MonoGo.Engine.UI.Controls
{
    /// <summary>
    /// Progress bars are like sliders, but instead of moving a handle they 'fill' an internal entity.
    /// These controls are useful to show progress, health bars, etc.
    /// </summary>
    /// <remarks>By default, progress bars have 'IgnoreInteractions' set to true. If you want the progress bar to behave like a slider and allow users to change its value, set it to false.</remarks>
    public class ProgressBar : Slider
    {
        /// <summary>
        /// Create the progress bar.
        /// </summary>
        /// <param name="stylesheet">Progress bar stylesheet.</param>
        /// <param name="fillStylesheet">Progress bar fill stylesheet.</param>
        /// <param name="orientation">Progress bar orientation.</param>
        public ProgressBar(StyleSheet? stylesheet, StyleSheet? fillStylesheet, Orientation orientation = Orientation.Horizontal, Entity? owner = null) : base(stylesheet, fillStylesheet, orientation, owner)
        {
            Handle.IgnoreInteractions = IgnoreInteractions = true;
            Handle.Anchor = Handle.StyleSheet?.DefaultAnchor ?? stylesheet?.DefaultAnchor ?? ((orientation == Orientation.Horizontal) ? Anchor.CenterLeft : Anchor.TopCenter);
        }

        /// <summary>
        /// Create the progress bar with default stylesheets.
        /// </summary>
        /// <param name="orientation">Progress bar orientation.</param>
        public ProgressBar(Orientation orientation = Orientation.Horizontal, Entity ? owner = null) :
            this(
                (orientation == Orientation.Horizontal) ? UISystem.DefaultStylesheets.HorizontalProgressBars : UISystem.DefaultStylesheets.VerticalProgressBars,
                (orientation == Orientation.Horizontal) ? UISystem.DefaultStylesheets.HorizontalProgressBarsFill : UISystem.DefaultStylesheets.VerticalProgressBarsFill,
                orientation,
                owner)
        {
        }

        /// <inheritdoc/>
        protected override void Update(float dt)
        {
            base.Update(dt);
            Handle.IgnoreInteractions = IgnoreInteractions;
        }

        /// <inheritdoc/>
        protected override void UpdateHandle(float dt)
        {
            var valuePercent = ValuePercent;
            if (Orientation == Orientation.Horizontal)
            {
                if (InterpolateHandlePosition)
                {
                    var currValue = Handle.Size.X.Value;
                    Handle.Size.X.SetPercents(MathUtils.Lerp(currValue, valuePercent * 100f, dt * HandleInterpolationSpeed));
                }
                else
                {
                    Handle.Size.X.SetPercents(valuePercent * 100f);
                }
            }
            else
            {
                if (InterpolateHandlePosition)
                {
                    var currValue = Handle.Size.Y.Value;
                    Handle.Size.Y.SetPercents(MathUtils.Lerp(currValue, valuePercent * 100f, dt * HandleInterpolationSpeed));
                }
                else
                {
                    Handle.Size.Y.SetPercents(valuePercent * 100f);
                }
            }
        }
    }
}
