﻿#pragma checksum "..\..\..\webSite.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "A2B167243ECBEDBC0E71FBF6789007D7B5669CF3"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
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
using wnmp;


namespace wnmp {
    
    
    /// <summary>
    /// WebSite
    /// </summary>
    public partial class WebSite : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 13 "..\..\..\webSite.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox domainNameInput;
        
        #line default
        #line hidden
        
        
        #line 15 "..\..\..\webSite.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox siteRootInput;
        
        #line default
        #line hidden
        
        
        #line 18 "..\..\..\webSite.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox sitePortInput;
        
        #line default
        #line hidden
        
        
        #line 20 "..\..\..\webSite.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox siteFileInput;
        
        #line default
        #line hidden
        
        
        #line 22 "..\..\..\webSite.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label removeWebSite;
        
        #line default
        #line hidden
        
        
        #line 29 "..\..\..\webSite.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox staticBox;
        
        #line default
        #line hidden
        
        
        #line 31 "..\..\..\webSite.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox staticInput;
        
        #line default
        #line hidden
        
        
        #line 37 "..\..\..\webSite.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox gzipBox;
        
        #line default
        #line hidden
        
        
        #line 40 "..\..\..\webSite.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox crossBox;
        
        #line default
        #line hidden
        
        
        #line 42 "..\..\..\webSite.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox hostBox;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.8.1.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/wnmp;component/website.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\webSite.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.8.1.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 12 "..\..\..\webSite.xaml"
            ((System.Windows.Controls.Label)(target)).MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.Label_MouseDown);
            
            #line default
            #line hidden
            return;
            case 2:
            this.domainNameInput = ((System.Windows.Controls.TextBox)(target));
            return;
            case 3:
            this.siteRootInput = ((System.Windows.Controls.TextBox)(target));
            return;
            case 4:
            
            #line 17 "..\..\..\webSite.xaml"
            ((System.Windows.Controls.Label)(target)).MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.Label_MouseDown_1);
            
            #line default
            #line hidden
            return;
            case 5:
            this.sitePortInput = ((System.Windows.Controls.TextBox)(target));
            
            #line 18 "..\..\..\webSite.xaml"
            this.sitePortInput.LostFocus += new System.Windows.RoutedEventHandler(this.sitePortInput_LostFocus);
            
            #line default
            #line hidden
            return;
            case 6:
            this.siteFileInput = ((System.Windows.Controls.TextBox)(target));
            
            #line 20 "..\..\..\webSite.xaml"
            this.siteFileInput.LostFocus += new System.Windows.RoutedEventHandler(this.siteFileInput_LostFocus);
            
            #line default
            #line hidden
            return;
            case 7:
            this.removeWebSite = ((System.Windows.Controls.Label)(target));
            
            #line 22 "..\..\..\webSite.xaml"
            this.removeWebSite.MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.removeWebSite_MouseDown);
            
            #line default
            #line hidden
            return;
            case 8:
            this.staticBox = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 9:
            this.staticInput = ((System.Windows.Controls.TextBox)(target));
            
            #line 31 "..\..\..\webSite.xaml"
            this.staticInput.LostFocus += new System.Windows.RoutedEventHandler(this.staticInput_LostFocus);
            
            #line default
            #line hidden
            return;
            case 10:
            this.gzipBox = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 11:
            this.crossBox = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 12:
            this.hostBox = ((System.Windows.Controls.CheckBox)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}
