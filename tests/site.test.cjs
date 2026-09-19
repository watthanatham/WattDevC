// Run: node tests/site.test.cjs
const assert = require('node:assert/strict');
const fs = require('node:fs');
const vm = require('node:vm');

function element(initial = []) {
  const classes = new Set(initial);
  return {
    attributes: {}, events: {},
    classList: {
      contains: name => classes.has(name),
      add: name => classes.add(name),
      toggle(name) {
        if (classes.has(name)) { classes.delete(name); return false; }
        classes.add(name); return true;
      }
    },
    setAttribute(name, value) { this.attributes[name] = value; },
    addEventListener(name, fn) { this.events[name] = fn; },
    focus() { this.focused = true; }
  };
}

const root = element(['dark']);
const theme = element();
const button = element();
const menu = element(['d-none']);
const events = {};
const document = {
  documentElement: root,
  querySelector: selector => ({ '.js-theme-toggle': theme, '.js-nav-toggle': button, '#homeMobileNav': menu })[selector],
  addEventListener: (name, fn) => { events[name] = fn; }
};
vm.runInNewContext(fs.readFileSync(require('node:path').join(__dirname, '../BlogWeb/wwwroot/js/site.js'), 'utf8'), {
  document,
  // Blocked browser storage must not prevent either control from working.
  localStorage: { setItem() { throw new Error('Storage blocked'); } }
});
events.DOMContentLoaded();
assert.equal(theme.attributes['aria-pressed'], 'true');
theme.events.click();
assert.equal(root.classList.contains('dark'), false);
assert.equal(theme.attributes['aria-label'], 'Switch to dark mode');
button.events.click();
assert.equal(menu.classList.contains('d-none'), false);
assert.equal(button.attributes['aria-expanded'], 'true');
events.keydown({ key: 'Escape' });
assert.equal(menu.classList.contains('d-none'), true);
assert.equal(button.attributes['aria-expanded'], 'false');
assert.equal(button.focused, true);
button.events.click();
menu.events.click({ target: { closest: () => ({}) } });
assert.equal(menu.classList.contains('d-none'), true);
assert.equal(button.attributes['aria-expanded'], 'false');
console.log('Theme, blocked storage, mobile navigation, and Escape checks passed.');

const home = fs.readFileSync(require('node:path').join(__dirname, '../BlogWeb/Views/Home/Index.cshtml'), 'utf8');
const navbar = fs.readFileSync(require('node:path').join(__dirname, '../BlogWeb/Views/Shared/_Navbar.cshtml'), 'utf8');
const inOrder = (source, values) => values.every((value, index) =>
  index === 0 || source.indexOf(values[index - 1]) < source.indexOf(value));
assert.ok(inOrder(home, ['id="experience"', 'id="work"', 'id="blog"', 'id="contact"']));
assert.ok(inOrder(navbar, ['("/#experience"', '("/#work"', '("/#contact"', '("/blog"']));
console.log('Homepage and navigation order checks passed.');
