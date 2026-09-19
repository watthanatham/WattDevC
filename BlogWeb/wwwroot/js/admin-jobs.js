$(function () {
  var modalEl = document.getElementById("jobModal");
  var $form = $("#jobForm");

  function open(action, title) {
    $form.attr("action", action);
    $("#jobModalTitle").text(title);
    bootstrap.Modal.getOrCreateInstance(modalEl).show();
  }

  // Add: reset() restores the server-rendered defaults (today's date, APPLIED,
  // empty fields) — no need to clear each input by hand.
  $(document).on("click", ".js-job-add", function () {
    $form[0].reset();
    open("/admin/jobs", "เพิ่มการสมัครงาน");
  });

  // Edit: fill the shared form from the row's data-* attributes. Read them with
  // .attr(), not .data() — the latter coerces "2024" or "true" to a non-string.
  $(document).on("click", ".js-job-edit", function () {
    var $btn = $(this);
    $form.find("[name=Company]").val($btn.attr("data-company"));
    $form.find("[name=Position]").val($btn.attr("data-position"));
    $form.find("[name=AppliedDate]").val($btn.attr("data-applied"));
    $form.find("[name=Status]").val($btn.attr("data-status"));
    $form.find("[name=Link]").val($btn.attr("data-link"));
    $form.find("[name=Reason]").val($btn.attr("data-reason"));
    open("/admin/jobs/" + $btn.attr("data-id"), "แก้ไขการสมัครงาน");
  });

  // Server-side validation failed — bring the modal back with what was typed.
  if (window.JOB_FORM_ERROR) {
    open(window.JOB_FORM_ERROR.action, window.JOB_FORM_ERROR.title);
  }

  // Search filters the rendered rows — every row is on the page, so this is a
  // complete search, not a partial one.
  $(document).on("input", "#jobSearch", function () {
    var q = this.value.trim().toLowerCase();
    var shown = 0;
    $("#jobRows tr[data-search]").each(function () {
      var hit = q === "" || $(this).attr("data-search").indexOf(q) !== -1;
      $(this).toggleClass("d-none", !hit);
      if (hit) shown++;
    });
    $("#jobNoMatch").toggleClass("d-none", shown > 0);
  });
});
