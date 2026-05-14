# SKILLS INDEX – ROLE → SKILLS FILE ROUTING

This file defines which skill files must be used for each role.
Open the relevant files in VS Code (or reference them in Copilot Chat prompts)
to ensure Copilot follows the correct guardrails.

All roles MUST always include:
- .ai/core.skills.md

Optional files apply depending on scope (UI, mobile, etc.).

---

## 1. GLOBAL (ALWAYS ON)

- Global Guardrails (mandatory for every task):
  - .ai/core.skills.md

---

## 2. ROLE-BASED ROUTING

### Analyst (BRD / Requirements)
Use:
- .ai/core.skills.md
- .ai/analyst.skills.md

Primary outputs:
- /docs/BRD.md

---

### Architect (HLD / LLD / TDS / DB Schema)
Use:
- .ai/core.skills.md
- .ai/architect.skills.md

Primary outputs:
- /docs/HLD.md
- /docs/LLD.md
- /docs/TDS.md
- /docs/DB_SCHEMA.sql

---

### Developer (Implementation / Code)
Use:
- .ai/core.skills.md
- .ai/developer.skills.md

Also include when applicable:
- UI Branding rules (if any UI is built):
  - .ai/ui-brand.skills.md

Primary inputs:
- /docs/BRD.md
- /docs/HLD.md
- /docs/LLD.md
- /docs/TDS.md
- /docs/DB_SCHEMA.sql

---

### QA (Testing / Validation)
Use:
- .ai/core.skills.md
- .ai/qa.skills.md

Primary inputs:
- /docs/BRD.md
- /docs/LLD.md
- /docs/TDS.md

Primary outputs:
- Test scenarios
- Test cases table
- Edge cases
- Automation strategy
- Risk areas

---

### Orchestrator (Workflow / SDLC Control)
Use:
- .ai/core.skills.md
- .ai/orchestrator.skills.md

Responsibilities:
- SDLC order enforcement
- Stack selection and validation
- Storing:
  - context.techStack
  - context.platformFeatures
- Rejection of unsupported stacks

---

## 3. UI BRANDING

If the project includes Web UI or Mobile UI:
Use:
- .ai/ui-brand.skills.md

Applies to:
- UI layout, typography, colors, logos, Material 3 adherence

Does NOT apply to:
- Backend logic, DB design, API security rules

---

## 4. QUICK USAGE GUIDE (COPILOT IN VS CODE)

### Recommended workflow
- Open the role skill file + core.skills.md in editor tabs before prompting Copilot.
- For UI work, also open ui-brand.skills.md.

### Prompt header (recommended)
Add this header in Copilot Chat prompts to reduce ambiguity:

ROLE: <Analyst | Architect | Developer | QA | Orchestrator>
SKILLS: .ai/core.skills.md, .ai/<role>.skills.md, (optional) .ai/ui-brand.skills.md
INPUTS: /docs/<relevant docs>
TASK: <what to do>

Example:
ROLE: Developer
SKILLS: .ai/core.skills.md, .ai/developer.skills.md
INPUTS: /docs/LLD.md, /docs/TDS.md, /docs/DB_SCHEMA.sql
TASK: Implement FR-3 endpoints + tests, follow platformFeatures and response envelope.

---

## 5. FILE LIST (REFERENCE)

.ai/core.skills.md
.ai/analyst.skills.md
.ai/architect.skills.md
.ai/developer.skills.md
.ai/qa.skills.md
.ai/orchestrator.skills.md
.ai/ui-brand.skills.md
.ai/skills.index.md

---