let voices = speechSynthesis.getVoices();

function UnoTextToSpeech_GetVoicesPromise() {
    return new Promise(
        function (resolve, reject) {
            if ('speechSynthesis' in window) {
                resolve(GetVoices());
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
    let result = '';
    let voices = speechSynthesis.getVoices();

    for (let i = 0; i < voices.length; i++) {
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
                let synth = window.speechSynthesis;
                let utterance = new SpeechSynthesisUtterance(text);
                let voices = speechSynthesis.getVoices();

                let found = false;
                for (i = 0; i < voices.length; i++) {
                    console.log('name=' + voices[i].name);
                    if (voices[i].name === name) {
                        utterance.voice = voices[i];
                        found = true;
                        break;
                    }
                }
                if (!found) {
                    for (i = 0; i < voices.length; i++) {
                        console.log('lang=' + voices[i].lang);
                        if (voices[i].lang === lang) {
                            utterance.voice = voices[i];
                            found = true;
                            break;
                        }
                    }
                }
                
                utterance.volume = volume;
                utterance.pitch = pitch;
                synth.speak(utterance);
                resolve("DONE");
            }
            else
                resolve("NOT_AVAILABLE");
        }
    );
}

