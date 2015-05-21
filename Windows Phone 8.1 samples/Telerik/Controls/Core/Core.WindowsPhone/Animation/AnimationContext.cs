﻿using System;
using System.Globalization;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace Telerik.Core
{
    /// <summary>
    ///		A helper class, used for chain-creation and update of storyboards in a jQuery way.
    ///		This class is for internal use only.
    /// </summary>
    /// <remarks>
    ///		The Result storyboard is a collection of DoubleAnimationUsingKeyFrame collection.
    /// </remarks>
    public class AnimationContext
    {
        internal AnimationContext(UIElement target, Storyboard storyboard, params object[] args) : this(target)
        {
            this.Storyboard = storyboard;
            this.Arguments = args;
        }

        internal AnimationContext(UIElement target)
        {
            this.Target = target;
            this.EnsureDefaultTransforms();
            this.EnsurePlaneProjection();
        }

        /// <summary>
        /// Gets or sets the resultant Storyboard for the AnimationContext.
        /// </summary>
        public Storyboard Storyboard
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets optional arguments associated with the animation.
        /// </summary>
        public object[] Arguments
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the UIElement instance, which is the actual target of the animation.
        /// </summary>
        public UIElement Target
        {
            get;
            internal set;
        }

        internal void SingleProperty(string propertyPath, params double[] args)
        {
            this.VerifyArguments(args);

            UIElement element = this.Target;

            var moveX = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTarget(moveX, element);
            Storyboard.SetTargetProperty(moveX, propertyPath);

            for (int i = 0; i < args.Length; i += 2)
            {
                moveX.KeyFrames.Add(new EasingDoubleKeyFrame()
                {
                    KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(args[i])),
                    Value = args[i + 1]
                });
            }

            this.Storyboard.Children.Add(moveX);
        }

        internal void Origin(double x1, double x2)
        {
            this.Target.RenderTransformOrigin = new Point(x1, x2);
        }

        internal void EnsurePlaneProjection()
        {
            this.Target.EnsurePlaneProjection();
        }

        internal void Scale(params double[] args)
        {
            this.VerifyArguments(args);

            UIElement element = this.Target;

            var scaleX = new DoubleAnimationUsingKeyFrames();
            scaleX.EnableDependentAnimation = true;
            Storyboard.SetTarget(scaleX, element);
            Storyboard.SetTargetProperty(scaleX, "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)");

            var scaleY = new DoubleAnimationUsingKeyFrames();
            scaleY.EnableDependentAnimation = true;
            Storyboard.SetTarget(scaleY, element);
            Storyboard.SetTargetProperty(scaleY, "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)");

            for (int i = 0; i < args.Length; i += 2)
            {
                scaleX.KeyFrames.Add(new EasingDoubleKeyFrame()
                {
                    KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(args[i])),
                    Value = args[i + 1]
                });

                scaleY.KeyFrames.Add(new EasingDoubleKeyFrame()
                {
                    KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(args[i])),
                    Value = args[i + 1]
                });
            }

            this.Storyboard.Children.Add(scaleX);
            this.Storyboard.Children.Add(scaleY);
        }

        internal void InitializeCenterOfRotationX(double centerX)
        {
            (this.Target.Projection as PlaneProjection).CenterOfRotationX = centerX;
        }

        internal void InitializeCenterOfRotationY(double centerY)
        {
            (this.Target.Projection as PlaneProjection).CenterOfRotationY = centerY;
        }

        internal void InitializeCenterOfRotationZ(double centerZ)
        {
            (this.Target.Projection as PlaneProjection).CenterOfRotationZ = centerZ;
        }
                
        internal void InitializeRotationX(double angle)
        {
            (this.Target.Projection as PlaneProjection).RotationX = angle;
        }

        internal void InitializeRotationY(double angle)
        {
            (this.Target.Projection as PlaneProjection).RotationY = angle;
        }

        internal void InitializeRotationZ(double angle)
        {
            (this.Target.Projection as PlaneProjection).RotationZ = angle;
        }

        internal void RotationX(params double[] args)
        {
            this.SingleProperty("(UIElement.Projection).(PlaneProjection.RotationX)", args);
        }

        internal void RotationY(params double[] args)
        {
            this.SingleProperty("(UIElement.Projection).(PlaneProjection.RotationY)", args);
        }

        internal void RotationZ(params double[] args)
        {
            this.SingleProperty("(UIElement.Projection).(PlaneProjection.RotationZ)", args);
        }

        internal void InitializeScaleX(double value)
        {
            ((this.Target.RenderTransform as TransformGroup).Children[0] as ScaleTransform).ScaleX = value;
        }

        internal void InitializeScaleY(double value)
        {
            ((this.Target.RenderTransform as TransformGroup).Children[0] as ScaleTransform).ScaleY = value;
        }

        internal void ScaleX(params double[] args)
        {
            this.SingleProperty("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)", args);
        }

        internal void ScaleY(params double[] args)
        {
            this.SingleProperty("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)", args);
        }

        internal void InitializeMoveX(double value)
        {
            ((this.Target.RenderTransform as TransformGroup).Children[3] as TranslateTransform).X = value;
        }

        internal void InitializeMoveY(double value)
        {
            ((this.Target.RenderTransform as TransformGroup).Children[3] as TranslateTransform).Y = value;
        }

        internal void MoveX(params double[] args)
        {
            this.SingleProperty("(UIElement.RenderTransform).(TransformGroup.Children)[3].(TranslateTransform.X)", args);
        }

        internal void MoveY(params double[] args)
        {
            this.SingleProperty("(UIElement.RenderTransform).(TransformGroup.Children)[3].(TranslateTransform.Y)", args);
        }

        internal void InitializeOpacity(double value)
        {
            this.Target.Opacity = value;
        }

        internal void Opacity(params double[] args)
        {
            this.SingleProperty("(UIElement.Opacity)", args);
        }

        //TODO:
        //internal void InitializeOpacityMaskRelativeMoveX(double value)
        //{
        //    (this.Target.OpacityMask.RelativeTransform as TranslateTransform).X = value;
        //}

        //internal void InitializeOpacityMaskRelativeMoveY(double value)
        //{
        //    (this.Target.OpacityMask.RelativeTransform as TranslateTransform).Y = value;
        //}

        //internal void OpacityMaskRelativeMoveY(params double[] args)
        //{
        //    this.SingleProperty("(UIElement.OpacityMask).(Brush.RelativeTransform).(TranslateTransform.Y)", args);
        //}

        //internal void OpacityMaskRelativeMoveX(params double[] args)
        //{
        //    this.SingleProperty("(UIElement.OpacityMask).(Brush.RelativeTransform).(TranslateTransform.X)", args);
        //}

        //internal void InitializeOpacityMaskMoveX(double value)
        //{
        //    (this.Target.OpacityMask.Transform as TranslateTransform).X = value;
        //}

        //internal void InitializeOpacityMaskMoveY(double value)
        //{
        //    (this.Target.OpacityMask.Transform as TranslateTransform).Y = value;
        //}

        //internal void OpacityMaskMoveY(params double[] args)
        //{
        //    this.SingleProperty("(UIElement.OpacityMask).(Brush.Transform).(TranslateTransform.Y)", args);
        //}

        //internal void OpacityMaskMoveX(params double[] args)
        //{
        //    this.SingleProperty("(UIElement.OpacityMask).(Brush.Transform).(TranslateTransform.X)", args);
        //}

        internal void InitializeHeight(double value)
        {
            (this.Target as FrameworkElement).Height = value;
        }

        internal void InitializeWidth(double value)
        {
            (this.Target as FrameworkElement).Width = value;
        }

        internal void Height(params double[] args)
        {
            this.SingleProperty("(FrameworkElement.Height)", args);
        }

        internal void Width(params double[] args)
        {
            this.SingleProperty("(FrameworkElement.Width)", args);
        }

        internal void EaseAll(EasingFunctionBase easing)
        {
            this.EaseStoryboard(this.Storyboard, easing);
        }

        internal void EnsureDefaultTransforms()
        {
            this.Target.EnsureDefaultTransforms();
        }

        private void EaseStoryboard(Storyboard target, EasingFunctionBase easing)
        {
            for (int childIndex = 0; childIndex < target.Children.Count; childIndex++)
            {
                var child = target.Children[childIndex];
                if (child is Storyboard)
                {
                    this.EaseStoryboard(child as Storyboard, easing);
                }
                else if (child is DoubleAnimationUsingKeyFrames)
                {
                    this.EaseDoubleAnimation(child as DoubleAnimationUsingKeyFrames, easing);
                }
            }
        }

        private void EaseDoubleAnimation(DoubleAnimationUsingKeyFrames target, EasingFunctionBase easing)
        {
            foreach (EasingDoubleKeyFrame keyFrame in target.KeyFrames)
            {
                keyFrame.EasingFunction = easing;
            }
        }

        //TODO:
        //private void SingleProperty(PropertyPath propertyPath, params double[] args)
        //{
        //    this.VerifyArguments(args);

        //    UIElement element = this.Target;

        //    var moveX = new DoubleAnimationUsingKeyFrames();
        //    Storyboard.SetTarget(moveX, element);
        //    Storyboard.SetTargetProperty(moveX, propertyPath);

        //    for (int i = 0; i < args.Length; i += 2)
        //    {
        //        moveX.KeyFrames.Add(new EasingDoubleKeyFrame()
        //        {
        //            KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(args[i])),
        //            Value = args[i + 1]
        //        });
        //    }

        //    this.Storyboard.Children.Add(moveX);
        //}

        private void VerifyArguments(Array args)
        {
            if (args == null || args.Length % 2 != 0)
            {
                throw new InvalidOperationException("Params should come in a time-value pair");
            }
        }
    }
}
