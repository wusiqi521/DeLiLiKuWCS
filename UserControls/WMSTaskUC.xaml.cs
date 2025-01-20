using BMHRI.WCS.Server.Models;
using BMHRI.WCS.Server.UserWindows;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BMHRI.WCS.Server.UserControls
{
    /// <summary>
    /// WMSTaskUC.xaml 的交互逻辑
    /// </summary>
    public partial class WMSTaskUC : UserControl
    {
        public ObservableCollection<WMSTask> WMSTaskCollection;
        public ObservableCollection<WCSTask> WCSTaskCollection;
        public WMSTaskUC()
        {
            InitializeComponent();
            try
            {
                WMSTaskCollection = new ObservableCollection<WMSTask>(WMSTasksManager.Instance.WMSTaskList);
                WMSTaskLV.ItemsSource = WMSTaskCollection;

                WCSTaskCollection = new ObservableCollection<WCSTask>(WCSTaskManager.Instance.WCSTaskList);
                WCSTaskLV.ItemsSource = WCSTaskCollection;

                WMSTasksManager.Instance.WMSTaskAdded += WMSTaskAdded;
                WMSTasksManager.Instance.WMSTaskChanged += WMSTaskChanged;
                WMSTasksManager.Instance.WMSTaskDeleted += WMSTaskDeleted;

                WCSTaskManager.Instance.WCSTaskAdded += WCSTaskAdded;
                WCSTaskManager.Instance.WCSTaskChanged += WCSTaskChanged;
                WCSTaskManager.Instance.WCSTaskDeleted += WCSTaskDeleted;
            }
            catch (Exception ex)
            {
                //LogHelper.WriteLog("WMSTaskUC() ", ex);
            }
        }

        private void RefreshBT_Click(object sender, RoutedEventArgs e)
        {
            WMSTaskCollection = new ObservableCollection<WMSTask>(WMSTasksManager.Instance.WMSTaskList);
            WMSTaskLV.ItemsSource = null;
            WMSTaskLV.ItemsSource = WMSTaskCollection;
            TaskNumLB.Content = WMSTaskCollection.Count;
        }

        private void WMSTaskOP_Click(object sender, RoutedEventArgs e)
        {
            if (WMSTaskLV.SelectedItems.Count < 1) return;
            WMSTask wMSTask = (WMSTask)WMSTaskLV.SelectedItems[0];
            WMSTaskOptionWindow wMSTaskOptionWindow = new WMSTaskOptionWindow(wMSTask);
            wMSTaskOptionWindow.ShowDialog();
        }

        private void WMSTaskAdded(object sender, EventArgs e)
        {
            if (e is not WMSTaskEventArgs wMSTaskEventArgs) return;
            Dispatcher.Invoke(new Action(delegate
            {
                WMSTaskCollection.Add(wMSTaskEventArgs.WMSTask);
                TaskNumLB.Content = WMSTaskCollection.Count;
            }));
        }
        private void WMSTaskChanged(object sender, EventArgs e)
        {
            Dispatcher.Invoke(new Action(delegate
            {
                if (e is not WMSTaskEventArgs wMSTaskEventArgs) return;
                foreach (WMSTask wMSTask in WMSTaskCollection)
                {
                    if (wMSTask.WMSSeqID == wMSTaskEventArgs.WMSTask.WMSSeqID)
                    {
                        int index = WMSTaskCollection.IndexOf(wMSTask);
                        WMSTaskCollection.Remove(wMSTask);
                        WMSTaskCollection.Insert(index, wMSTaskEventArgs.WMSTask);
                        break;
                    }
                }
            }));
        }
        private void WMSTaskDeleted(object sender, EventArgs e)
        {
            Dispatcher.Invoke(new Action(delegate
            {
                if (!(e is WMSTaskEventArgs wMSTaskEventArgs)) return;
                WMSTaskCollection.Remove(wMSTaskEventArgs.WMSTask);
                TaskNumLB.Content = WMSTaskCollection.Count;
            }));
        }
        private void WCSTaskAdded(object sender, EventArgs e)
        {
            if (!(e is WCSTaskEventArgs wCSTaskEventArgs)) return;
            Dispatcher.Invoke(new Action(delegate
            {
                WCSTaskCollection.Add(wCSTaskEventArgs.WCSTask);
                WCSTaskNumLB.Content = WCSTaskCollection.Count;
            }));
        }
        private void WCSTaskChanged(object sender, EventArgs e)
        {
            Dispatcher.Invoke(new Action(delegate
            {
                if (!(e is WCSTaskEventArgs wCSTaskEventArgs)) return;
                foreach (WCSTask wCSTask in WCSTaskCollection)
                {
                    if (wCSTask.WCSSeqID == wCSTaskEventArgs.WCSTask.WCSSeqID)
                    {
                        int index = WCSTaskCollection.IndexOf(wCSTask);
                        WCSTaskCollection.Remove(wCSTask);
                        WCSTaskCollection.Insert(index, wCSTaskEventArgs.WCSTask);
                        break;
                    }
                }
            }));
        }
        private void WCSTaskDeleted(object sender, EventArgs e)
        {
            Dispatcher.Invoke(new Action(delegate
            {
                if (!(e is WCSTaskEventArgs wCSTaskEventArgs)) return;
                WCSTaskCollection.Remove(wCSTaskEventArgs.WCSTask);
                WCSTaskNumLB.Content = WCSTaskCollection.Count;
            }));
        }
        private void WCSRefreshBT_Click(object sender, RoutedEventArgs e)
        {
            WCSTaskCollection = new ObservableCollection<WCSTask>(WCSTaskManager.Instance.WCSTaskList);
            WCSTaskLV.ItemsSource = null;
            WCSTaskLV.ItemsSource = WCSTaskCollection;
            WCSTaskNumLB.Content = WCSTaskCollection.Count;
        }
        private void WMSTaskLV_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListViewItem lv = (ListViewItem)sender;
            if (lv is null) return;
            if (lv.DataContext is WMSTask wMSTask)
            {
                WCSTaskCollection = new ObservableCollection<WCSTask>(WCSTaskManager.Instance.WCSTaskList.FindAll(x => x.WMSSeqID == wMSTask.WMSSeqID));
                WCSTaskLV.ItemsSource = null;
                WCSTaskLV.ItemsSource = WCSTaskCollection;
            }
            else if (lv.DataContext is WCSTask wCSTask)
            {
                WCSTaskOptionWindow wCSTaskOptionWindow = new WCSTaskOptionWindow();
                wCSTaskOptionWindow.wCSTask = wCSTask;
                wCSTaskOptionWindow.ShowDialog();
            }
        }

        private void FindTaskFromWMSBT_Click(object sender, RoutedEventArgs e)
        {
            WMSTaskCollection = new ObservableCollection<WMSTask>(WMSTasksManager.Instance.WMSTaskList.FindAll(x => x.PalletNum.Contains(PalletNumTBM.Text.Trim())));
            WMSTaskLV.ItemsSource = null;
            WMSTaskLV.ItemsSource = WMSTaskCollection;
            TaskNumLB.Content = WMSTaskCollection.Count;
        }

        private void FindTaskFromWCSBT_Click(object sender, RoutedEventArgs e)
        {
            WCSTaskCollection = new ObservableCollection<WCSTask>(WCSTaskManager.Instance.WCSTaskList.FindAll(x => x.PalletNum.Contains(PalletNumTBC.Text.Trim())));
            WCSTaskLV.ItemsSource = null;
            WCSTaskLV.ItemsSource = WCSTaskCollection;
            WCSTaskNumLB.Content = WCSTaskCollection.Count;
        }

        private void OutBoundTaskWaittingBT_Click(object sender, RoutedEventArgs e)
        {
            WCSTaskCollection = new ObservableCollection<WCSTask>(WCSTaskManager.Instance.WCSTaskList.FindAll(x => x.TaskType == WCSTaskTypes.DDJUnstack && (x.TaskStatus == WCSTaskStatus.Waiting || x.TaskStatus == WCSTaskStatus.Cannot || x.TaskStatus == WCSTaskStatus.StackChged)));
            WCSTaskLV.ItemsSource = null;
            WCSTaskLV.ItemsSource = WCSTaskCollection;
            WCSTaskNumLB.Content = WCSTaskCollection.Count;
        }

        private void DDJTaskFaultBT_Click(object sender, RoutedEventArgs e)
        {
            WCSTaskCollection = new ObservableCollection<WCSTask>(WCSTaskManager.Instance.WCSTaskList.FindAll(x => (x.TaskType == WCSTaskTypes.DDJUnstack || x.TaskType == WCSTaskTypes.DDJStack || x.TaskType == WCSTaskTypes.DDJStackMove || x.TaskType == WCSTaskTypes.DDJDirect) && (x.TaskStatus == WCSTaskStatus.Fault || x.TaskStatus == WCSTaskStatus.UnStackEmpty || x.TaskStatus == WCSTaskStatus.UnStackEmptyConfirm || x.TaskStatus == WCSTaskStatus.PickEmpty || x.TaskStatus == WCSTaskStatus.PickEmptyConfirm || x.TaskStatus == WCSTaskStatus.StackDouble || x.TaskStatus == WCSTaskStatus.StackDoubleConfirm || x.TaskStatus == WCSTaskStatus.FarOutboundClearHave || x.TaskStatus == WCSTaskStatus.FarOutboundClearHaveConfirm)));
            WCSTaskLV.ItemsSource = null;
            WCSTaskLV.ItemsSource = WCSTaskCollection;
            WCSTaskNumLB.Content = WCSTaskCollection.Count;
        }
    }
}
