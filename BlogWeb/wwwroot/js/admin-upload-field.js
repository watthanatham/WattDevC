// Wires up ".js-upload-field" widgets: a hidden input carries the uploaded
// file's public URL to the server; a thumbnail (image kind) or link (file
// kind) previews the current value. Ported from UploadField.tsx.
//
// While any upload in the field's <form> is in flight, the form's submit
// button is disabled — mirrors post-form.tsx's `uploading > 0` guard, so the
// form can't be submitted before the URL is ready.
$(function () {
  $(".js-upload-field").each(function () {
    var $field = $(this);
    var $hidden = $field.find(".js-upload-hidden");
    var $preview = $field.find(".js-upload-preview");
    var $fileInput = $field.find(".js-upload-input");
    var $status = $field.find(".js-upload-status");
    var folder = $field.data("folder");
    var kind = $field.data("kind") || "image";
    var autoSubmit = $field.data("autosubmit") === true;
    var $submit = $field.closest("form").find("button[type=submit]");

    function setUploading(uploading) {
      if (uploading) {
        $submit.prop("disabled", true).data("upload-lock", true);
        $status.text("กำลังอัปโหลด…");
      } else {
        $submit.prop("disabled", false).removeData("upload-lock");
        $status.text("");
      }
    }

    $fileInput.on("change", function () {
      var file = this.files && this.files[0];
      $fileInput.val("");
      if (!file) return;

      $status.removeClass("text-danger");
      setUploading(true);
      window.uploadToStorage(file, folder).then(
        function (url) {
          $hidden.val(url);
          if (kind === "image") {
            $preview.attr("src", url).show();
          } else {
            $preview.attr("href", url).text("ดูไฟล์ปัจจุบัน →").show();
          }
          setUploading(false);
          // Some fields (e.g. a skill's icon) have no submit button of their own —
          // a successful upload IS the save action.
          if (autoSubmit) $field.closest("form").trigger("submit");
        },
        function (err) {
          $status.addClass("text-danger").text(err.message || "อัปโหลดไม่สำเร็จ");
          $submit.prop("disabled", false).removeData("upload-lock");
        }
      );
    });
  });
});
