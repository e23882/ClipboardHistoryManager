using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using System.Windows.Controls;
using ClipboardHistoryManager.Models;
using ClipboardHistoryManager.Services;
using ClipboardHistoryManager.ViewModels;

namespace ClipboardHistoryManager
{
    /// <summary>
    /// 
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Fields
        private MainViewModel _viewModel;
        private bool _isSettingsPanelVisible = false;
        #endregion

        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            _viewModel = (MainViewModel)DataContext;
            _viewModel.OnCopySuccess += ViewModel_OnCopySuccess;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// 
        /// </summary>
        private void ViewModel_OnCopySuccess()
        {
            var sb = (Storyboard)this.FindResource("CopySuccessAnimation");
            sb.Begin(this);
        }

        #region Clipboard Monitoring

        private HwndSource? _source;
        private const int WM_CLIPBOARDUPDATE = 0x031D;

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AddClipboardFormatListener(IntPtr hwnd);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool RemoveClipboardFormatListener(IntPtr hwnd);

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var helper = new WindowInteropHelper(this);
            IntPtr handle = helper.Handle;
            _source = HwndSource.FromHwnd(handle);
            _source?.AddHook(HwndHook);
            AddClipboardFormatListener(handle);
            var sb = (Storyboard)this.FindResource("CopySuccessAnimation");
            sb.Begin(this);
        }

        protected override void OnClosed(EventArgs e)
        {
            var settings = new Settings
            {
                WindowTop = this.Top,
                WindowLeft = this.Left,
                History = _viewModel.ClipboardHistory.Select(item => item.Text).ToList(),
                FontSize = FontSizeSlider.Value
            };
            SettingsService.Save(settings);

            base.OnClosed(e);
            var helper = new WindowInteropHelper(this);
            RemoveClipboardFormatListener(helper.Handle);
            _source?.RemoveHook(HwndHook);
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_CLIPBOARDUPDATE)
            {
                try
                {
                    if (Clipboard.ContainsText())
                    {
                        string text = Clipboard.GetText();
                        _viewModel.AddHistoryItem(text);
                    }
                }
                catch
                {
                    // Clipboard can be busy, ignore errors
                }
            }
            return IntPtr.Zero;
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBoxItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is System.Windows.Controls.ListBoxItem item && item.DataContext != null)
            {
                _viewModel.CopyItemCommand.Execute(item.DataContext);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                DragMove();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //_glitchStoryboard = (Storyboard)this.FindResource("GlitchAnimation");
            var settings = SettingsService.Load();
            this.Top = settings.WindowTop;
            this.Left = settings.WindowLeft;
            _viewModel.LoadHistory(settings.History);
            FontSizeSlider.Value = settings.FontSize;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            SpotlightLight.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            SpotlightLight.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainGrid_MouseMove(object sender, MouseEventArgs e)
        {
            Point pos = e.GetPosition(MainGrid);
            Canvas.SetLeft(SpotlightLight, pos.X - (SpotlightLight.ActualWidth / 2));
            Canvas.SetTop(SpotlightLight, pos.Y - (SpotlightLight.ActualHeight / 2));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1)
            {
                _isSettingsPanelVisible = !_isSettingsPanelVisible;

                Storyboard sb;
                if (_isSettingsPanelVisible)
                {
                    sb = (Storyboard)this.FindResource("SlideInAnimation");
                }
                else
                {
                    sb = (Storyboard)this.FindResource("SlideOutAnimation");
                }
                sb.Begin(SettingsPanel);
            }
        }
        #endregion
    }
}
