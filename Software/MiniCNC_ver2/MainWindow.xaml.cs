using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        private ObservableCollection<ChatItem> chatItems = new ObservableCollection<ChatItem>();

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

        private ChatItem chatItem = new ChatItem();
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
        public void firtLoad()
        {
            IsStarted = false;
            IsPaused = false;
            IsConnected = false;
            IsLaptop = true;
            PCchatMCU.ItemsSource = chatItems;
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
            chatItem.UpdateChat("PC", "Start");
            chatItems.Add(chatItem);
            scrollview.ScrollToEnd();
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
            IsLaptop = !IsLaptop;
        }
        private void ScrollChat(object sender, MouseWheelEventArgs e)
        {
            scrollview.ScrollToVerticalOffset(scrollview.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        #endregion


    }
}
