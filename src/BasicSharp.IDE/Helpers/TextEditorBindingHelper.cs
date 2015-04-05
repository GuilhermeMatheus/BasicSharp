using ICSharpCode.AvalonEdit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BasicSharp.IDE.Helpers
{
    public class TextEditorBindingHelper
    {
        #region Dependency Properties
        public static readonly DependencyProperty TextProperty = DependencyProperty.RegisterAttached("Text",
            typeof(string), typeof(TextEditorBindingHelper), new FrameworkPropertyMetadata(string.Empty, OnTextPropertyChanged));

        public static readonly DependencyProperty AttachProperty = DependencyProperty.RegisterAttached("Attach",
            typeof(bool), typeof(TextEditorBindingHelper), new PropertyMetadata(false, Attach));

        private static readonly DependencyProperty IsUpdatingProperty = DependencyProperty.RegisterAttached("IsUpdating",
            typeof(bool), typeof(TextEditorBindingHelper));
        #endregion
        #region Getters and Setters
        public static void SetAttach(DependencyObject dp, bool value)
        {
            dp.SetValue(AttachProperty, value);
        }
        public static bool GetAttach(DependencyObject dp)
        {
            return (bool)dp.GetValue(AttachProperty);
        }

        public static string GetText(DependencyObject dp)
        {
            return (string)dp.GetValue(TextProperty);
        }
        public static void SetText(DependencyObject dp, string value)
        {
            dp.SetValue(TextProperty, value);
        }

        private static bool GetIsUpdating(DependencyObject dp)
        {
            return (bool)dp.GetValue(IsUpdatingProperty);
        }
        private static void SetIsUpdating(DependencyObject dp, bool value)
        {
            dp.SetValue(IsUpdatingProperty, value);
        }
        #endregion

        private static void OnTextPropertyChanged(DependencyObject sender,
            DependencyPropertyChangedEventArgs e)
        {
            TextEditor codeEditor = sender as TextEditor;
            codeEditor.TextChanged -= TextChanged;

            if (!GetIsUpdating(codeEditor))
                codeEditor.Text = (string)e.NewValue;

            codeEditor.TextChanged += TextChanged;
        }

        private static void Attach(DependencyObject sender,
            DependencyPropertyChangedEventArgs e)
        {
            TextEditor codeEditor = sender as TextEditor;

            if (codeEditor == null)
                return;

            if ((bool)e.OldValue)
                codeEditor.TextChanged -= TextChanged;

            if ((bool)e.NewValue)
                codeEditor.TextChanged += TextChanged;
        }

        static void TextChanged(object sender, EventArgs e)
        {
            TextEditor codeEditor = sender as TextEditor;
            SetIsUpdating(codeEditor, true);
            SetText(codeEditor, codeEditor.Text);
            SetIsUpdating(codeEditor, false);
        }
    }
}
