

String.prototype.isEmpty = function () {
    return (this.length === 0 || !this.trim());
};

function UnoShare_GetFileName(path) {
    const pathArray = path.split("/");
    const lastIndex = pathArray.length - 1;
    return pathArray[lastIndex];
}

function UnoShare_GetShareFileType(shareFile, isUtf8) {

    if (shareFile.Type && !isEmpty(shareFile.Type)) 
        return shareFile.Type;
    
    let mimeType = UnoMime_Lookup(shareFile.FullPath);
    if (mimeType) 
        return mimeType;
    
    if (isUtf8)
        return "text/plain";
    return "application/octet-stream";
}

function UnoShare_ShareFileToJsFile(shareFile) {
    const buf = FS.readFile(shareFile.FullPath);
    const result = GetFsFileArrayBuf(buf);
    console.log('content: ' + result.content);
    const type = UnoShare_GetShareFileType(shareFile.FullPath, result.isUtf8);
    console.log('type: '+ type);

    const file = new File(result.content, UnoShare_GetFileName(shareFile.FullPath), { type: type });
    console.log('file[' + file.name + ']: path[' + file.path + '] size[' + file.size + ']');

    const read = new FileReader();
    read.onloadend = function () {
        console.log('file[' + file.name + '] content[' + read.result + ']');
    }
    read.readAsText(file);

    return file;
}

function UnoShare_ShareFilesToJsFiles(shareFile) {

    const files = [];
    for (i = 0; i < shareFile.length; i++) {
        files.push(UnoShare_ShareFileToJsFile(shareFile[i]));
    }
    return files;
}

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
    if ('File' in obj) {
        result.files = UnoShare_ShareFilesToJsFiles([obj.File]);
        //var result = { title: obj.Title, files: files, text: obj.Text, url: obj.Uri };
    }
    else if ('Files' in obj) {
        result.files = UnoShare_ShareFilesToJsFiles(obj.Files);
        //var result = { title: obj.Title, files: files, text: obj.Text, url: obj.Uri };
    }
    //var result = { title: obj.Title, text: obj.Text, url: obj.Uri };
    return result;
}

function UnoShare_ShareFromElement(id) {
    const getShareRequestJsonForHtmlElement = Module.mono_bind_static_method("[P42.Uno.Xamarin.Essentials] Xamarin.Essentials.SharingExtensions:GetShareRequestJsonForHtmlElement");
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

// http://www.onicos.com/staff/iz/amuse/javascript/expert/utf.txt

/* utf.js - UTF-8 <=> UTF-16 convertion
 *
 * Copyright (C) 1999 Masanao Izumo <iz@onicos.co.jp>
 * Version: 1.0
 * LastModified: Dec 25 1999
 * This library is free.  You can redistribute it and/or modify it.
 */

function GetFsFileArrayBuf(array) {
    var out, i, len, c;
    var char2, char3;
    var bom = false;
    out = "";
    len = array.length;
    i = 0;
    while (i < len) {
        c = array[i++];
        switch (c >> 4) {
            case 0: case 1: case 2: case 3: case 4: case 5: case 6: case 7:
                // 0xxxxxxx
                out += String.fromCharCode(c);
                break;
            case 12: case 13:
                // 110x xxxx   10xx xxxx
                char2 = array[i++];
                out += String.fromCharCode(((c & 0x1F) << 6) | (char2 & 0x3F));
                break;
            case 14:
                // 1110 xxxx  10xx xxxx  10xx xxxx
                char2 = array[i++];
                char3 = array[i++];
                out += String.fromCharCode(((c & 0x0F) << 12) |
                    ((char2 & 0x3F) << 6) |
                    ((char3 & 0x3F) << 0));
                break;
            default:
                if (i == 0 && c == 0xff) {
                    bom = true;
                    break;
                }
                if (bom && i == 1 && c == 0xBB)
                    break;
                if (bom && i == 2 && c == 0xBF)
                    break;
                return { isUtf8: false, result: array }
        }
    }
    return { isUtf8: true, content: [out] };
}
