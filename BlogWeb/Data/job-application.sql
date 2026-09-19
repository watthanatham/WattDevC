-- Job application tracker (admin > บันทึกการสมัครงาน).
-- Run once in the Supabase SQL editor. Naming follows the existing
-- Prisma-generated tables: PascalCase table, camelCase quoted columns.
create table if not exists "JobApplication" (
    id            serial primary key,
    company       text not null,
    "position"    text not null,
    "appliedDate" date not null,
    status        text not null default 'APPLIED',
    link          text,
    reason        text
);
