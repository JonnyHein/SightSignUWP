﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SightSign {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "14.0.0.0")]
    internal sealed partial class Settings1 : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings1 defaultInstance = ((Settings1)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings1())));
        
        public static Settings1 Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string LoadedInkLocation {
            get {
                return ((string)(this["LoadedInkLocation"]));
            }
            set {
                this["LoadedInkLocation"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool ShowDotWhenWriting {
            get {
                return ((bool)(this["ShowDotWhenWriting"]));
            }
            set {
                this["ShowDotWhenWriting"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool ShowTranslucentInkWhenWriting {
            get {
                return ((bool)(this["ShowTranslucentInkWhenWriting"]));
            }
            set {
                this["ShowTranslucentInkWhenWriting"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("White")]
        public global::System.Drawing.Color InkColor {
            get {
                return ((global::System.Drawing.Color)(this["InkColor"]));
            }
            set {
                this["InkColor"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("64, 255, 255, 255")]
        public global::System.Drawing.Color FadedInkColor {
            get {
                return ((global::System.Drawing.Color)(this["FadedInkColor"]));
            }
            set {
                this["FadedInkColor"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("10")]
        public double InkWidth {
            get {
                return ((double)(this["InkWidth"]));
            }
            set {
                this["InkWidth"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("128, 254, 212, 42")]
        public global::System.Drawing.Color DotColor {
            get {
                return ((global::System.Drawing.Color)(this["DotColor"]));
            }
            set {
                this["DotColor"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("100")]
        public double DotWidth {
            get {
                return ((double)(this["DotWidth"]));
            }
            set {
                this["DotWidth"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("20")]
        public int AnimationInterval {
            get {
                return ((int)(this["AnimationInterval"]));
            }
            set {
                this["AnimationInterval"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("200")]
        public int AnimationPointsOnFirstStroke {
            get {
                return ((int)(this["AnimationPointsOnFirstStroke"]));
            }
            set {
                this["AnimationPointsOnFirstStroke"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Swift")]
        public string RobotType {
            get {
                return (string)this["RobotType"];
            }
            set {
                this["RobotType"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("com7")]
        public string RobotComPort {
            get {
                return ((string)(this["RobotComPort"]));
            }
            set {
                this["RobotComPort"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1.66")]
        public double RobotZShift {
            get {
                return ((double)(this["RobotZShift"]));
            }
            set {
                this["RobotZShift"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0.35")]
        public double RobotWorkspaceScale {
            get {
                return ((double)(this["RobotWorkspaceScale"]));
            }
            set {
                this["RobotWorkspaceScale"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool RobotControl {
            get {
                return ((bool)(this["RobotControl"]));
            }
            set {
                this["RobotControl"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("100")]
        public double DotDownWidth {
            get {
                return ((double)(this["DotDownWidth"]));
            }
            set {
                this["DotDownWidth"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("254, 212, 42")]
        public global::System.Drawing.Color DotDownColor {
            get {
                return ((global::System.Drawing.Color)(this["DotDownColor"]));
            }
            set {
                this["DotDownColor"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public double RobotRadiusBacklash {
            get {
                return ((double)(this["RobotRadiusBacklash"]));
            }
            set {
                this["RobotRadiusBacklash"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public double RobotThetaBacklash {
            get {
                return ((double)(this["RobotThetaBacklash"]));
            }
            set {
                this["RobotThetaBacklash"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Black")]
        public global::System.Drawing.Color BackgroundColor {
            get {
                return ((global::System.Drawing.Color)(this["BackgroundColor"]));
            }
            set {
                this["BackgroundColor"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("254, 212, 42")]
        public global::System.Drawing.Color ButtonBackgroundColor {
            get {
                return ((global::System.Drawing.Color)(this["ButtonBackgroundColor"]));
            }
            set {
                this["ButtonBackgroundColor"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Black")]
        public global::System.Drawing.Color ButtonTextColor {
            get {
                return ((global::System.Drawing.Color)(this["ButtonTextColor"]));
            }
            set {
                this["ButtonTextColor"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("White")]
        public global::System.Drawing.Color ButtonBorderColor {
            get {
                return ((global::System.Drawing.Color)(this["ButtonBorderColor"]));
            }
            set {
                this["ButtonBorderColor"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("2")]
        public double ButtonBorderWidth {
            get {
                return ((double)(this["ButtonBorderWidth"]));
            }
            set {
                this["ButtonBorderWidth"] = value;
            }
        }
    }
}
