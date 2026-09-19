// The portfolio has one presentation; only the light/dark preference persists.
document.addEventListener("DOMContentLoaded", function () {
  var root = document.documentElement;
  var themeButton = document.querySelector(".js-theme-toggle");
  var menuButton = document.querySelector(".js-nav-toggle");
  var menu = document.querySelector("#homeMobileNav");

  function renderTheme() {
    var dark = root.classList.contains("dark");
    themeButton.setAttribute("aria-pressed", String(dark));
    themeButton.setAttribute("aria-label", dark ? "Switch to light mode" : "Switch to dark mode");
  }

  if (themeButton) {
    renderTheme();
    themeButton.addEventListener("click", function () {
      var dark = root.classList.toggle("dark");
      try { localStorage.setItem("theme", dark ? "dark" : "light"); } catch (e) {}
      renderTheme();
    });
  }

  // Hero cursor light: fine pointers only, off under reduced motion.
  var hero = document.querySelector("[data-spotlight]");
  if (hero && window.matchMedia("(hover: hover) and (pointer: fine) and (prefers-reduced-motion: no-preference)").matches) {
    hero.addEventListener("pointermove", function (event) {
      var box = hero.getBoundingClientRect();
      hero.style.setProperty("--mx", event.clientX - box.left + "px");
      hero.style.setProperty("--my", event.clientY - box.top + "px");
      hero.classList.add("is-lit");
    });
    hero.addEventListener("pointerleave", function () { hero.classList.remove("is-lit"); });
  }

  // Scroll reveal: .reveal elements fade up once as they enter the viewport.
  // Elements arriving in the same frame (a row of cards) stagger by 90ms.
  // _Layout's head script sets .js-reveal; without it everything stays visible.
  if (root.classList.contains("js-reveal")) {
    var observer = new IntersectionObserver(function (entries) {
      var batch = 0;
      entries.forEach(function (entry) {
        if (!entry.isIntersecting) return;
        var el = entry.target;
        observer.unobserve(el);
        el.style.setProperty("--d", Math.min(batch++ * 90, 450) + "ms");
        el.classList.add("is-in");
        el.addEventListener("transitionend", function done(event) {
          if (event.target !== el || event.propertyName !== "opacity") return;
          el.removeEventListener("transitionend", done);
          el.classList.add("is-done");
          el.style.removeProperty("--d");
        });
      });
    }, { rootMargin: "0px 0px -8% 0px" });
    document.querySelectorAll(".reveal").forEach(function (el) { observer.observe(el); });
  }

  if (menuButton && menu) {
    function closeMenu() {
      menu.classList.add("d-none");
      menuButton.setAttribute("aria-expanded", "false");
    }
    menuButton.addEventListener("click", function () {
      var hidden = menu.classList.toggle("d-none");
      menuButton.setAttribute("aria-expanded", String(!hidden));
    });
    menu.addEventListener("click", function (event) {
      if (event.target.closest("a")) closeMenu();
    });
    document.addEventListener("keydown", function (event) {
      if (event.key === "Escape" && !menu.classList.contains("d-none")) {
        closeMenu();
        menuButton.focus();
      }
    });
  }
});
