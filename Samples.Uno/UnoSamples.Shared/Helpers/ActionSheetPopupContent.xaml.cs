using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
#if !HAS_UNO
using Popup = Microsoft.UI.Xaml.Controls.Primitives.Popup;
#else
using Popup = Microsoft.UI.Xaml.Controls.Popup;
#endif

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Samples.Helpers
{
    public partial class ActionSheetPopupContent : UserControl
    {
        public static readonly DependencyProperty TitleLabelProperty = DependencyProperty.Register(
            nameof(TitleLabel),
            typeof(string),
            typeof(ActionSheetPopupContent),
            new PropertyMetadata(default(string), new PropertyChangedCallback(OnTitleChanged)));

        static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ActionSheetPopupContent actionSheet && actionSheet._titleContentPresenter is ContentPresenter cp)
            {
                cp.Content = actionSheet.TitleLabel;
            }
        }

        public string TitleLabel
        {
            get => (string)GetValue(TitleLabelProperty);
            set => SetValue(TitleLabelProperty, value);
        }

        public static readonly DependencyProperty CancelLabelProperty = DependencyProperty.Register(
            nameof(CancelLabel),
            typeof(string),
            typeof(ActionSheetPopupContent),
            new PropertyMetadata(default(string)));

        public string CancelLabel
        {
            get => (string)GetValue(CancelLabelProperty);
            set => SetValue(CancelLabelProperty, value);
        }

        public static readonly DependencyProperty DestructionLabelProperty = DependencyProperty.Register(
            nameof(DestructionLabel),
            typeof(string),
            typeof(ActionSheetPopupContent),
            new PropertyMetadata(default(string), new PropertyChangedCallback(OnButtonsChanged)));

        public string DestructionLabel
        {
            get => (string)GetValue(DestructionLabelProperty);
            set => SetValue(DestructionLabelProperty, value);
        }

        public static readonly DependencyProperty ButtonsLabelsProperty = DependencyProperty.Register(
            nameof(ButtonsLabels),
            typeof(IList<string>),
            typeof(ActionSheetPopupContent),
            new PropertyMetadata(default(IList<string>), new PropertyChangedCallback(OnButtonsChanged)));

        static void OnButtonsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ActionSheetPopupContent actionSheet && actionSheet.topGrid is StackPanel topGrid)
            {
                var remove = topGrid.Children.Where(c => c is Button);
                foreach (var child in remove)
                    topGrid.Children.Remove(child);
                actionSheet.AddButton(actionSheet.DestructionLabel, true);
                foreach (var label in actionSheet.ButtonsLabels)
                    actionSheet.AddButton(label, false);
            }
        }

        public IList<string> ButtonsLabels
        {
            get => (IList<string>)GetValue(ButtonsLabelsProperty);
            set => SetValue(ButtonsLabelsProperty, value);
        }

        internal WeakReference<Popup> WeakParentPopup;

        public event EventHandler<string> ButtonClicked;

        readonly TaskCompletionSource<string> tcs;

        public ActionSheetPopupContent(TaskCompletionSource<string> tcs)
        {
            this.tcs = tcs;
            InitializeComponent();
            ButtonsLabels = new List<string>();
            SizeChanged += OnSizeChanged;
        }

        void AddButton(string label, bool isDestruct)
        {
            if (string.IsNullOrEmpty(label))
                return;
            var button = new Button
            {
                Content = label,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
            };
            if (isDestruct)
                button.Foreground = new SolidColorBrush(Colors.Red);
            button.Click += Button_Click;
            topGrid.Children.Add(button);
        }

        void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                if (button.Content is string label)
                {
                    ButtonClicked?.Invoke(this, label);
                    tcs.SetResult(label);
                }
            }
        }

        void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var window = this.GetWindow();
            var transform = window.Content.TransformToVisual(Parent as UIElement);
            var point = transform.TransformPoint(new Point(0, 0)); // gets the window's (0,0) coordinate relative to the popup

            var hOffset = (window.Bounds.Width - ActualWidth) / 2;
            var vOffset = window.Bounds.Height - ActualHeight;

            if (WeakParentPopup.TryGetTarget(out var parent))
            {
                parent.HorizontalOffset = point.X + hOffset;
                parent.VerticalOffset = point.Y + vOffset;
            }
        }
    }
}
