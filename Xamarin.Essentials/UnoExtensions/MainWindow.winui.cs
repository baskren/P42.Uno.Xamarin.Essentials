using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Contacts;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace Xamarin.Essentials
{

    public static class MainWindowExtensions
    {
        static readonly Guid dtm_iid =
            new Guid(0xa5caee9b, 0x8708, 0x49d1, 0x8d, 0x36, 0x67, 0xd2, 0x5a, 0x8d, 0xa0, 0x0c);


        public static IntPtr GetHandle(this Window window) 
            => WinRT.Interop.WindowNative.GetWindowHandle(window);

        public static DataTransferManager GetDataTransferManager(this Window window)
        {
            var interop = Windows.ApplicationModel.DataTransfer.DataTransferManager.As<IDataTransferManagerInterop>();
            var result = interop.GetForWindow(window.GetHandle(), dtm_iid);
            var dataTransferManager = WinRT.MarshalInterface<Windows.ApplicationModel.DataTransfer.DataTransferManager>.FromAbi(result);
            return dataTransferManager;
        }


        public static void LoadIcon(this Window window, string iconName)
        {
            //Get the Window's HWND
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            var hIcon = PInvoke.User32.LoadImage(System.IntPtr.Zero, iconName,
                      PInvoke.User32.ImageType.IMAGE_ICON, 16, 16, PInvoke.User32.LoadImageFlags.LR_LOADFROMFILE);

            PInvoke.User32.SendMessage(hwnd, PInvoke.User32.WindowMessage.WM_SETICON, (System.IntPtr)0, hIcon);
        }

    }
    /*
    public partial class MainWindow : Window
    {

        public static new MainWindow Current
        {
            get
            {
                if (Platform.MainWindow is MainWindow window)
                    return window;
                MainThread.BeginInvokeOnMainThread(() => throw new InvalidOperationException("Cannot show Share UI unless Window in App.xaml.cs is of type Xamarin.Essentials.MainPage"));
                throw new InvalidOperationException("Cannot show Share UI unless Window in App.xaml.cs is of type Xamarin.Essentials.MainPage");
            }
        }

        static readonly Guid _dtm_iid = 
            new Guid(0xa5caee9b, 0x8708, 0x49d1, 0x8d, 0x36, 0x67, 0xd2, 0x5a, 0x8d, 0xa0, 0x0c);


        public IntPtr Handle => WinRT.Interop.WindowNative.GetWindowHandle(this);

        public MainWindow()
        {
            Title = Package.Current.DisplayName;
        }

        public DataTransferManager GetDataTransferManager()
        {
            var interop = Windows.ApplicationModel.DataTransfer.DataTransferManager.As<IDataTransferManagerInterop>();
            var result = interop.GetForWindow(Handle, _dtm_iid);
            var dataTransferManager = WinRT.MarshalInterface <Windows.ApplicationModel.DataTransfer.DataTransferManager>.FromAbi(result);
            return dataTransferManager;
        }

    }
    */

    static class WinUiWindowInteropExtensions
    {

        public static void ShowShareUI(this DataTransferManager manager)
        {
            var interop = Windows.ApplicationModel.DataTransfer.DataTransferManager.As<IDataTransferManagerInterop>();
            // Show the Share UI
            // interop.ShowShareUIForWindow(MainWindow.Current.Handle);  // UWP approach
            interop.ShowShareUIForWindow(WinRT.Interop.WindowNative.GetWindowHandle(Platform.MainWindow));
        }

        public static void InitializeWithWindow(this FileOpenPicker picker)
            => WinRT.Interop.InitializeWithWindow.Initialize(picker, WinRT.Interop.WindowNative.GetWindowHandle(Platform.MainWindow));

        public static void InitializeWithWindow(this FileSavePicker picker)
            => WinRT.Interop.InitializeWithWindow.Initialize(picker, WinRT.Interop.WindowNative.GetWindowHandle(Platform.MainWindow));

        public static void InitializeWithWindow(this FolderPicker picker)
            => WinRT.Interop.InitializeWithWindow.Initialize(picker, WinRT.Interop.WindowNative.GetWindowHandle(Platform.MainWindow));

        public static void InitializeWithWindow(this ContactPicker picker)
            => WinRT.Interop.InitializeWithWindow.Initialize(picker, WinRT.Interop.WindowNative.GetWindowHandle(Platform.MainWindow));
    }

    [System.Runtime.InteropServices.ComImport]
    [System.Runtime.InteropServices.Guid("3A3DCD6C-3EAB-43DC-BCDE-45671CE800C8")]
    [System.Runtime.InteropServices.InterfaceType(
    System.Runtime.InteropServices.ComInterfaceType.InterfaceIsIUnknown)]
    interface IDataTransferManagerInterop
    {
        IntPtr GetForWindow([System.Runtime.InteropServices.In] IntPtr appWindow,
            [System.Runtime.InteropServices.In] ref Guid riid);
        void ShowShareUIForWindow(IntPtr appWindow);
    }

}
