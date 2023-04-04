using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
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



        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            firtLoad();
        }

        #region Fields
        public const int CNC_PID = 22370;
        public const int CNC_VID = 1115;

        public static UsbDevice myUsbDevice;
        public static UsbDeviceFinder myUsbFinder = new UsbDeviceFinder(CNC_VID, CNC_PID);
        public static UsbEndpointReader reader;
        public static UsbEndpointWriter writer;

        private List<Grid> Pages = new List<Grid>();

        

        // Chat box
        private ObservableCollection<ChatItem> PCchatItems = new ObservableCollection<ChatItem>();
        private ObservableCollection<ChatItem> MCUchatItems = new ObservableCollection<ChatItem>();
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
        private bool _autoCheckConnet { get; set; }
        #endregion

        #region Functions
        private void firtLoad()
        {
            Pages.Add(FolderShow);
            Pages.Add(SettingShow);
            Pages.Add(MainShow);
            Pages.Add(WarningShow);

            IsStarted = false;
            IsPaused = false;
            IsConnected = false;
            IsLaptop = true;

            PCChatPage.Visibility = Visibility.Visible;
            MCUChatPage.Visibility = Visibility.Hidden;

            PCchatMCU.ItemsSource = PCchatItems;
            MCUchatMCU.ItemsSource = MCUchatItems;

            showPage(MainShow);
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

        // show page
        private void showPage(Grid grid)
        {
            foreach( Grid page in Pages)
            {
                if (page == grid)
                    page.Visibility = Visibility.Visible;
                else
                    page.Visibility = Visibility.Hidden;
            }    
        }
        // autocheck connect
        private void AutoCheckConnect(bool state)
        {
            _autoCheckConnet = state;
            if (_autoCheckConnet)
            {
                Thread autoCheckConnect = new Thread(checkConnect);
                autoCheckConnect.Start();
            }
        }
        private void checkConnect()
        {

            WqlEventQuery query = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 3");
            ManagementEventWatcher watcher = new ManagementEventWatcher(query);
            watcher.EventArrived += new EventArrivedEventHandler(DeviceRemoved);
            watcher.Start();
            while (_autoCheckConnet)
            {
                Thread.Sleep(1000);
            }
            watcher.Stop();
        }

        // Connect
        private void checkConnected()
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
                    AutoCheckConnect(true);
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
                AutoCheckConnect(false);
            }
        }

        // send data
        private void SendData(string input)
        {
            int bytesWritten;
            writer.Write(Encoding.Default.GetBytes(input), 1000, out bytesWritten);
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
            showPage(FolderShow);
            showFile();
        }
        private void Setting(object sender, MouseButtonEventArgs e)
        {
            showPage(SettingShow);
        }
        private void Send(object sender, MouseButtonEventArgs e)
        {
            SendData("0");
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
            checkConnected();
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
                //string data = File.ReadAllText(selectedFile.FullName);
                //string x = selectedFile.FullName;
            }
        }
        private void CNC_fileList_SelectionChaged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void DeviceRemoved(object sender, EventArrivedEventArgs e)
        {
            string query = "SELECT * FROM Win32_USBControllerDevice";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            foreach (ManagementObject usbDevice in searcher.Get())
            {
                // Kiểm tra xem thiết bị có phải là CNC Mini không
                if (usbDevice["Dependent"].ToString().Contains("5762"))
                {
                    return;
                }
            }
            WarningShow.Dispatcher.Invoke(() => WarningShow.Visibility = Visibility.Visible);
        }

        private void Ok_Warning(object sender, MouseButtonEventArgs e)
        {
            checkConnected();
            WarningShow.Visibility = Visibility.Hidden;
        }
        #endregion


    }
}
