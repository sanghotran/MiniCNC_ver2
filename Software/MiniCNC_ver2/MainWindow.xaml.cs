using System;
using System.Collections.Generic;
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

namespace MiniCNC_ver2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
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
        #endregion

        #region Functions
        public void firtLoad()
        {
            IsStarted = false;
            IsPaused = false;
            IsConnected = false;
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
            IsConnected = !IsConnected;
        }
        #endregion


    }
}
