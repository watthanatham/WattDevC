// Shared across all admin pages: native confirm() before any destructive
// delete form submits — ported from ConfirmSubmitButton.tsx.
$(function () {
  $(document).on("submit", ".js-confirm-form", function (e) {
    if (!confirm($(this).data("confirm"))) e.preventDefault();
  });
});
