function UnoTextToSpeech_GetVoicesPromise() {
    return new Promise(
        function (resolve, reject) {
            if ('speechSynthesis' in window) {
                let synth = window.speechSynthesis;
                let id;

                id = setInterval(() => {
                    if (synth.getVoices().length !== 0) {
                        resolve(GetVoices());
                        clearInterval(id);
                    }
                }, 10);
            }
            else
                resolve('NOT_AVAILABLE');
        }
    );
}

function UnoTextToSpeech_GetVoices() {
    if (typeof speechSynthesis === 'undefined') {
        return 'NONE';
    }
    var result = '';
    var voices = speechSynthesis.getVoices();

    for (var i = 0; i < voices.length; i++) {
        result += voices[i].name + ':' + voices[i].lang;
        if (voices[i].default)
            result += '-DEFAULT';
        result += ';';
    }
    return result;
}

function UnoTextToSpeech_PerformSpeekPromise(text, name, lang, volume, pitch) {
    return new Promise(
        function (resolve, reject) {
            if ('speechSynthesis' in window) {
                var synth = window.speechSynthesis;
                var utterance = new SpeechSynthesisUtterance(text);
                var found = false;
                if (!IsNullEmptyOrWhiteSpace(name)) {
                    for (i = 0; i < voices.length; i++) {
                        if (voices[i].name === name) {
                            utterance.voice = voices[i];
                            found = true;
                        }
                    }
                }
                if (!found) {
                    for (i = 0; i < voices.length; i++) {
                        if (voices[i].lang === lang) {
                            utterance.voice = voices[i];
                            found = true;
                        }
                    }
                }
                if (!IsNullEmptyOrWhiteSpace(volume)) {
                    utterance.volume = volume;
                }
                if (!IsNullEmptyOrWhiteSpace(pitch)) {
                    utterance.pitch = pitch;
                }
                synth.speak(utterance);
            }
            else
                resolve('NOT_AVAILABLE');
        }
    );
}

