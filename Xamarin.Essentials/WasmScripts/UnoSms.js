function UnoSms_IsSupported() {
    return true;
}

function UnoSms_Compose(recipient, body) {
    if (!IsNullEmptyOrWhiteSpace(recipient)) {
        let url = 'sms:' + recipient
        if (!IsNullEmptyOrWhiteSpace(body))
            url += '?body=' + body;
        window.open(url,'_self');
    }
}