
function UnoFilePicker_Pick(optionsJson, multiple) {
    return new Promise(function (resolve, reject) {

        console.log('optionsJson: ' + optionsJson);
        const options = JSON.parse(optionsJson);
        console.log('options: ' + options);

        var picker = document.createElement('input');
        picker.setAttribute('type', 'file');

        console.log('1');
        if (multiple === true)
            picker.setAttribute('multiple', '');
        console.log('2');

        console.log('FileTypes: ' + options.FileTypes);

        if ('FileTypes' in options && options.FileTypes !== null && options.FileTypes !== undefined && 'Value' in options.FileTypes) {
            console.log('options.FileTypes: ' + JSON.stringify(options.FileTypes.Value));
            picker.setAttribute('accept', options.FileTypes.Value);
        }
        console.log('3');
        //picker.setAttribute('placeholder', placeholder);
        //picker.setAttribute('value', value);

        picker.onabort = function () {
            console.log('abort');
            resolve(null);
        };
        console.log('4');

        picker.oncancel = function () {
            console.log('cancel');
            resolve(null);
        };

        console.log('5');

        picker.onchange = function () {

            //Blob.prototype.arrayBuffer ??= function () { return new Response(this).arrayBuffer() }

            
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

                let reader = new FileReader();
                reader.onload = function (e) {
                    completed++;
                    let content = e.target.result;
                    console.log('content: length[' + content.length + ']');
                    if (typeof content === "string" || content instanceof String) {
                        FS.writeFile(path, content);
                    }
                    else {
                        var view = new Uint8Array(content);
                        console.log('view: length[' + view.byteLength + '] buffer.length[' + view.buffer.length + ']')
                        FS.writeFile(path, view);
                    }
                    
                    var analysis = FS.analyzePath(path);
                    console.log('    isRoot: ' + analysis.isRoot);
                    console.log('    exists: ' + analysis.exists);
                    console.log('    Error:  ' + analysis.Error);
                    console.log('    name:   ' + analysis.name);
                    console.log('    path:   ' + analysis.path);
                    console.log('    parentPath: ' + analysis.parentPath);
                    var stats = FS.stat(path);
                    console.log('    size:   ' + stats.size);
                    console.log('    atime:  ' + stats.atime);
                    console.log('    mtime:  ' + stats.mtime);
                    console.log('    ctime:  ' + stats.ctime);


                    result.Files.push(fileObj);
                    if (completed == picker.files.length)
                        resolve(JSON.stringify(result));
                };
                reader.onabort = function (e) {
                    completed++;
                    result.Aborts.push(fileObj);
                    if (completed == picker.files.length)
                        resolve(JSON.stringify(result));
                }
                reader.onerror = function (e) {
                    completed++;
                    fileObj.Error = reader.error;
                    result.Errors.push(fileObj);
                    if (completed == picker.files.length)
                        resolve(JSON.stringify(result));
                }

                if (file.type.toString().startsWith('text')) {
                    reader.readAsText(file);
                } else {
                    reader.readAsArrayBuffer(file);
                }

                console.log('selectedFile: name[' + file.name + '] size[' + file.size + '] type[' + file.type + ']');
            }
            //resolve(JSON.stringify(result));
        }
        picker.onclose = function () {
            console.log('closed');
        }

        picker.click();
    });
}