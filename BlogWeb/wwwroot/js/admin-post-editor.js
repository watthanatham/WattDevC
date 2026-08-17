// Summernote setup for the post body editor — replaces Tiptap. Produces the
// same HTML shape the server sanitizer (PostHtmlSanitizer) and the reader
// (blog-prose CSS) already expect: <img data-width="normal|wide|full"> and
// <div data-gallery> wrapping multiple <img>s. Ported from rich-editor.tsx.
//
// Simplification vs. the original: instead of a floating per-image toolbar,
// clicking an inserted image opens a small prompt to change its width/caption
// or delete it — same outcome (same data-width values), simpler to build.
$(function () {
  var $editorEl = $(".js-post-editor");
  if ($editorEl.length === 0) return;

  function uploadOne(file) {
    if (!/^image\//.test(file.type)) return Promise.resolve(null);
    return window.uploadToStorage(file, "posts/body").then(
      function (src) { return { src: src, alt: "" }; },
      function (err) {
        alert(err.message || "อัปโหลดรูปไม่สำเร็จ");
        return null;
      }
    );
  }

  function insertImage(context, src, alt, width) {
    var img = $("<img>").attr({ src: src, alt: alt || "", "data-width": width || "normal" });
    context.invoke("editor.insertNode", img[0]);
  }

  function insertGallery(context, images) {
    var $gallery = $("<div>").attr("data-gallery", "");
    images.forEach(function (im) {
      $gallery.append($("<img>").attr({ src: im.src, alt: im.alt || "" }));
    });
    context.invoke("editor.insertNode", $gallery[0]);
  }

  function handleFiles(context, files) {
    Promise.all(files.map(uploadOne)).then(function (uploaded) {
      uploaded = uploaded.filter(Boolean);
      if (uploaded.length === 0) return;

      if (uploaded.length === 1) {
        var caption = window.prompt("คำบรรยายรูป (ไม่บังคับ)", "") || "";
        insertImage(context, uploaded[0].src, caption, "normal");
      } else {
        insertGallery(context, uploaded);
      }
    });
  }

  $editorEl.summernote({
    height: 320,
    placeholder: "เขียนเรื่องราวของคุณ…",
    toolbar: [
      ["style", ["bold", "italic"]],
      ["para", ["ul", "ol", "paragraph"]],
      ["insert", ["uploadImage", "insertGallery"]],
    ],
    styleTags: [
      { title: "Paragraph", tag: "p", value: "p" },
      { title: "Heading 2", tag: "h2", value: "h2" },
      { title: "Heading 3", tag: "h3", value: "h3" },
      { title: "Quote", tag: "blockquote", value: "blockquote" },
    ],
    buttons: {
      uploadImage: function (context) {
        var ui = $.summernote.ui;
        var $input = $('<input type="file" accept="image/*" multiple style="display:none">');
        $input.on("change", function () {
          var files = Array.from(this.files || []);
          $input.val("");
          if (files.length) handleFiles(context, files);
        });
        var button = ui.button({
          contents: '<i class="note-icon">🖼</i>',
          tooltip: "แทรกรูป",
          click: function () { $input.trigger("click"); },
        });
        $("body").append($input);
        return button.render();
      },
      insertGallery: function (context) {
        var ui = $.summernote.ui;
        return ui.button({
          contents: '<i class="note-icon">▦</i>',
          tooltip: "แกลเลอรีหลายรูป",
          click: function () {
            var $input = $('<input type="file" accept="image/*" multiple style="display:none">');
            $input.on("change", function () {
              var files = Array.from(this.files || []);
              if (files.length > 1) handleFiles(context, files);
              else if (files.length === 1) alert("เลือกอย่างน้อย 2 รูปสำหรับแกลเลอรี");
              $input.remove();
            });
            $("body").append($input);
            $input.trigger("click");
          },
        }).render();
      },
    },
    callbacks: {
      onChange: function (contents) {
        $editorEl.val(contents);
      },
    },
  });

  // Click-to-edit an existing image: change width variant, edit caption, or delete.
  var $editable = $editorEl.next(".note-editor").find(".note-editable");
  $editable.on("click", "img", function () {
    var img = this;
    var current = img.getAttribute("data-width") || "normal";
    var choice = window.prompt(
      'ปรับรูป: พิมพ์ normal / wide / full เพื่อปรับความกว้าง, หรือ "ลบ" เพื่อลบรูป',
      current
    );
    if (choice === null) return;
    choice = choice.trim().toLowerCase();
    if (choice === "ลบ" || choice === "delete") {
      var $gallery = $(img).closest("[data-gallery]");
      $(img).remove();
      if ($gallery.length && $gallery.children("img").length === 0) $gallery.remove();
      $editorEl.val($editable.html());
      return;
    }
    if (["normal", "wide", "full"].includes(choice)) {
      img.setAttribute("data-width", choice);
      $editorEl.val($editable.html());
    }
  });
});
