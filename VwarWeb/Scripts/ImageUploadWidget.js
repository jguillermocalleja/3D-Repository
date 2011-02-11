﻿function resizeImage(element) {
    var img = new Image();
    img.src = $(element).attr('src');

    var elementWidth = img.width;
    var elementHeight = img.height;
    var ratio = elementWidth / (elementHeight * 1.0);

    var container = $(element).parent();
    var parentWidth = $(container).width();
    var parentHeight = $(container).height();

    if (elementWidth > parentWidth || elementHeight > parentHeight) {
        if (elementWidth < elementHeight) {
            $(element).attr('width', parentHeight * ratio);
            $(element).attr('height', parentHeight);
        } else {
            $(element).attr('width', parentWidth);
            $(element).attr('height', parentWidth / ratio);
        }
    } else {
        $(element).attr('width', elementWidth);
        $(element).attr('height', elementHeight);
    }
}

function ImageUploadWidget(property, WidgetContainer) {

    this.Finished = false;
    this.ProgressBar = $(WidgetContainer).find('.progressbarContainer');
    this.StatusIcon = $(WidgetContainer).find('.statusIcon');
    this.StatusText = $(WidgetContainer).find('.statusText');
    this.ErrorMessage = $(WidgetContainer).find('.errorMessage');
    this.PreviewImage = $(WidgetContainer).find('.previewImage');
    this.LoadingImageContainer = $(WidgetContainer).find('.LoadingImageContainer');
    this.UploadButton = $(WidgetContainer).find('.rr-upload-button');
    $(this.LoadingImageContainer).hide();

    this.CancelButton = $(WidgetContainer).find('.cancel');
    this.IsCancelled = false;
    this.Active = false;

    $(this.PreviewImage).load(jQuery.proxy(function () {
        $(this.LoadingImageContainer).hide();
        resizeImage($(this.PreviewImage));
        $(this.PreviewImage).show();
    }, this));

    $(this.CancelButton).click(function(){
        $(this.CancelButton).hide();
        this.IsCancelled = true;
        $(this.StatusText).html("Cancelled");
        $(this.StatusIcon).attr("src", failLocation);
    });


    this.CompleteCallback = function (id, fileName, responseJSON) {
        $(this.LoadingImageContainer).hide();
        if (responseJSON.success == "true") {
            if (!this.IsCancelled) {
                $(this.CancelButton).hide();
                $(this.ProgressBar).progressbar("option", "value", 100);
                $(this.ProgressBar).slideUp(400);
                $(this.StatusText).html("Upload Complete");
                $(this.StatusIcon).attr("src", checkLocation);
                $(this.PreviewImage).attr("src", "../Public/Upload.ashx?image=true&method=get&hashname=" + responseJSON.newfilename + "&time=" + new Date().getTime());
                this.Finished = true;
            } else {
                this.DeleteTempImage(newfilename);
            }
        } else {
            $(this.CancelButton).hide();
            if (!this.IsCancelled) {
                $(this.ProgressBar).slideUp(400, function () { $(this.StatusText).html("Upload Failed"); });
                $(this.StatusIcon).attr("src", failLocation);
                $(this.ErrorMessage).show();
                $(this.ErrorMessage).html('An error occured while trying to upload your image. The server may be busy or down. Please try again.');
                $(this.PreviewImage).attr("src", "../Images/SmallUpload_Btn.png");
                $(this.PreviewImage).css("width", $(this.PreviewImage).parent().css("width"));
                $(this.PreviewImage).css("height", $(this.PreviewImage).parent().css("height"));
            }
        }
    }

    this.FileDialogCompleteCallback = function (id, filename) {
        cancelled = false;
        $(this.PreviewImage).hide();
        $(this.LoadingImageContainer).show();
        $(this.ErrorMessage).hide();
        $(this.CancelButton).show();
        $(this.UploadButton).removeClass("qq-upload-button-hover");
        $(this.StatusText).html("Uploading image...");
        $(this.StatusIcon).show();
        $(this.StatusIcon).attr("src", loadingLocation);
        if (browserVersion == -1) {
            $(this.ProgressBar).progressbar();
            $(this.ProgressBar).show();
            $(this.ProgressBar).progressbar("option", "value", 0);
        }
        return true;
    }

    this.ProgressCallback = function (id, filename, bytesLoaded, totalBytes) {
         totalBytes *= 1.0; bytesLoaded *= 1.0;
         result = (bytesLoaded / totalBytes) * 100.0;
         $(this.ProgressBar).progressbar("option", "value", result);
    }


    this.DeleteTempImage = function(filename) {
        $.ajax({
            type: "post",
            url: "Upload.aspx/DeleteTempImage",
            data: '{ "filename" : "' + filename + '"}',
            contentType: "application/json; charset=utf-8",
            dataType: "json"
        });
    }

    this.Activate = function (hashname) {
        $(this.LoadingImageContainer).hide();
        this.FileUploader = new qq.FileUploaderBasic({
            button: $(WidgetContainer).find('.rr-upload-button').get(0),
            action: '../Public/Upload.ashx?image=true&method=set&property=' + property + "&hashname=" + hashname,
            allowedExtensions: ['png', 'jpg', 'gif'],
            sizeLimit: 2097152, //2MB
            minSizeLimit: 1024, //1KB
            onSubmit: jQuery.proxy(this.FileDialogCompleteCallback, this),
            onProgress: jQuery.proxy(this.ProgressCallback, this),
            onComplete: jQuery.proxy(this.CompleteCallback, this)
        });
    }



    return true;
}