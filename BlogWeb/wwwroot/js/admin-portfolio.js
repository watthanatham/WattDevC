$(function () {
  // Skill row: picking a new icon file uploads it, then auto-submits the
  // tiny form that saves it — mirrors SkillRow's handlePick in skills-manager.tsx.
  $(document).on("change", ".js-icon-file", function () {
    var $file = $(this);
    var $form = $file.closest("form");
    var $hidden = $form.find(".js-icon-hidden");
    var $status = $form.find(".js-icon-status");
    var file = this.files && this.files[0];
    $file.val("");
    if (!file) return;

    $status.removeClass("text-danger").text("กำลังอัปโหลด…");
    $file.prop("disabled", true);
    window.uploadToStorage(file, "skills").then(
      function (url) {
        $hidden.val(url);
        $form.trigger("submit");
      },
      function (err) {
        $status.addClass("text-danger").text(err.message || "อัปโหลดไอคอนไม่สำเร็จ");
        $file.prop("disabled", false);
      }
    );
  });

  // Experience row: toggle between the read-only card and its (pre-rendered,
  // hidden) edit form — mirrors ExperienceRow's `editing` state.
  $(document).on("click", ".js-exp-edit", function () {
    var id = $(this).data("id");
    $("#exp-view-" + id).addClass("d-none");
    $("#exp-edit-" + id).removeClass("d-none");
  });
  $(document).on("click", ".js-exp-cancel", function () {
    var id = $(this).data("id");
    $("#exp-edit-" + id).addClass("d-none");
    $("#exp-view-" + id).removeClass("d-none");
  });

  // "ทำอยู่ปัจจุบัน" disables the end-date field — mirrors the isCurrent state
  // driving the `disabled` prop on the end-date input.
  $(document).on("change", ".js-exp-current", function () {
    var $endDate = $(this).closest("form").find(".js-exp-enddate");
    $endDate.prop("disabled", this.checked);
  });
});
