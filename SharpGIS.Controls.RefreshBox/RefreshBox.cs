using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace SharpGIS.Controls
{
	/// <summary>
	/// Refresh box. Pull down beyond the top limit on the listbox to 
	/// trigger a refresh, similar to iPhone's lists.
	/// </summary>
	public class RefreshBox : ListBox
	{
		private bool isPulling = false;
		private ScrollViewer ElementScrollViewer;
		private UIElement ElementRelease;

		/// <summary>
		/// Initializes a new instance of the <see cref="RefreshBox"/> class.
		/// </summary>
		public RefreshBox()
		{
			DefaultStyleKey = typeof(RefreshBox);
		}

		/// <summary>
		/// Builds the visual tree for the <see cref="T:System.Windows.Controls.ListBox"/>
		/// control when a new template is applied.
		/// </summary>
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			if (ElementScrollViewer != null)
			{
				ElementScrollViewer.MouseMove -= viewer_MouseMove;
				ElementScrollViewer.ManipulationCompleted -= viewer_ManipulationCompleted;
			}
			ElementScrollViewer = GetTemplateChild("ScrollViewer") as ScrollViewer;
			if (ElementScrollViewer != null)
			{			
				ElementScrollViewer.MouseMove += viewer_MouseMove;
				ElementScrollViewer.ManipulationCompleted += viewer_ManipulationCompleted;
			}
			ElementRelease = GetTemplateChild("ReleaseElement") as UIElement;
				
			ChangeVisualState(false);
		}

		private void ChangeVisualState(bool useTransitions)
		{
			if (isPulling)
			{
				GoToState(useTransitions, "Pulling");
			}
			else
			{
				GoToState(useTransitions, "NotPulling");
			}
		}

		private bool GoToState(bool useTransitions, string stateName)
		{
			return VisualStateManager.GoToState(this, stateName, useTransitions);
		}

		private void viewer_MouseMove(object sender, MouseEventArgs e)
		{
			if (VerticalOffset == 0)
			{
				var p = this.TransformToVisual(ElementRelease).Transform(new Point());
				if (p.Y < -VerticalPullToRefreshDistance)
				{
					if (!isPulling)
					{
						isPulling = true;
						if (EnteredPullRefreshThreshold != null)
						{
							EnteredPullRefreshThreshold(this, EventArgs.Empty);
						}
						ChangeVisualState(true);
					}
				}
				else if (isPulling)
				{
					isPulling = false;
					if (LeftPullRefreshThreshold != null)
					{
						LeftPullRefreshThreshold(this, EventArgs.Empty);
					}
					ChangeVisualState(true);
				}
			}
		}

		private void viewer_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
		{
			var p = this.TransformToVisual(ElementRelease).Transform(new Point());
			if (p.Y < -VerticalPullToRefreshDistance)
			{
				if (PullRefresh != null)
					PullRefresh(this, EventArgs.Empty);
				isPulling = false;
				ChangeVisualState(true);
			}
		}

		private double VerticalOffset
		{
			get
			{
				if (ElementScrollViewer == null) return double.NaN;
				return ElementScrollViewer.VerticalOffset;
			}
		}

		/// <summary>
		/// Distance in pixels to pull down the RefreshBox before a refresh will get initiated.
		/// </summary>
		public double VerticalPullToRefreshDistance
		{
			get { return (double)GetValue(VerticalPullToRefreshDistanceProperty); }
			set { SetValue(VerticalPullToRefreshDistanceProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="VerticalPullToRefreshDistance"/> property
		/// </summary>
		public static readonly DependencyProperty VerticalPullToRefreshDistanceProperty =
			DependencyProperty.Register("VerticalPullToRefreshDistance", typeof(double), typeof(RefreshBox), new PropertyMetadata(20d));

		/// <summary>
		/// Gets or sets the refresh text. Ie "Pull down to refresh".
		/// </summary>
		public string RefreshText
		{
			get { return (string)GetValue(RefreshTextProperty); }
			set { SetValue(RefreshTextProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="RefreshText"/> property
		/// </summary>
		public static readonly DependencyProperty RefreshTextProperty =
			DependencyProperty.Register("RefreshText", typeof(string), typeof(RefreshBox), new PropertyMetadata("Pull down to refresh..."));

		/// <summary>
		/// Gets or sets the release text. Ie "Release to refresh".
		/// </summary>
		public string ReleaseText
		{
			get { return (string)GetValue(ReleaseTextProperty); }
			set { SetValue(ReleaseTextProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="ReleaseText"/> property
		/// </summary>
		public static readonly DependencyProperty ReleaseTextProperty =
			DependencyProperty.Register("ReleaseText", typeof(string), typeof(RefreshBox), new PropertyMetadata("Release to refresh..."));

		/// <summary>
		/// Sub text below Release/Refresh text. For example: Updated last: 12:34pm
		/// </summary>
		public string PullSubtext
		{
			get { return (string)GetValue(PullSubtextProperty); }
			set { SetValue(PullSubtextProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="PullSubtext"/> property
		/// </summary>
		public static readonly DependencyProperty PullSubtextProperty =
			DependencyProperty.Register("PullSubtext", typeof(string), typeof(RefreshBox), null);

		/// <summary>
		/// Triggered when the user requested a refresh.
		/// </summary>
		public event EventHandler PullRefresh;
		/// <summary>
		/// If the user lets go of the screen after this event fires, a PullRefresh event is fired.
		/// </summary>
		public event EventHandler EnteredPullRefreshThreshold;
		/// <summary>
		/// If the user exited the "refresh" area without letting go of the screen.
		/// </summary>
		public event EventHandler LeftPullRefreshThreshold;
	}
}
