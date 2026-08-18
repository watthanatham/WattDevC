// Theme (light/dark) + site mode (formal/game) toggles.
// Ported from src/lib/use-mode.ts + theme-toggle.tsx + mode-toggle.tsx —
// same localStorage keys ('theme', 'mode') so a visitor's choice on either
// app carries over if they ever load both against the same browser.
$(function () {
  var $html = $(document.documentElement);
  var $overlay = $("#modeTransitionOverlay");
  var reduceMotion = window.matchMedia && window.matchMedia("(prefers-reduced-motion: reduce)").matches;

  // Brief flash so the theme/mode swap (an instant class toggle) reads as a
  // deliberate transition instead of an abrupt flicker.
  function withFlash(apply) {
    if (reduceMotion || !$overlay.length) {
      apply();
      return;
    }
    $overlay.addClass("is-active");
    setTimeout(function () {
      apply();
      setTimeout(function () { $overlay.removeClass("is-active"); }, 150);
    }, 150);
  }

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
    withFlash(function () {
      var next = !isDark();
      $html.toggleClass("dark", next);
      try {
        localStorage.setItem("theme", next ? "dark" : "light");
      } catch (e) {}
      renderToggles();
    });
  });

  $(document).on("click", ".js-mode-toggle", function () {
    withFlash(function () {
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
  });

  $(document).on("click", ".js-nav-toggle", function () {
    $($(this).data("target")).toggleClass("d-none");
  });

  renderToggles();
});

// Scroll reveal for Home (formal mode) — fades/slides sections and fills
// skill bars in as they enter the viewport. Game mode has no .reveal /
// .skill-bar-fill elements, so this is a no-op there.
(function () {
  var reduceMotion = window.matchMedia && window.matchMedia("(prefers-reduced-motion: reduce)").matches;
  var els = document.querySelectorAll(".reveal, .skill-bar-fill");
  if (!els.length) return;

  if (reduceMotion || !("IntersectionObserver" in window)) {
    els.forEach(function (el) { el.classList.add("is-visible"); });
    return;
  }

  var observer = new IntersectionObserver(function (entries) {
    entries.forEach(function (entry) {
      if (entry.isIntersecting) {
        entry.target.classList.add("is-visible");
        observer.unobserve(entry.target);
      }
    });
  }, { threshold: 0.15 });

  els.forEach(function (el) { observer.observe(el); });
})();
