using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace SampleProject
{
	public partial class MainPage : PhoneApplicationPage
	{
		public class DataObject
		{
			public string Title { get; set; }
			public string Body { get; set; }
		}
		private Random r = new Random();
		private ObservableCollection<DataObject> data = new ObservableCollection<DataObject>();
		private const string LoremIpsum = "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.";

		public MainPage()
		{
			InitializeComponent();
			DataContext = data;
		}

		protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);
			CreateData(20);
		}

		private void CreateData(int count)
		{
			for (int i = 0; i < count; i++)
			{
				DataObject obj = new DataObject();
				obj.Title = string.Format("Item {0}", data.Count + 1);
				obj.Body = LoremIpsum.Substring(0, 30 + r.Next(LoremIpsum.Length - 30));
				data.Insert(0, obj);
			}
			list.PullSubtext = string.Format("Last updated: ");
		}

		private void list_PullRefresh(object sender, EventArgs e)
		{
			//Simulate loading data that takes some time:
			DispatcherTimer timer = new DispatcherTimer()
			{
				Interval = TimeSpan.FromSeconds(3)
			};
			timer.Tick += (a, b) =>
				{
					timer.Stop();
					CreateData(10);
					progressBar.IsIndeterminate = false;
					progressBar.Visibility = System.Windows.Visibility.Collapsed;
				};
			timer.Start();
			progressBar.IsIndeterminate = true;
			progressBar.Visibility = System.Windows.Visibility.Visible;
		}
	}
}