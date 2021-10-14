using System;
using Xamarin.Forms.Platform.iOS;
using UIKit;
using PrescriptionFiller;
using Xamarin.Forms;
using CoreAnimation;
using CoreGraphics;

[assembly: ExportRenderer(typeof(PopupPage), typeof(PopupPageRenderer))]
namespace PrescriptionFiller
{
    public class PopupPageRenderer : PageRenderer
    {
        public class CustomTransitionDelegate : UIViewControllerTransitioningDelegate
        {
            private CustomTransitionAnimator customTransitionAnimator;

            public CustomTransitionDelegate()
            {

            }

            public override IUIViewControllerAnimatedTransitioning GetAnimationControllerForPresentedController (UIViewController presented, UIViewController presenting, UIViewController source)
            {
                customTransitionAnimator = new CustomTransitionAnimator (presented, presenting, source);
                customTransitionAnimator.IsPresentation = true;
                return customTransitionAnimator;
            }

            public override IUIViewControllerAnimatedTransitioning GetAnimationControllerForDismissedController (UIViewController dismissed)
            {
                customTransitionAnimator.IsPresentation = false;
                return customTransitionAnimator;
            }
        }

        public class CustomTransitionAnimator : UIViewControllerAnimatedTransitioning
        {
            private UIView dimmingView;
            private float duration = 0f;

            public bool IsPresentation
            {
                get;
                set;
            }

            public UIViewController ContainerView 
            {
                get;
                set;
            }

            public UIViewController PresentedViewController 
            {
                get;
                set;
            }

            public UIViewController PresentingViewController 
            {
                get;
                set;
            }

            public CustomTransitionAnimator(UIViewController presentedViewController, UIViewController presentingViewController, UIViewController source)
            {
                PresentedViewController = presentedViewController;
                PresentingViewController = presentingViewController;
                ContainerView = source;
                SetUpDimmingView ();
            }

            void SetUpDimmingView ()
            {
                this.dimmingView = new UIView ();
                this.dimmingView.BackgroundColor = UIColor.Black.ColorWithAlpha (0.4f);
                this.dimmingView.Alpha = 0;
            }

            public override void AnimateTransition (IUIViewControllerContextTransitioning transitionContext)
            {
                var fromVC = transitionContext.GetViewControllerForKey (UITransitionContext.FromViewControllerKey);
                var fromView = fromVC.View;
                var toVC = transitionContext.GetViewControllerForKey (UITransitionContext.ToViewControllerKey);
                var toView = toVC.View;
                var containerView = transitionContext.ContainerView;

                var isPresentation = this.IsPresentation;

                if (isPresentation)
                {
                    PresentationTransitionWillBegin();
                    toView.BackgroundColor = UIColor.Clear;
                    containerView.AddSubview(PresentedViewController.View);
                }
                else
                {
                    DismissalTransitionWillBegin();
                    fromView.RemoveFromSuperview();
                }

                transitionContext.CompleteTransition(true);
            }

            public override double TransitionDuration (IUIViewControllerContextTransitioning transitionContext)
            {
                return duration;
            }

            public void PresentationTransitionWillBegin ()
            {
                this.dimmingView.Frame = this.ContainerView.View.Bounds;
                this.dimmingView.Alpha = 1;
                this.ContainerView.View.AddSubview(this.dimmingView);
            }

            public void DismissalTransitionWillBegin ()
            {
                this.dimmingView.Alpha = 0;
                this.dimmingView.RemoveFromSuperview();
            }
        }

        public PopupPageRenderer() : base()
        {
            ModalPresentationStyle = UIModalPresentationStyle.Custom;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            ModalPresentationStyle = UIModalPresentationStyle.Custom;
        }
                        
        public override void WillMoveToParentViewController(UIViewController parent)
        {
            base.WillMoveToParentViewController(parent);

            if (parent != null)
            {
                parent.ModalPresentationStyle = ModalPresentationStyle;
                parent.TransitioningDelegate = new CustomTransitionDelegate();
            }
        }
    }
}

