﻿#pragma checksum "..\..\..\CustomControls\MFMOption.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "A247AA1B3FDFB331C1CE4010F95D3BD2"
//------------------------------------------------------------------------------
// <auto-generated>
//     이 코드는 도구를 사용하여 생성되었습니다.
//     런타임 버전:4.0.30319.42000
//
//     파일 내용을 변경하면 잘못된 동작이 발생할 수 있으며, 코드를 다시 생성하면
//     이러한 변경 내용이 손실됩니다.
// </auto-generated>
//------------------------------------------------------------------------------

using MusicFileManager;
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


namespace MusicFileManager {
    
    
    /// <summary>
    /// MFMOption
    /// </summary>
    public partial class MFMOption : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 6 "..\..\..\CustomControls\MFMOption.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal MusicFileManager.MFMOption userControl;
        
        #line default
        #line hidden
        
        
        #line 34 "..\..\..\CustomControls\MFMOption.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox cbMultiAudioInArchive;
        
        #line default
        #line hidden
        
        
        #line 41 "..\..\..\CustomControls\MFMOption.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Slider sldBitRate;
        
        #line default
        #line hidden
        
        
        #line 43 "..\..\..\CustomControls\MFMOption.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Slider sldDuration;
        
        #line default
        #line hidden
        
        
        #line 44 "..\..\..\CustomControls\MFMOption.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblBitRate;
        
        #line default
        #line hidden
        
        
        #line 45 "..\..\..\CustomControls\MFMOption.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblDuration;
        
        #line default
        #line hidden
        
        
        #line 46 "..\..\..\CustomControls\MFMOption.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox cbDeleteWithOutFiltering;
        
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
            System.Uri resourceLocater = new System.Uri("/MusicFileManager;component/customcontrols/mfmoption.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\CustomControls\MFMOption.xaml"
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
            this.userControl = ((MusicFileManager.MFMOption)(target));
            return;
            case 2:
            this.cbMultiAudioInArchive = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 3:
            this.sldBitRate = ((System.Windows.Controls.Slider)(target));
            return;
            case 4:
            this.sldDuration = ((System.Windows.Controls.Slider)(target));
            return;
            case 5:
            this.lblBitRate = ((System.Windows.Controls.Label)(target));
            return;
            case 6:
            this.lblDuration = ((System.Windows.Controls.Label)(target));
            return;
            case 7:
            this.cbDeleteWithOutFiltering = ((System.Windows.Controls.CheckBox)(target));
            
            #line 46 "..\..\..\CustomControls\MFMOption.xaml"
            this.cbDeleteWithOutFiltering.Checked += new System.Windows.RoutedEventHandler(this.cbDeleteWithOutFiltering_Checked);
            
            #line default
            #line hidden
            
            #line 46 "..\..\..\CustomControls\MFMOption.xaml"
            this.cbDeleteWithOutFiltering.Unchecked += new System.Windows.RoutedEventHandler(this.cbDeleteWithOutFiltering_Unchecked);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

