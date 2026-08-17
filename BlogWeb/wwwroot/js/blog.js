// Post-processes a sanitized post body: wraps standalone images in a
// captioned <figure data-width="...">, and opens a click-to-zoom lightbox.
// Ported from src/components/blog/article-body.tsx.
$(function () {
  var $article = $(".js-article-body");
  if ($article.length) {
    $article.find("img").each(function () {
      var $img = $(this);
      if ($img.closest("figure").length) return;

      $img.addClass("cursor-zoom-in").attr("loading", "lazy");

      // Gallery images stay in their grid — still clickable, not wrapped.
      if ($img.closest("[data-gallery]").length) return;

      var width = $img.attr("data-width") || "normal";
      var alt = $img.attr("alt");
      var $fig = $("<figure>").attr("data-width", width);
      $img.before($fig);
      $fig.append($img);
      if (alt) {
        $fig.append($("<figcaption>").text(alt));
      }
    });
  }

  $(document).on("click", ".js-article-body img", function () {
    var src = this.src;
    var alt = $(this).attr("alt") || "";
    var $overlay = $('<div class="lightbox-overlay" role="dialog" aria-modal="true"></div>');
    $overlay.append($('<button class="lightbox-close" aria-label="Close">&times;</button>'));
    $overlay.append($("<img>").attr({ src: src, alt: alt }).on("click", function (e) { e.stopPropagation(); }));
    $overlay.on("click", function () { close(); });
    $overlay.find(".lightbox-close").on("click", function () { close(); });
    $("body").append($overlay).css("overflow", "hidden");

    function close() {
      $overlay.remove();
      $("body").css("overflow", "");
      $(document).off("keydown.lightbox");
    }
    $(document).on("keydown.lightbox", function (e) {
      if (e.key === "Escape") close();
    });
  });
});
