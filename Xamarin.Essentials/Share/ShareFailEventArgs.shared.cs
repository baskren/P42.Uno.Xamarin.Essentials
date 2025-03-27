using System;
using System.Collections.Generic;
using System.Text;

namespace Xamarin.Essentials;

public class ShareFailEventArgs : EventArgs
{
    public ShareRequestBase ShareRequest { get; private set; }

    internal ShareFailEventArgs(ShareRequestBase shareRequest)
    {
        ShareRequest = shareRequest;
    }
}