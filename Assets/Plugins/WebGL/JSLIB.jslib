mergeInto(LibraryManager.library,
{
 
    SaveScreenshotWebGL: function(filename, data)
    {
        const pageImage = new Image();
        filename = Pointer_stringify(filename);
       
        if(!filename.endsWith('.png'))
        {
            filename += '.png';
        }
       
        pageImage.src =  'data:image/png;base64,' + Pointer_stringify(data);
       
        pageImage.onload = function()
        {
            const canvas = document.createElement('canvas');
            canvas.width = pageImage.naturalWidth;
            canvas.height = pageImage.naturalHeight;
 
            const ctx = canvas.getContext('2d');
            ctx.imageSmoothingEnabled = false;
            ctx.drawImage(pageImage, 0, 0);
            saveScreenshot(canvas);
        }
 
        function saveScreenshot(canvas)
        {
            const link = document.createElement('a');
            link.download = filename;
           
            canvas.toBlob(function(blob)
            {
                link.href = URL.createObjectURL(blob);
                link.click();
            });
        };
    }
 
});

var CheckMobile = {
    IsMobile : function()
    {
        return Module.SystemInfo.mobile;
    }
};
mergeInto(LibraryManager.library, CheckMobile);