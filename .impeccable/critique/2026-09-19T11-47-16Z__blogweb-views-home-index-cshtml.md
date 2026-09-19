---
target: homepage personal information
total_score: 16
max_score: 24
na_heuristics: 5,7,9,10
p0_count: 0
p1_count: 0
timestamp: 2026-09-19T11-47-16Z
slug: blogweb-views-home-index-cshtml
---
Method: dual-agent (A: /root/design_review · B: /root/design_evidence)

คำตัดสิน: ควรปรับหน้า Formal แบบเฉพาะจุด โดยเปลี่ยนลำดับเนื้อหาและน้ำหนักภาพ ไม่ต้องรื้อเว็บใหม่ เก็บ 8-bit เป็นโหมดเสริม

ตรวจหน้าเว็บจริง localhost:5005 บน desktop และ mobile 390x844 รวม source หน้าแรก CSS navbar และ JS ไม่ได้ตรวจ keyboard/contrast/dark/game แบบครบถ้วน

Design specificity: โหมด Formal สะอาดแต่ยังคล้าย portfolio ทั่วไป จุดแข็งเรื่องระบบโรงงานและ integration ยังไม่ได้เป็นจุดนำสายตา

จุดดี: ชื่อ/บทบาท/การติดต่อชัดเจน; case study มี problem/approach/result จริง; spacing และโครง responsive ใช้งานต่อได้

Priority issues (ทั้งหมด P2 ยกเว้น navigation P3):
1. ผลงานอยู่หลัง stats, About/Skills และ experience ยาว ย้าย selected work ขึ้นหลัง hero (Index.cshtml:82,92,126,173,204). Suggested: impeccable layout.
2. 3+ years ซ้ำ hero; Roles held 1 ให้น้ำหนักมากเกินประโยชน์ ยุบ stats เป็นบรรทัดสั้น (Index.cshtml:53,85). Suggested: impeccable distill.
3. API Center, Safety Patrol, Health Information ซ้ำ experience/case studies/projects รวมการเล่าเป็นผลงานคัดสรรพร้อมผลลัพธ์และภาพที่เปิดเผยได้ (Index.cshtml:148,180,210). Suggested: impeccable shape.
4. ชื่อ gradient กับกรอบภาพเด่นกว่าความเชี่ยวชาญ ลดการตกแต่งแล้วเพิ่มน้ำหนักผลลัพธ์ (site.css:388,447). Suggested: impeccable typeset.
5. Navigation ไม่มีทางลัด Work/Experience/Contact และ INSERT COIN ไม่บอกผลการกดชัดเจน (Navbar:4, site.js:41). Suggested: impeccable clarify.

Nielsen (qualitative): Status 3/4; real-world match 3/4; control 3/4; consistency 3/4; recognition 2/4; aesthetics 2/4. Error prevention, efficiency, error recovery, help n/a ตาม scope portfolio. Total 16/24.

Personas: recruiter ต้องอ่านข้อมูลซ้ำก่อนถึงหลักฐาน; engineering lead ต้องการภาพ/architecture ที่เปิดเผยได้; mobile visitor ใช้ viewport แรกกับรูป/ข้อมูล/ปุ่ม/stats ก่อนเห็นผลงาน.

Cognitive load: ไม่มีตัวเลือกเกิน 4 ที่ hero แต่หน้าอ่านยาวและซ้ำ Emotional journey: เปิดชัดเจน กลางหน้าเป็นข้อความยาว ก่อนปิดด้วย contact ที่ชัดเจน.

Detector: cshtml unsupported (empty result not proof of clean). CSS 7 findings = 6 warnings + 1 advisory: side-tab 202/544/563; overused-font 51/572; gradient-text 391; codex-grid-background 55. หลายรายการเป็น shared/game styles จึงไม่ใช่ปัญหาหน้า Formal โดยตรง ไม่มี overlay เพราะ API ประเมิน DOM ได้แบบ read-only.

Minor: lang=th แต่หน้าอังกฤษ; mobile menu ไม่มี aria-expanded/aria-controls; game pixel text 8-9px. CSS ระบุหมุน avatar wrapper ทั้งชิ้น แต่ภาพจริงที่ตรวจตั้งตรง จึงไม่สรุปเป็นอาการที่เห็น.

แนวทาง: Hero สั้น → Selected work 2–3 เรื่อง → Experience ย่อ + Skills → About/Blog → Contact. ใช้พื้นสว่าง ตัวอักษรเข้ม accent เดียว รูปนิ่ง และผลลัพธ์จริง เช่นข้อมูลลดกระดาษ 90% ที่มีอยู่.

Direction question: เริ่มปรับ Formal ให้เน้นความน่าเชื่อถือและผลงาน โดยเก็บ 8-bit ไว้เป็นโหมดเสริมหรือไม่?
