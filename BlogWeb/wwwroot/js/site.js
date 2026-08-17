// Theme (light/dark) + site mode (formal/game) toggles.
// Ported from src/lib/use-mode.ts + theme-toggle.tsx + mode-toggle.tsx —
// same localStorage keys ('theme', 'mode') so a visitor's choice on either
// app carries over if they ever load both against the same browser.
$(function () {
  var $html = $(document.documentElement);

  function isDark() {
    return $html.hasClass("dark");
  }

  function isGame() {
    return $html.attr("data-mode") === "game";
  }

  function renderToggles() {
    var dark = isDark();
    var game = isGame();

    $(".js-theme-toggle").each(function () {
      $(this).find(".js-theme-icon").text(dark ? "☾" : "☀");
      $(this).attr("title", dark ? "Light mode" : "Dark mode");
    });

    $(".js-mode-toggle").each(function () {
      $(this).text(game ? "💼 FORMAL MODE" : "🕹️ INSERT COIN");
      $(this)
        .toggleClass("pixel-btn bg-xp", game)
        .toggleClass("mode-btn-formal", !game);
    });
  }

  $(document).on("click", ".js-theme-toggle", function () {
    var next = !isDark();
    $html.toggleClass("dark", next);
    try {
      localStorage.setItem("theme", next ? "dark" : "light");
    } catch (e) {}
    renderToggles();
  });

  $(document).on("click", ".js-mode-toggle", function () {
    var next = isGame() ? "formal" : "game";
    if (next === "game") {
      $html.attr("data-mode", "game");
    } else {
      $html.removeAttr("data-mode");
    }
    try {
      localStorage.setItem("mode", next);
    } catch (e) {}
    renderToggles();
  });

  $(document).on("click", ".js-nav-toggle", function () {
    $($(this).data("target")).toggleClass("d-none");
  });

  renderToggles();
});
