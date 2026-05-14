# UI BRAND & VISUAL IDENTITY SKILLS (MATERIAL 3)

This file defines the mandatory visual identity and branding rules for all frontend and UI-related implementations, including web and mobile applications. These rules are non-negotiable for the Developer and QA agents.

---

## 1. PRIMARY BRAND IDENTITY

### 1.1 Logo Usage
- **Primary Logo:** Mahindra Red Logo.
- **Logo URL:** `https://www.mahindra.com//sites/default/files/2025-07/mahindra-red-logo.webp`.
- **Placement Requirements:** - The logo MUST be placed in the **AppBar** (top-left) for all screens.
    - The logo MUST be featured in the **Home/Login screen header**.
- **Constraint:** Do not alter logo colors, proportions, or shape. Follow official brand guidelines for clear-space requirements.

---

## 2. TYPOGRAPHY

### 2.1 Approved Fonts
- All fonts MUST be sourced from the approved internal storage: `https://fontsstorage.blob.core.windows.net/html/index.html`.

### 2.2 Primary Headline Font (QuantumRise)
- **Font Family:** QuantumRise GX.
- **Local Paths:**
    - Headline: `/resources/QuantumRiseGX.ttf`.
    - Localized/Devnagiri: `/resources/QuantumDevnagiri.ttf`.
- **Usage Rule:** All headlines (H1 through H6) MUST use the QuantumRise font family.

### 2.3 Body Font
- **Usage Rule:** Body text should use approved system fonts or standard sans-serif fonts defined in the Material 3 theme. Do not introduce unapproved third-party fonts.

---

## 3. COLOR PALETTE (MANDATORY)

Implementation MUST use the following hex codes exactly. Do not use generic CSS color names (e.g., "red").

### 3.1 Primary Colors
- **Primary (Action):** `#E31837` – Rising Red.
- **Secondary (Neutral):** `#4D4D4F` – Steel Grey.

### 3.2 Text Colors
- **Primary Text:** `#231F20` – Ink Black.

### 3.3 Background & Surface
- **Background:** `#E6E7E8` – Light Grey.
- **Surface:** `#FFFFFF` – Ivory.

---

## 4. MATERIAL 3 COMPLIANCE

- **Design System:** Follow Material 3 design principles for all components.
- **Consistency:** Maintain uniform spacing, typography scale, and elevation across all modules.
- **Interaction:** Primary actions (buttons, toggles, active states) MUST use **Rising Red (#E31837)**.
- **Clutter:** Avoid visual clutter; prioritize whitespace and neutral backgrounds to maintain professional enterprise aesthetics.

---

## 5. RESTRICTIONS

- **No Invention:** Do not invent or introduce new brand colors.
- **No Overrides:** Do not override typography rules or use different headline fonts.
- **Domain Isolation:** Do not embed branding logic (hex codes, font loading) into backend services or APIs.
- **Scope:** These rules apply strictly to UI/UX concerns and visual presentation layers.

---