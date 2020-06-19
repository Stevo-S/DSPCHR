function setSubscriptionPath() {
    var xhttp;
    xhttp = new XMLHttpRequest();
    xhttp.open('GET', 'http://header.safaricombeats.co.ke/dxl/');
    xhttp.onreadystatechange = function () {

        if (this.readyState == 4 && this.status == 200) {
            sReadyState = this.readyState;
            sStatus = this.status;

            var result = xhttp.responseText;

            console.log(result);

            var response = JSON.parse(result);
            var responseMessage = response["ServiceResponse"]["ResponseHeader"]["ResponseMsg"];
            var subscribeLink = document.getElementById("subscribeLink");

            console.log("Response: " + responseMessage);

            if (responseMessage.toLowerCase() == "success") {
                var msisdn = response["ServiceResponse"]["ResponseBody"]["Response"]["Msisdn"];

                subscribeLink.setAttribute("data-msisdn", msisdn);
                subscribeLink.onclick = subscribe;
                console.log("Msisdn: " + msisdn);

                // Subscribe right away
                subscribe();
            }
            else {
                console.log("Falling back to USSD...");
                //subscribeLink.click();
                document.location.href = subscribeLink.getAttribute("href");
            }
        }
    };
    xhttp.onerror = function (e) {
        console.log(e);
    }
    xhttp.send();
}

function subscribe() {
    var xhr = new XMLHttpRequest();
    var url = "http://197.248.181.130/dspchr/api/Subscriptions/Activate";
    var subscribeLink = document.getElementById("subscribeLink");
    var msisdn = subscribeLink.getAttribute("data-msisdn");
    var offerCode = subscribeLink.getAttribute("data-offercode");
    var clickId = parseInt(subscribeLink.getAttribute("data-clickId"));
    //var clickId = subscribeLink.getAttribute("data-clickId");
    xhr.open("POST", url, true);
    xhr.setRequestHeader("Content-Type", "application/json");
    xhr.onreadystatechange = function () {
        if (xhr.readyState === 4 && xhr.status === 200) {
            //var json = JSON.parse(xhr.responseText);
            var result = xhr.responseText;
            console.log("Subscribing MSISDN: " + msisdn + " on Offer Code " + offerCode);
            console.log("Activation status: " + result);
        }
    };
    var data = JSON.stringify({ "msisdn": msisdn, "offerCode": offerCode, "clickId": clickId });
    console.log(data);
    xhr.send(data);
}

window.onload = setSubscriptionPath;