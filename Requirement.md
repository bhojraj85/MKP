# One-Page SOP: New Employee Joining (KT Learning Platform)

## Objective
Enable every new joiner to become production-ready through structured KT and controlled access provisioning.

## Process Snapshot
1. HR initiates onboarding for new joinners.
2. Temporary ID is created in msetu portal to access KT Session.
3. Role/department mapping for new joiiners.
4. Content Admin curates KT assets for role/department wise (add/update/delete videos, PDFs, and documents) and also create KT quiz Master according to Role.
5. KT learning path is auto-assigned to new joinners according to there role after login with there temporary ID.
6. Joiner completes KT sessions and modules.
7. KT quiz is triggered.
8. Decision gate:
   - Pass (>= passing score): approval mail sent to Admin/AmiT for creating permanent NT ID created.
   - Fail (< passing score): remedial KT assigned and re-quiz scheduled.
9. Once NT Id(Permannet ID in mahindra domain) created Process End   

## Decision Logic
- Passing score: 70% (configurable)
- Max attempts before manager escalation: 3 (configurable)
- NT ID creation condition: quiz status must be PASS

## Ownership Matrix
- Admin: onboarding start + basic profile
- Content Admin: manage KT library (add/update/delete content assets)
- KT Coordinator: module/session assignment
- Manager: readiness oversight + escalation
- IT/Admin: NT ID creation after pass
- Employee: complete KT + clear quiz

## SLA Targets
- Temporary ID creation: <= 1 business Day
- KT assignment: <= 1 business day
- Quiz result: immediate to <= 2 hours
- NT ID creation post-pass: <= 7 business day

## KPIs
- First-attempt pass rate
- Average days to onboarding completion
- Re-KT assignment rate
- NT ID TAT (turnaround time)
- Audit compliance score

## Diagram Sources
- Process diagram: docs/diagrams/onboarding-process.mmd
- System architecture: docs/diagrams/system-architecture.mmd
