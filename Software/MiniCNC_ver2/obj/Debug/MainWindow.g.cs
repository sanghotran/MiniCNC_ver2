﻿#pragma checksum "..\..\MainWindow.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "7D2EC228D07671A9CEBFAF14CDE39B065DC02015BD78B2A9B821F1CA2380DAB4"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using MiniCNC_ver2;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace MiniCNC_ver2 {
    
    
    /// <summary>
    /// MainWindow
    /// </summary>
    public partial class MainWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 243 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid MainShow;
        
        #line default
        #line hidden
        
        
        #line 247 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid FolderShow;
        
        #line default
        #line hidden
        
        
        #line 253 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListView PC_fileList;
        
        #line default
        #line hidden
        
        
        #line 269 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListView CNC_fileList;
        
        #line default
        #line hidden
        
        
        #line 273 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid SettingShow;
        
        #line default
        #line hidden
        
        
        #line 278 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox thinkness;
        
        #line default
        #line hidden
        
        
        #line 293 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid PCChatPage;
        
        #line default
        #line hidden
        
        
        #line 295 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ScrollViewer scrollviewPC;
        
        #line default
        #line hidden
        
        
        #line 296 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox PCchatMCU;
        
        #line default
        #line hidden
        
        
        #line 311 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid MCUChatPage;
        
        #line default
        #line hidden
        
        
        #line 313 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ScrollViewer scrollviewMCU;
        
        #line default
        #line hidden
        
        
        #line 314 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox MCUchatMCU;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/MiniCNC_ver2;component/mainwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\MainWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 20 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.DockPanel)(target)).PreviewMouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.DragWindow);
            
            #line default
            #line hidden
            return;
            case 2:
            
            #line 26 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.Image)(target)).PreviewMouseDown += new System.Windows.Input.MouseButtonEventHandler(this.CloseApp);
            
            #line default
            #line hidden
            return;
            case 3:
            
            #line 43 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.Image)(target)).PreviewMouseDown += new System.Windows.Input.MouseButtonEventHandler(this.MinimizeApp);
            
            #line default
            #line hidden
            return;
            case 4:
            
            #line 92 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.Image)(target)).PreviewMouseDown += new System.Windows.Input.MouseButtonEventHandler(this.Log);
            
            #line default
            #line hidden
            return;
            case 5:
            
            #line 113 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.Image)(target)).PreviewMouseDown += new System.Windows.Input.MouseButtonEventHandler(this.OpenFile);
            
            #line default
            #line hidden
            return;
            case 6:
            
            #line 130 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.Image)(target)).PreviewMouseDown += new System.Windows.Input.MouseButtonEventHandler(this.Setting);
            
            #line default
            #line hidden
            return;
            case 7:
            
            #line 147 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.Image)(target)).PreviewMouseDown += new System.Windows.Input.MouseButtonEventHandler(this.Send);
            
            #line default
            #line hidden
            return;
            case 8:
            
            #line 164 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.Image)(target)).PreviewMouseDown += new System.Windows.Input.MouseButtonEventHandler(this.Home);
            
            #line default
            #line hidden
            return;
            case 9:
            
            #line 180 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.Image)(target)).PreviewMouseDown += new System.Windows.Input.MouseButtonEventHandler(this.Start);
            
            #line default
            #line hidden
            return;
            case 10:
            
            #line 200 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.Image)(target)).PreviewMouseDown += new System.Windows.Input.MouseButtonEventHandler(this.Pause);
            
            #line default
            #line hidden
            return;
            case 11:
            
            #line 220 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.Image)(target)).PreviewMouseDown += new System.Windows.Input.MouseButtonEventHandler(this.Connect);
            
            #line default
            #line hidden
            return;
            case 12:
            this.MainShow = ((System.Windows.Controls.Grid)(target));
            return;
            case 13:
            this.FolderShow = ((System.Windows.Controls.Grid)(target));
            return;
            case 14:
            this.PC_fileList = ((System.Windows.Controls.ListView)(target));
            
            #line 254 "..\..\MainWindow.xaml"
            this.PC_fileList.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.PC_fileList_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 15:
            this.CNC_fileList = ((System.Windows.Controls.ListView)(target));
            
            #line 270 "..\..\MainWindow.xaml"
            this.CNC_fileList.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.CNC_fileList_SelectionChaged);
            
            #line default
            #line hidden
            return;
            case 16:
            this.SettingShow = ((System.Windows.Controls.Grid)(target));
            return;
            case 17:
            this.thinkness = ((System.Windows.Controls.TextBox)(target));
            return;
            case 18:
            this.PCChatPage = ((System.Windows.Controls.Grid)(target));
            return;
            case 19:
            this.scrollviewPC = ((System.Windows.Controls.ScrollViewer)(target));
            
            #line 295 "..\..\MainWindow.xaml"
            this.scrollviewPC.PreviewMouseWheel += new System.Windows.Input.MouseWheelEventHandler(this.ScrollChatPC);
            
            #line default
            #line hidden
            return;
            case 20:
            this.PCchatMCU = ((System.Windows.Controls.ListBox)(target));
            return;
            case 21:
            this.MCUChatPage = ((System.Windows.Controls.Grid)(target));
            return;
            case 22:
            this.scrollviewMCU = ((System.Windows.Controls.ScrollViewer)(target));
            
            #line 313 "..\..\MainWindow.xaml"
            this.scrollviewMCU.PreviewMouseWheel += new System.Windows.Input.MouseWheelEventHandler(this.ScrollChatMCU);
            
            #line default
            #line hidden
            return;
            case 23:
            this.MCUchatMCU = ((System.Windows.Controls.ListBox)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

