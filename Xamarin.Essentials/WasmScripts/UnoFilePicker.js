
function UnoFilePicker_Pick(optionsJson, multiple) {
    return new Promise(function (resolve, reject) {

        const options = JSON.parse(optionsJson);
        var picker = document.createElement('input');
        picker.setAttribute('type', 'file');

        if (multiple === true)
            picker.setAttribute('multiple', '');

        if ('FileTypes' in options && options.FileTypes !== null && options.FileTypes !== undefined && 'Value' in options.FileTypes) {
            console.log('options.FileTypes: ' + JSON.stringify(options.FileTypes.Value));
            picker.setAttribute('accept', options.FileTypes.Value);
        }

        picker.onabort = function () {
            console.log('abort');
            resolve(null);
        };

        picker.oncancel = function () {
            console.log('cancel');
            resolve(null);
        };

        picker.onchange = function () {
            let homeAnalysis = FS.analyzePath('/UnoFilePicker', false);
            if (!homeAnalysis.exists)
                FS.mkdir('/UnoFilePicker');
            var dir = '/UnoFilePicker/' + uuidv4();
            FS.mkdir(dir);

            let completed = 0;
            let result = new Object();
            result.Files = new Array();
            result.Errors = new Array();
            result.Aborts = new Array();
            for (i = 0; i < picker.files.length; i++) {
                let file = picker.files[i];
                let path =  dir + '/' + file.name;
                let fileObj = new Object();
                fileObj.FullPath = path;
                fileObj.ContentType = file.type;

                UnoFileSystem_GetDataFromJsFile(file).then(function (jsFileData) {
                    completed++;
                    if ('abort' in jsFileData)
                        result.Aborts.push(fileObj);
                    else if ('error' in jsFileData) {
                        fileObj.Error = reader.error;
                        result.Errors.push(fileObj);
                    }
                    else if ('text' in jsFileData) {
                        result.Files.push(fileObj);
                        //console.log('text: length[' + jsFileData.text.length + '] ');
                        FS.writeFile(path, jsFileData.text);
                        //console.log(' =================== ');
                        //console.log('path: ' + path);
                        //console.log(FS.readFile(path));
                        //console.log(' =================== ');
                    } else if ('view' in jsFileData) {
                        result.Files.push(fileObj);
                        //console.log('view: length[' + jsFileData.view.length + '] buffer.byteLength[' + jsFileData.view.byteLength + ']');
                        FS.writeFile(path, jsFileData.view);
                        //console.log(' =================== ');
                        //console.log('path: ' + path);
                        //console.log(toHex(FS.readFile(path)));
                        //console.log(' =================== ');
                    }

                    if (completed == picker.files.length)
                        resolve(JSON.stringify(result));
                });
            }
        }

        picker.onclose = function () {
            console.log('closed');
            resolve(null);
        }

        picker.click();
    });
}

function UnoFilePicker_Export(shareFile, fileName) {
    //console.log('UnoFilePicker_Export');
    //console.log(' shareFile: ' + shareFile);
    //console.log(' fileName: ' + fileName);
    let file = UnoFileSystem_ShareFileToJsFile(shareFile);
    //console.log(' file: ' + file);
    //saveAs(file, fileName);
    UnoFileSystem_GetDataFromJsFile(file).then(function (data) {
        if ('text' in data)
            console.log('text: ' + data.text);
        else if ('view' in data)
            console.log('view: ' + toHex(data.view));
        else if ('error' in data)
            console.log('error: ' + data.error);
        else if ('abort' in data)
            console.log('abort');
        saveAs(file, fileName);
    });
}


// Pre-Init
const LUT_HEX_4b = ['0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'];
const LUT_HEX_8b = new Array(0x100);
for (let n = 0; n < 0x100; n++) {
    LUT_HEX_8b[n] = `${LUT_HEX_4b[(n >>> 4) & 0xF]}${LUT_HEX_4b[n & 0xF]}`;
}
// End Pre-Init
function toHex(buffer) {
    let out = '';
    let length = buffer.length;
    if (length > 16 * 16)
        length = 16 * 16;
    for (let idx = 0, edx = buffer.length; idx < length; idx++) {
        if (idx % 4 == 0)
            out += ' ';
        if (idx % 16 == 0)
            out += '\n';
        out += LUT_HEX_8b[buffer[idx]];
    }
    return out;
}
