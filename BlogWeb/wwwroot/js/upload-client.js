// Uploads a file straight from the browser to Supabase Storage and resolves
// with its public URL — ported from src/lib/upload-client.ts. Bypassing our
// own server avoids ever needing a large-file upload endpoint.
//
// Auth: the admin's Supabase session (captured at login — see AccountController
// + the sb-access-token/sb-refresh-token cookies) is restored into supabase-js
// below, so the bucket's "authenticated insert" RLS policy is satisfied.
(function () {
  function readCookie(name) {
    var match = document.cookie.match(new RegExp("(?:^|; )" + name + "=([^;]*)"));
    return match ? decodeURIComponent(match[1]) : null;
  }

  var cfg = window.SUPABASE_CONFIG || {};
  var client = window.supabase.createClient(cfg.url, cfg.anonKey);

  var accessToken = readCookie("sb-access-token");
  var refreshToken = readCookie("sb-refresh-token");
  var sessionReady = accessToken && refreshToken
    ? client.auth.setSession({ access_token: accessToken, refresh_token: refreshToken })
    : Promise.resolve();

  window.uploadToStorage = function (file, folder) {
    var ext = (file.name.split(".").pop() || "bin").toLowerCase();
    var path = folder + "/" + Date.now() + "-" + Math.random().toString(36).slice(2) + "." + ext;

    return sessionReady.then(function () {
      return client.storage.from(cfg.bucket).upload(path, file, {
        contentType: file.type || undefined,
        upsert: false,
      });
    }).then(function (result) {
      if (result.error) throw new Error(result.error.message);
      return client.storage.from(cfg.bucket).getPublicUrl(path).data.publicUrl;
    });
  };
})();
