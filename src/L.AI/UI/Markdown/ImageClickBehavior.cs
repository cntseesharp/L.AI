
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace L_AI.UI.Markdown
{
    public static class ImageClickBehavior
    {
        public static readonly DependencyProperty ClickCommandProperty =
            DependencyProperty.RegisterAttached(
                "ClickCommand",
                typeof(ICommand),
                typeof(ImageClickBehavior),
                new PropertyMetadata(null, OnClickCommandChanged));

        public static ICommand GetClickCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(ClickCommandProperty);
        }

        public static void SetClickCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(ClickCommandProperty, value);
        }

        private static void OnClickCommandChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var image = obj as Image;
            if (image == null)
            {
                throw new InvalidOperationException("This behavior can only be attached to an Image control.");
            }

            if ((e.NewValue != null) && (e.OldValue == null))
            {
                image.MouseLeftButtonDown += Image_MouseLeftButtonDown;
            }
            else if ((e.NewValue == null) && (e.OldValue != null))
            {
                image.MouseLeftButtonDown -= Image_MouseLeftButtonDown;
            }
        }

        private static void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var image = sender as Image;
            var command = GetClickCommand(image);
            if (command != null && command.CanExecute(null))
            {
                command.Execute(null);
            }
        }
    }
}
