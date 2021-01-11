function UnoShare_ShareFiles(title, nodeFilePaths) {
    const extractFilename = (path) => {
        const pathArray = path.split("/");
        const lastIndex = pathArray.length - 1;
        return pathArray[lastIndex];
    };

    const files = [];
    for (i = 0; i < nodeFilePaths.length; i++) {
        var file = new File(FS.readFile(nodeFilePaths[i]), extractFilename(nodeFilePaths[i]));
        files.push(file);
    }

    var share = { files: files, title: title };

    return navigator.share(share);
}

function UnoShare_CanShareFiles(nodeFilePaths) {
    const extractFilename = (path) => {
        const pathArray = path.split("/");
        const lastIndex = pathArray.length - 1;
        return pathArray[lastIndex];
    };

    const files = [];
    for (i = 0; i < nodeFilePaths.length; i++) {
        var file = new File(FS.readFile(nodeFilePaths[i]), extractFilename(nodeFilePaths[i]));
        files.push(file);
    }

    var share = { files: files };

    var result = navigator.canShare(share);
    console.log('can share: ' + result);
    return result;
}

String.prototype.isEmpty = function () {
    return (this.length === 0 || !this.trim());
};

function UnoShare_GetFileName(path) {
    const pathArray = path.split("/");
    const lastIndex = pathArray.length - 1;
    return pathArray[lastIndex];
}

//var UnoShare_MimeType = require('mime-types')

function UnoShare_GetShareFileType(shareFile, isUtf8) {

    if (shareFile.Type && !shareFile.Type.isEmpty()) 
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

function UnoShare_BuildRequest(json) {
    const obj = JSON.parse(json);
    if ('File' in obj) {
        let files = UnoShare_ShareFilesToJsFiles([obj.File]);
        return { title: obj.Title, files: files, text: obj.Text, url: obj.Uri };
    }
    if ('Files' in obj) {
        files = UnoShare_ShareFilesToJsFiles(obj.Files);
        return { title: obj.Title, files: files, text: obj.Text, url: obj.Uri };
    }
    return { title: obj.Title, text: obj.Text, url: obj.Uri };
}

function UnoShare_ShareFromElement(id) {
    const getShareRequestJsonForHtmlElement = Module.mono_bind_static_method("[P42.Uno.Xamarin.Essentials] Xamarin.Essentials.SharingExtensions:GetShareRequestJsonForHtmlElement");
    //const onPlatformShareMultipleFilesFailed = Module.mono_bind_static_method("[P42.Uno.Xamarin.Essentials] Xamarin.Essentials.Share:OnPlatformShareMultipleFilesFailed");

    console.log('UnoShare_ShareFromElement(' + id + ') ENTER');
    const json = getShareRequestJsonForHtmlElement(id);
    console.log("json= " + json);
    var share = UnoShare_BuildRequest(json);
    //getConfirmation(share);
    if ('files' in share && navigator.canShare && !navigator.canShare(share))
        alert('Sharing of files may not yet be supported on this browser.');
    else {
        navigator.share(share).catch((reason) => {
            console.log('D failure: ' + reason);
            alert('Sharing may not yet be supported on this browser.  Error code: ' + reason);
        });
    }
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
