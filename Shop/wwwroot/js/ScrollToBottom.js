window.ScrollToBottom = function (element) {
    if (element) {
        element.scrollTop = element.scrollHeight;
        console.log("ScrollToBottom called");
    }
}