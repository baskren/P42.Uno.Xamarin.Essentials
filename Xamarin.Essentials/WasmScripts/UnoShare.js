

String.prototype.isEmpty = function () {
    return (this.length === 0 || !this.trim());
};

function isEmpty(obj) {
    for (var key in obj) {
        if (obj.hasOwnProperty(key))
            return false;
    }
    return true;
}

function UnoShare_BuildRequest(json) {
    const obj = JSON.parse(json);
    var result = new Object();
    if ('Title' in obj && !IsNullEmptyOrWhiteSpace(obj.Title))
        result.title = obj.Title;
    if ('Uri' in obj && !IsNullEmptyOrWhiteSpace(obj.Uri))
        result.url = obj.Uri;
    if ('Text' in obj && !IsNullEmptyOrWhiteSpace(obj.Text))
        result.text = obj.Text;
    if ('File' in obj) 
        result.files = UnoFileSystem_ShareFilesToJsFiles([obj.File]);
    else if ('Files' in obj) 
        result.files = UnoFileSystem_ShareFilesToJsFiles(obj.Files);
    return result;
}

function UnoShare_ShareFromElement(id) {
    const getShareRequestJsonForHtmlElement = Module.mono_bind_static_method("[P42.Uno.Xamarin.Essentials.Wasm] Xamarin.Essentials.SharingExtensions:GetShareRequestJsonForHtmlElement");
    const json = getShareRequestJsonForHtmlElement(id);
    var share = UnoShare_BuildRequest(json);
    //getConfirmation(share);
    if ('files' in share && navigator.canShare && !navigator.canShare(share))
        alert('Sharing of files not yet supported on this browser.');
    else {
        navigator.share(share).catch((reason) => {
            alert('Sharing may not yet be supported on this browser or sharing may not be permitted with a file.  Error code: ' + reason);
        });
    }
}

function UnoShare_Share(json) {
    if (navigator.share) {
        var share = UnoShare_BuildRequest(json);
        navigator.share(share).catch((reason) => {
            alert('Sharing may not yet be supported on this browser or sharing may not be permitted with a file.  Error code: ' + reason);
        });
    }
    else
        alert('Sharing not yet supported on this browser.');

}

function UnoShare_CanShare(json) {
    //getConfirmation(share);
    if (navigator.share) {
        var share = UnoShare_BuildRequest(json);
        if (navigator.canShare)
            return navigator.canShare(share);
        else
            return true;
    }
    else
        return false;
}

function UnoShare_IsAvailable() {
    if (navigator.share) {
        return true;
    }
    return false;
}

