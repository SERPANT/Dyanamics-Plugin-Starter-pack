/**
 * Get request to a URL
 * 
 * @param {string} url: API Url
 * @param {Function} callBack: callback function
 * @param {any} parameterToCallBack: Any parameter to pass to callback
 */
function get(url, callBack, parameterToCallBack) {
    var req = new XMLHttpRequest();
    req.open(requestTypes.get, url, true);
    req.setRequestHeader("Accept", "application/json");
    req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
    req.setRequestHeader("OData-MaxVersion", "4.0");
    req.setRequestHeader("OData-Version", "4.0");

    req.onreadystatechange = e => {
        if (req.status === httpStatus.success && req.readyState === requestReadyState.done) {
            var { value } = JSON.parse(req.response);
            callBack(value, parameterToCallBack);
        }
    };
    req.send();
}
