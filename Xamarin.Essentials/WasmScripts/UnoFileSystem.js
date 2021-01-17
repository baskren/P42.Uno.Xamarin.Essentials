function UnoFileSystem_GetFileName(path) {
    const pathArray = path.split("/");
    const lastIndex = pathArray.length - 1;
    return pathArray[lastIndex];
}

function UnoFileSystem_GetShareFileType(shareFile, isUtf8) {

    if (shareFile.ContentType && !isEmpty(shareFile.ContentType))
        return shareFile.ContentType;

    let mimeType = UnoMime_Lookup(shareFile.FullPath);
    if (mimeType)
        return mimeType;

    if (isUtf8)
        return "text/plain";
    return "application/octet-stream";
}

function UnoFileSystem_ShareFileToJsFile(shareFile) {
    if (typeof shareFile === "string" || shareFile instanceof String) {
        shareFile = JSON.parse(shareFile);
    }
    //console.log('UnoFileSystem_ShareFileToJsFile');
    //console.log('  shareFile: ' + JSON.stringify(shareFile));
    const buf = FS.readFile(shareFile.FullPath);
    //console.log('  buf: ' + toHex(buf));

    const result = UnoFileSystem_GetFsFileArrayBuf(buf);

    //console.log('  result: ' + toHex(result.content));
    //console.log('content: ' + result.content);
    const type = UnoFileSystem_GetShareFileType(shareFile, result.isUtf8);
    //console.log('type: ' + type);

    const file = new File([result.content], UnoFileSystem_GetFileName(shareFile.FullPath), { type: type });
    //console.log('file[' + file.name + ']: path[' + file.path + '] size[' + file.size + ']');
    return file;
}

function UnoFileSystem_ShareFilesToJsFiles(shareFiles) {
   
    const files = [];
    for (i = 0; i < shareFiles.length; i++) {
        files.push(UnoFileSystem_ShareFileToJsFile(shareFiles[i]));
    }
    return files;
}

// http://www.onicos.com/staff/iz/amuse/javascript/expert/utf.txt

/* utf.js - UTF-8 <=> UTF-16 convertion
 *
 * Copyright (C) 1999 Masanao Izumo <iz@onicos.co.jp>
 * Version: 1.0
 * LastModified: Dec 25 1999
 * This library is free.  You can redistribute it and/or modify it.
 */

function UnoFileSystem_GetFsFileArrayBuf(array) {
    console.log('GetFsFileArrayBuf');
    var out, i, len, c;
    var char2, char3;
    var bom = false;
    out = "";
    len = array.length;
    i = 0;
    while (i < len) {
        console.log('  i: ' + i);
        c = array[i++];
        console.log('  c: ' + c);
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
                //console.log('GetFsFileArrayBuf: untouched binary')
                return { isUtf8: false, content: array }
        }
    }
    //console.log('GetFsFileArrayBuf: converted to UTF8');
    return { isUtf8: true, content: out };
}

function UnoFileSystem_GetDataFromJsFile(file) {
    return new Promise(function (resolve, reject) {
        let reader = new FileReader();

        let result = new Object();
        isText = file.type.toString().startsWith('text');

        reader.onload = function (e) {
            let content = e.target.result;
            console.log('content: length[' + content.length + ']');

            if (isText) {
                result.text = content;
            }
            else {
                let view = new Uint8Array(content);
                //console.log(' =================== ');
                //console.log('view: length[' + view.length + '] buffer.byteLength[' + view.byteLength + ']')
                //console.log(toHex(view));
                //console.log(' =================== ');
                result.view = view;
            }
            resolve(result);
        };

        reader.onabort = function (e) {
            result.abort = true;
            resolve(result);
        }

        reader.onerror = function (e) {
            result.error = reader.error;
            resolve(result);
        }

        if (isText)
            reader.readAsText(file);
        else
            reader.readAsArrayBuffer(file);
    });
}


