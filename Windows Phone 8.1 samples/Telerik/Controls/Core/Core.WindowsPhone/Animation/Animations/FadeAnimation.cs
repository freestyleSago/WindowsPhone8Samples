
using Windows.UI.Xaml;
namespace Telerik.Core
{
	/// <summary>
	/// Fade animation for showing/hiding elements.
	/// </summary>
	public class RadFadeAnimation : RadAnimation
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RadFadeAnimation"/> class.
		/// </summary>
		public RadFadeAnimation()
		{
			this.EndOpacity = 1.0;
			this.StartOpacity = 0.0;
		}

		/// <summary>
		/// Gets or sets a value from which the animation will start.
		/// </summary>
		public double StartOpacity
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value will be applied when the animation finishes.
		/// </summary>
		public double EndOpacity
		{
			get;
			set;
		}

        /// <summary>
        /// Removes any property modifications, applied to the specified element by this instance.
        /// </summary>
        /// <param name="target">The element which property values are to be cleared.</param>
        /// <remarks>
        /// It is assumed that the element has been previously animated by this animation.
        /// </remarks>
        public override void ClearAnimation(UIElement target)
        {
            target.ClearValue(UIElement.OpacityProperty);
        }

        /// <summary>
        /// Sets the initial animation values to the provided target element.
        /// </summary>
        /// <param name="target"></param>
        public override void ApplyInitialValues(UIElement target)
        {
            base.ApplyInitialValues(target);
            AnimationContext context = new AnimationContext(target);
            context.InitializeOpacity(this.StartOpacity);
        }

        /// <summary>
        /// Creates a new instance of this animation that is the reverse of this instance.
        /// </summary>
        /// <returns>A new instance of this animation that is the reverse of this instance.</returns>
        public override RadAnimation CreateOpposite()
        {
            RadFadeAnimation opposite = base.CreateOpposite() as RadFadeAnimation;
            double tmp = opposite.StartOpacity;
            opposite.StartOpacity = opposite.EndOpacity;
            opposite.EndOpacity = tmp;

            return opposite;
        }

        /// <summary>
        /// Applies already stored (if any) animated values.
        /// </summary>
        /// <param name="info"></param>
        protected internal override void ApplyAnimationValues(PlayAnimationInfo info)
        {
            double opacity = 0;
            if (this.GetAutoReverse())
            {
                opacity = this.StartOpacity;
            }
            else
            {
                opacity = this.EndOpacity;
            }

            if (info.Target.Opacity != opacity)
            {
                info.Target.Opacity = opacity;
            }
        }

        /// <summary>
        /// Core update routine.
        /// </summary>
        /// <param name="context">The context that holds information about the animation.</param>
        protected override void UpdateAnimationOverride(AnimationContext context)
		{
            context.Opacity(0, this.StartOpacity, this.Duration.TimeSpan.TotalSeconds, this.EndOpacity);
            base.UpdateAnimationOverride(context);
        }
	}
}
