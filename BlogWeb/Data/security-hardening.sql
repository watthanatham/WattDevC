-- Locks the database down to the app's own connection (role `postgres`, which
-- owns every table and has BYPASSRLS, so nothing here affects BlogWeb itself).
-- Replace :admin_email below before running. Run once in the Supabase SQL editor.
--
-- Why: the anon key is client-side by design, so RLS + grants are the only
-- thing standing between a stranger and these tables.
begin;

-- Supabase's default privileges grant every NEW public table to anon/authenticated.
-- That is how "JobApplication" became world-writable the moment it was created.
alter default privileges in schema public revoke all on tables from anon, authenticated;
alter default privileges in schema public revoke all on sequences from anon, authenticated;
alter default privileges in schema public revoke all on functions from anon, authenticated;

-- A permissive ALL policy would defeat RLS on this table.
drop policy if exists "Anyone can access migrations" on "_prisma_migrations";

alter table "Post"                  enable row level security;
alter table "Profile"               enable row level security;
alter table "JobApplication"        enable row level security;
alter table "Project"               enable row level security;
alter table "Skill"                 enable row level security;
alter table "Experience"            enable row level security;
alter table "_prisma_migrations"    enable row level security;
alter table "__EFMigrationsHistory" enable row level security;

revoke all on "Post", "Profile", "JobApplication", "Skill", "Experience",
              "_prisma_migrations", "__EFMigrationsHistory", "Project"
  from anon, authenticated;

-- Projects are public on the homepage anyway; keep the existing read-only grant.
grant select on "Project" to anon, authenticated;

-- Storage: the media bucket accepted writes from ANY Supabase user, and sign-ups
-- are open. Narrow it to the admin.
drop policy if exists "authenticated_insert_media" on storage.objects;
drop policy if exists "authenticated_update_media" on storage.objects;
drop policy if exists "authenticated_delete_media" on storage.objects;

create policy "admin_insert_media" on storage.objects for insert to authenticated
  with check (bucket_id = 'media' and (auth.jwt() ->> 'email') = :'admin_email');
create policy "admin_update_media" on storage.objects for update to authenticated
  using (bucket_id = 'media' and (auth.jwt() ->> 'email') = :'admin_email');
create policy "admin_delete_media" on storage.objects for delete to authenticated
  using (bucket_id = 'media' and (auth.jwt() ->> 'email') = :'admin_email');

-- 50 MB was the Supabase default; nothing uploaded here needs more than 10.
update storage.buckets set file_size_limit = 10485760 where id = 'media';

commit;
