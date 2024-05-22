window.resizeObserverInterop = {
    initialize: (dotnetHelper) => {
        const resizeObserver = new ResizeObserver(entries => {
            for (let entry of entries) {
                dotnetHelper.invokeMethodAsync('OnResize', Math.round(entry.contentRect.width), Math.round(entry.contentRect.height));
            }
        });
        resizeObserver.observe(document.body);
    }
};