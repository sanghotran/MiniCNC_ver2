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

        // Data
        CNCMachine cnc = new CNCMachine();
                
        // Chat box
        private ObservableCollection<ChatItem> PCchatItems = new ObservableCollection<ChatItem>();
        private ObservableCollection<ChatItem> MCUchatItems = new ObservableCollection<ChatItem>();
        private ChatItem PCchatItem = new ChatItem("PC");
        private ChatItem mainMCUchatItem = new ChatItem("MCU Main");
        private ChatItem mainMCUchatItem1 = new ChatItem("MCU Main");
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

            cnc.State = 0; // mode disconect
            cnc.readyReceiveGcode = false;
            //cnc.home = false;

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
                if(fileInfo.Name.Contains(".tap") || fileInfo.Name.Contains(".txt"))
                {
                    fileItems.Add(new FileItem(fileInfo.Name, fileInfo.Length, fileInfo.FullName));
                }                
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
                autoCheckConnect.IsBackground = true;
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
        private void connect()
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
                SendData("C 0");// 0 is command connect
                showMessage(PCchatItem, scrollviewPC, PCchatItems, "Let's connect with me");
                cnc.State = 1;
            }
            catch
            {
                WarningShow.Visibility = Visibility.Visible;
            }
        }
        // Disconnect
        private void disconnect()
        {
            try
            {
                if (myUsbDevice == null) throw new Exception("Device Not Found.");
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
                cnc.State = 0;
            }
            catch
            {

            }            
        }        
        // send data
        private void SendData(string input)
        {
            int bytesWritten;
            if ((input.Length % 2) == 0)
            {
                input += ' ';
            }
            writer.Write(Encoding.Default.GetBytes(input), 1000, out bytesWritten);
        }
        // send gcode
        private void SendGcode()
        {
            if(cnc.index < (cnc.cncGcode.Length - 1))
            {
                SendData("D 1 " + cnc.cncGcode[cnc.index] + "! " ); // sending  gcode
                cnc.index++;
            }
            else
            {
                cnc.index = 0;
                SendData("D 0"); //finish send gcode
            }
            
        }
        // process data
        private void ProcessData(string input)
        {
            cnc.DataReceive = input.Split(' ');

            switch(cnc.DataReceive[0])
            {
                case "C":
                    switch(cnc.DataReceive[1])
                    {
                        case "CONNECTED":
                            showMessage(mainMCUchatItem, scrollviewPC, PCchatItems, "Connected");
                            break;
                        case "DISCONNECTED":                            
                            disconnect();
                            showMessage(mainMCUchatItem, scrollviewPC, PCchatItems, "Disconnected");
                            break;
                        case "DOING":
                            showMessage(mainMCUchatItem, scrollviewPC, PCchatItems, "I am going to home");
                            //showMessage(mainMCUchatItem1, scrollviewMCU, MCUchatItems, "Let's go to home");
                            break;
                        case "YES":
                            showMessage(mainMCUchatItem, scrollviewPC, PCchatItems, "I ready for receive a gcode");
                            cnc.readyReceiveGcode = true;
                            //SendGcode();
                            break;
                        case "RUNNING":
                            showMessage(mainMCUchatItem, scrollviewPC, PCchatItems, "Started");
                            SendGcode();
                            break;
                        case "ACK":
                            cnc.drawFromFeedback(MainShow, cnc.DataReceive[2]);
                            SendGcode();
                            break;
                        case "HOME":
                            showMessage(mainMCUchatItem, scrollviewPC, PCchatItems, "I have just come home");
                            break;
                        case "DONE":
                            showMessage(mainMCUchatItem, scrollviewPC, PCchatItems, "I'm done");
                            cnc.State = 1; // mode connect
                            cnc.readyReceiveGcode = false;
                            IsStarted = !IsStarted;
                            break;
                        case "NOHOME":
                            showMessage(mainMCUchatItem, scrollviewPC, PCchatItems, "I have come home, please press HOME");
                            break;
                        default:
                            break;
                    }
                    break;
                case "D":
                    break;
                default:
                    break;
            }
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
            // not press when running state
            if (cnc.State == 2)
                return;
            showPage(FolderShow);
            showFile();
        }
        private void Setting(object sender, MouseButtonEventArgs e)
        {
            // not press when running state
            if (cnc.State == 2)
                return;
            showPage(SettingShow);
        }
        private void Send(object sender, MouseButtonEventArgs e)
        {
            // only press when connect state
            if (cnc.State == 1)
            {
                SendData("C 5 " + cnc.fileName);
                showMessage(PCchatItem, scrollviewPC, PCchatItems, "I will send you a gcode");
            }
        }
        private void Home(object sender, MouseButtonEventArgs e)
        {
            // only press when connect state
            if(cnc.State == 1)
            {
                SendData("C 3");// 3 is command go to home
                showMessage(PCchatItem, scrollviewPC, PCchatItems, "Let's go to home");

            }
        }
        private void Start(object sender, MouseButtonEventArgs e)
        {
            // not press when disconnect state or not ready receive gcode
            if (cnc.State == 0 || !cnc.readyReceiveGcode)
                return;
            if(!IsStarted)
            {
                SendData("C 4");
                showMessage(PCchatItem, scrollviewPC, PCchatItems, "Let's start");
                cnc.State = 2; // mode running
            }
            else
            {
                cnc.State = 1; // mode connect
                cnc.readyReceiveGcode = false;
            }
            
            IsStarted = !IsStarted;
        }
        private void Pause(object sender, MouseButtonEventArgs e)
        {
            // not press when disconnect and connect state
            if (cnc.State == 0 || cnc.State == 1)
                return;

            IsPaused = !IsPaused;
        }
        private void Connect(object sender, MouseButtonEventArgs e)
        {
            if (!IsConnected) //kết nối
            {
                connect();
            }
            else // ngắt kết nối
            {                
                if (cnc.State == 0)
                    return;
                SendData("C 1");// 1 is command disconnect
                showMessage(PCchatItem, scrollviewPC, PCchatItems, "Let's disconnect with me");
            }
        }
        private void OnRxEndPointData(object sender, EndpointDataEventArgs e)
        {
            Action<string> Action = ProcessData;
            this.Dispatcher.Invoke(Action, (Encoding.Default.GetString(e.Buffer, 0, e.Count)));
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
                MainShow.Children.Clear();
                cnc.fileName = selectedFile.Name;
                string data = File.ReadAllText(selectedFile.FullName);
                if(selectedFile.Name.Contains(".tap"))
                {
                    string data_after_change;
                    data_after_change = cnc.readGcodeFromAspire(data);
                    cnc.gcode = data_after_change.Split('\n');
                }
                else
                {
                    cnc.gcode = data.Split('\n');
                }                
                cnc.drawFromGcode(cnc.gcode, MainShow);
                cnc.index = 0;
                showPage(MainShow);                
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
            disconnect();
            WarningShow.Visibility = Visibility.Hidden;
        }
        #endregion


    }
}
