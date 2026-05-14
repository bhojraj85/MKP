# CORE SKILLS – GLOBAL SDLC & SYSTEM GUARDRAILS

This file defines the **Non-Negotiable Constitutional Laws** for all agents. Every design, line of code, and test case must comply with these rules.

## 1. THE STANDARD API RESPONSE ENVELOPE (MANDATORY)
```json
{
  "success": boolean,
  "message": "User-friendly description of the result",
  "data": object | array | null,
  "errors": [
    {
      "code": "STRING_ERROR_CODE",
      "message": "Detailed developer-friendly message"
    }
  ]
}
```

### Response Rules:
* **Success (2xx):** `success` is `true`, `errors` is `null`.
* **Business Failures (4xx):** `success` is `false`, `message` is a client-safe explanation, `data` is `null`.
* **Technical Failures (5xx):** `success` is `false`, `message` is generic ("Internal Server Error"), `errors` contains a tracking ID. **NEVER** expose stack traces.

---

## 2. SECURITY GUARDRAILS (ZERO-TRUST & TOKEN LIFECYCLE)

### 2.1 Token Constraints
* **Access Token:** JWT-based, strictly **2-minute** validity.
* **Refresh Token:** **90-day** validity, **Single-Use (Rotation)** policy.
* **Breach Detection:** Any reuse of a rotated refresh token MUST trigger immediate revocation of the entire session family.

### 2.2 Configuration & Secrets
* **Storage:** **ALL** application secrets (DB strings, API keys) **MUST** be retrieved from **Google Secret Manager** at runtime.
* **Traceability:** Every protected endpoint must map to an **Ownership Check** in the Service Layer.

### 2.3 Security Baselines: OWASP, CWE, & ASVS Level 2
The following controls are MANDATORY CONSTRAINTS for the Architect and Developer. Compliance is verified during the Stage 7 Audit, but implementation MUST occur at the design and coding stages.

#### **OWASP Top 10 (2025)**
* **A01-A10:** A01–A10: (Broken Access Control, Security Misconfiguration, Software Supply Chain Failures, Cryptographic Failures, Injection, Insecure Design, Authentication  Failures, Software or Data Integrity Failures, Security Logging & Alerting Failures, Mishandling of Exceptional Conditions)

#### **CWE Top 25 (Key Weaknesses)**
* **CWE-79/89:** XSS and SQL Injection prevention.
* **CWE-22/434:** Path Traversal and Unrestricted File Upload protection.
* **CWE-502/918:** Insecure Deserialization and SSRF mitigation.

#### **ASVS Level 2 Verification Checklist (Detailed)**
* **V1 (Architecture):** All components must be identified and their trust boundaries defined. Threat modeling is mandatory for all new features.
* **V2 (Authentication):** Enforce multi-factor authentication (where applicable), password complexity, and secure credential storage (Argon2/bcrypt).
* **V3 (Session Management):** Tokens must be generated using cryptographically strong random numbers. Logout must invalidate sessions globally.
* **V4 (Access Control):** Enforce "Principle of Least Privilege." All requests must be checked for authorization at the Service Layer (prevent IDOR).
* **V5 (Input Validation):** All input must be validated against a strict "allow-list." Input must be encoded before being processed by the interpreter (SQL/HTML).
* **V7 (Cryptography):** All secrets must be stored in **Google Secret Manager**. Use only industry-standard algorithms (AES-256, RSA-4096).
* **V8 (Error Handling & Logging):** Log all security-relevant events (failed logins, high-value transactions) with timestamps and user IDs. **Zero PII in logs.**
* **V12 (File/Resources):** Files must be stored outside the web root. Validate file types by magic numbers, not extensions.

---

## 3. BRANDING & VISUAL IDENTITY (MAHINDRA STANDARDS)

Compliance with `.ai/ui-brand.skills.md` is mandatory:
* **Primary Color:** Rising Red (#E31837).
* **Secondary Color:** Steel Grey (#4D4D4F).
* **Typography:** Headlines MUST use **QuantumRise GX**.
* **Logo:** The Mahindra Red Logo must be present in the primary AppBar.

---

## 4. PERFORMANCE & SCALABILITY (BY DEFAULT)
* **Pagination:** Mandatory for all list-based APIs.
* **Caching:** GET endpoints must support **ETag** and **304 Not Modified**.
* **Compression:** Enable **Gzip** or **Brotli** for all responses.
* **Naming:** Entities (Singular), API Paths (Plural).
---

## 5. REQUIREMENT TRACEABILITY
No artifact exists in isolation. You must maintain this chain:
**BRD (FR-ID)** $\rightarrow$ **LLD (API/DTO)** $\rightarrow$ **Code (Service/Entity)** $\rightarrow$ **QA (Test Case)**.

---