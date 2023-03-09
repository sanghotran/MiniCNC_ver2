using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LibUsbDotNet;
using LibUsbDotNet.Main;

namespace MiniCNC_ver2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        // Property Change
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        // Chat box
        private ObservableCollection<ChatItem> PCchatItems = new ObservableCollection<ChatItem>();
        private ObservableCollection<ChatItem> MCUchatItems = new ObservableCollection<ChatItem>();

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            firtLoad();
        }
        #region Fields
        private const int CNC_VID = 1156;
        private const int CNC_PID = 22353;

        public static UsbDevice myUsbDevice;
        public static UsbDeviceFinder myUsbFinder = new UsbDeviceFinder(CNC_VID, CNC_PID);
        UsbEndpointReader reader;
        UsbEndpointWriter writer;

        private ChatItem PCchatItem = new ChatItem("PC");
        private ChatItem mainMCUchatItem = new ChatItem("MCU Main");
        private ChatItem controlMCUchatItem = new ChatItem("MCU Control");
        #endregion

        #region Properites
        private bool _IsStarted;
        public bool IsStarted
        {
            get => _IsStarted;
            set
            {
                _IsStarted = value;
                OnPropertyChanged();
            }
        }
        private bool _IsPaused;
        public bool IsPaused
        {
            get => _IsPaused;
            set
            {
                _IsPaused = value;
                OnPropertyChanged();
            }
        }
        private bool _IsConnected;
        public bool IsConnected
        {
            get => _IsConnected;
            set
            {
                _IsConnected = value;
                OnPropertyChanged();
            }
        }
        private bool _IsLaptop;
        public bool IsLaptop
        {
            get => _IsLaptop;
            set
            {
                _IsLaptop = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Functions
        private void firtLoad()
        {
            IsStarted = false;
            IsPaused = false;
            IsConnected = false;
            IsLaptop = true;

            PCChatPage.Visibility = Visibility.Visible;
            MCUChatPage.Visibility = Visibility.Hidden;

            PCchatMCU.ItemsSource = PCchatItems;
            MCUchatMCU.ItemsSource = MCUchatItems;

            showFile();
        }

        //showMessage(controlMCUchatItem, scrollviewMCU, MCUchatItems, "X Y Z");
        private void showMessage(ChatItem chatitem, ScrollViewer scrollViewer, ObservableCollection<ChatItem> chatItems,string message)
        {
            chatitem.UpdateChat(message);
            chatItems.Add(chatitem);
            scrollViewer.ScrollToEnd();
        }

        // Windows Explorer
        private void showFile()
        {
            string path = @"E:\Project\MiniCNC_ver2\Ref\gcode";
            string[] files = Directory.GetFiles(path);
            List<FileItem> fileItems = new List<FileItem>();
            foreach (string file in files)
            {
                FileInfo fileInfo = new FileInfo(file);
                fileItems.Add(new FileItem(fileInfo.Name, fileInfo.Length, fileInfo.FullName));
            }
            PC_fileList.ItemsSource = fileItems;

        }
        #endregion

        #region Event
        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.DragMove();
        }
        private void CloseApp(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
        private void MinimizeApp(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        private void OpenFile(object sender, MouseButtonEventArgs e)
        {

        }
        private void Setting(object sender, MouseButtonEventArgs e)
        {

        }
        private void Send(object sender, MouseButtonEventArgs e)
        {

        }
        private void Home(object sender, MouseButtonEventArgs e)
        {
            
        }
        private void Start(object sender, MouseButtonEventArgs e)
        {
            IsStarted = !IsStarted;
        }
        private void Pause(object sender, MouseButtonEventArgs e)
        {
            IsPaused = !IsPaused;
        }
        private void Connect(object sender, MouseButtonEventArgs e)
        {
            if (!IsConnected) //kết nối
            {
                try
                {
                    myUsbDevice = UsbDevice.OpenUsbDevice(myUsbFinder);
                    if (myUsbDevice == null) throw new Exception("Device Not Found.");
                    IUsbDevice wholeUsbDevice = myUsbDevice as IUsbDevice;
                    if (!ReferenceEquals(wholeUsbDevice, null))
                    {
                        wholeUsbDevice.SetConfiguration(1);
                        wholeUsbDevice.ClaimInterface(0);
                    }
                    reader = myUsbDevice.OpenEndpointReader(ReadEndpointID.Ep01);
                    writer = myUsbDevice.OpenEndpointWriter(WriteEndpointID.Ep01);
                    reader.DataReceived += (OnRxEndPointData);
                    reader.DataReceivedEnabled = true;
                    IsConnected = true;
                }
                catch
                {

                }
            }
            else // ngắt kết nối
            {
                reader.DataReceivedEnabled = false;
                reader.DataReceived -= (OnRxEndPointData);
                reader.Dispose();
                writer.Dispose();
                if (myUsbDevice != null)
                {
                    if (myUsbDevice.IsOpen)
                    {
                        IUsbDevice wholeUsbDevice = myUsbDevice as IUsbDevice;
                        if (!ReferenceEquals(wholeUsbDevice, null))
                        {
                            wholeUsbDevice.ReleaseInterface(0);
                        }
                        myUsbDevice.Close();

                    }
                    myUsbDevice = null;
                    UsbDevice.Exit();
                }
                IsConnected = false;
            }
        }
        private void OnRxEndPointData(object sender, EndpointDataEventArgs e)
        {
            //Action<string> Action = ProcessReceiveData;
            //this.Dispatcher.Invoke(Action, (Encoding.Default.GetString(e.Buffer, 0, e.Count)));
        }
        private void Log(object sender, MouseButtonEventArgs e)
        {
            if (IsLaptop)
            {
                PCChatPage.Visibility = Visibility.Hidden;
                MCUChatPage.Visibility = Visibility.Visible;
            }
            else
            {
                PCChatPage.Visibility = Visibility.Visible;
                MCUChatPage.Visibility = Visibility.Hidden;
            }

            IsLaptop = !IsLaptop;
        }
        private void ScrollChatPC(object sender, MouseWheelEventArgs e)
        {
            scrollviewPC.ScrollToVerticalOffset(scrollviewPC.VerticalOffset - e.Delta);
            e.Handled = true;
        }
        private void ScrollChatMCU(object sender, MouseWheelEventArgs e)
        {
            scrollviewMCU.ScrollToVerticalOffset(scrollviewMCU.VerticalOffset - e.Delta);
            e.Handled = true;
        }
        private void PC_fileList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PC_fileList.SelectedItem is FileItem selectedFile)
            {                
                string x = selectedFile.FullName;
            }
        }
        private void CNC_fileList_SelectionChaged(object sender, SelectionChangedEventArgs e)
        {

        }

        #endregion
    }
}
