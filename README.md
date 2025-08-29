# AI for special Education Planning (AIEP) ✨  
_Public knowledge and special education expertise combined with AI to design effective, tailored learning paths for every child._

🌐 **Website:** [https://www.aieduflow.com/)

<p align="center">
  <img alt="AIEP banner" src="https://img.shields.io/badge/AIEP-IEP%20Assistant-6C63FF?style=for-the-badge">&nbsp;
  <img alt="Build" src="https://img.shields.io/badge/build-passing-brightgreen?style=for-the-badge">&nbsp;
  <img alt="License" src="https://img.shields.io/badge/license-Apache--2.0-blue?style=for-the-badge">&nbsp;
  <img alt="WCAG" src="https://img.shields.io/badge/WCAG-2.2%20AA-0A7C86?style=for-the-badge">&nbsp;
  <img alt="Privacy by design" src="https://img.shields.io/badge/Privacy-by%20design-222?style=for-the-badge">
</p>

<p align="center">
  <b>Mission:</b> Help schools and families craft effective, equitable, and legally compliant IEPs with AI and anonymized data.  
  <br/>
  <i>“Faster drafting, higher acceptance, smaller gaps.”</i>
</p>

---

## ⛳ Quick Navigation
- [🌍 Mission & Problem](#-mission--problem)  
- [🎯 Objectives (SMART)](#-objectives-smart)  
- [👥 Scope & Users](#-scope--users)  
- [🔐 Data & Privacy](#-data--privacy)  
- [🧠 System Overview](#-system-overview--workflow)  
- [📚 Content Libraries](#-content-libraries)  
- [🤖 Model Design & Guardrails](#-model-design--guardrails)  
- [🔌 Integrations & Interoperability](#-integrations--interoperability)  
- [🧩 UX & Accessibility](#-ux--accessibility)  
- [🚀 Quick Start](#-quick-start)  
- [🖼️ Screens & Charts](#-screens--charts)  
- [🗺️ Roadmap](#-roadmap)  
- [🤝 Contributing](#-contributing)  
- [📜 License](#-license)  

---

## 🌍 Mission & Problem
**Mission:** Help schools and families craft effective, equitable, and legally compliant IEPs with AI and anonymized data.  
**Problem:** Teams spend excessive time searching for goals, services, and accommodations; quality varies across sites.  

**Target Outcomes:**  
- ⏱️ Drafting time ↓ 40%  
- ✅ Top-3 acceptance ≥ 80%  
- ⚖️ Parity gaps ≤ 5%  
- 📜 IDEA / 504 aligned  
- 🌎 Inclusive learning practices  

---

## 🎯 Objectives (SMART)
- ≥ 80% educator acceptance of top-3 recommendations within 6 months  
- ≥ 40% drafting time reduction within one school year  
- ≤ 5% subgroup parity gaps  
- Full IDEA/FAPE/504/ADA alignment  
- +15% goal progress vs. baseline in 12 months  

---

## 👥 Scope & Users
**Students:** IDEA (birth–21) + Section 504 plans. Education-focused (no medical diagnosis).  

**Primary Users:**  
- Special educators  
- School psychologists  
- Therapists  
- IEP coordinators  
- Administrators  

**Secondary Users:**  
- Parents/guardians  
- General educators  
- Related service providers  

---

## 🔐 Data & Privacy
- **Sources:** prior IEPs/504s, evaluations, progress, services, outcomes  
- **Anonymization:** identifiers removed; k-anonymity; low-count suppression; pseudonymous linkage  
- **Governance:** FERPA baseline; HIPAA segregation if PHI appears  
- **Security:** encryption, least-privilege IAM, audit logs, penetration testing  
- **AI Governance:** AI + governance by design  

---

## 🧠 System Overview & Workflow
```mermaid
flowchart LR
    A[⚡ Intake: Questionnaire + NLP] --> B[🤖 Profile: Needs & Strengths]
    B --> C[🔒 Matching: Recommender + Constraints]
    C --> D[📡 Draft: SMART goals, accommodations, services]
    D --> E[🧰 Review: Human-in-the-loop + Rationale]
    E --> F[📈 Progress: Continuous improvement loop]
```

---

## 🌐 Azure Application Architecture

```mermaid
flowchart TB
  %% Client Layer
  subgraph CL[Client Layer]
    U1["🖥️ Web Browser"]
    U2["📱 Mobile App"]
  end

  %% Edge and Security
  subgraph EG[Edge and Security]
    AFD["🌐 Azure Front Door"]
    WAF["🛡️ Application Gateway + WAF"]
    APIM["🔌 Azure API Management"]
    ENTRA["👤 Microsoft Entra ID (Azure AD)"]
  end

  %% Application Layer
  subgraph APP[Application Layer]
    APP1["⚙️ App Service (Web)"]
    FUNC["⚡ Azure Functions"]
    AKS["🐳 AKS / Container Apps"]
    REDIS["🧰 Azure Cache for Redis"]
    BUS["📩 Service Bus"]
    EH["📡 Event Hubs"]
    COG["🤖 Azure OpenAI / Cognitive Services"]
  end

  %% Data Layer
  subgraph DATA[Data Layer]
    COSMOS["🪐 Cosmos DB"]
    SQL["🗄️ Azure SQL Database"]
    STORAGE["🗂️ Blob Storage"]
    FS["📁 File Storage"]
    SYN["📊 Synapse / Data Lake"]
  end

  %% Security and Operations
  subgraph SECOPS[Security and Operations]
    KV["🔑 Key Vault"]
    VNET["🌐 Virtual Network"]
    PE["🔒 Private Endpoints"]
    MON["📈 Monitor + Log Analytics + App Insights"]
    DEFENDER["🛡️ Defender for Cloud"]
    BAK["💾 Backup & Recovery Vault"]
  end

  %% Edges
  U1 -->|HTTPS| AFD
  U2 -->|HTTPS| AFD
  AFD --> WAF --> APIM

  APIM -->|REST/GraphQL| APP1
  APIM --> FUNC
  APIM --> AKS

  APP1 --> REDIS
  FUNC --> BUS
  AKS --> EH

  APP1 --> COSMOS
  APP1 --> SQL
  APP1 --> STORAGE
  FUNC --> STORAGE
  AKS --> COSMOS
  AKS --> SQL

  BUS --> FUNC
  EH --> FUNC

  FUNC --> COG
  APP1 --> COG

  COSMOS --- SYN
  SQL --- SYN
  STORAGE --- SYN

  %% Security bindings
  KV --- APP1
  KV --- FUNC
  KV --- AKS
  VNET --- PE
  PE --- COSMOS
  PE --- SQL
  PE --- STORAGE

  MON --- APP1
  MON --- FUNC
  MON --- AKS
  MON --- APIM
  MON --- AFD

  DEFENDER --- SECOPS
  BAK --- DATA

  ENTRA --> APIM
  ENTRA --> APP1
  ENTRA --> AKS


```

---

## 👩‍🏫 IEP Planning Workflow

```mermaid
flowchart TB
  subgraph PARENT[👨‍👩‍👧 Parent / Guardian]
    P1["📝 Provide concerns & consent"]
    P2["📄 Review plain-language draft"]
    P3["✅ e-Consent & sign"]
  end

  subgraph EDUCATOR[👩‍🏫 Special Educator]
    T1["📋 Intake questionnaire"]
    T2["🔍 Review student profile"]
    T3["🎯 Select SMART goals"]
    T4["🛠️ Choose accommodations/services"]
    T5["✍️ Draft IEP & rationale"]
  end

  subgraph PROS[🧑‍⚕️ Psychologist & Therapists]
    S1["🧪 Assessments & evaluations"]
    S2["💊 Service dosage recommendations"]
    S3["📊 Progress probes & baselines"]
  end

  subgraph AIEP_SYS[🤖 AIEP System]
    A1["🧾 NLP: extract needs/strengths"]
    A2["⚖️ Hybrid recommender + constraints"]
    A3["❓ Explainability: 'why this?'"]
    A4["📏 Bias checks & parity guardrails"]
    A5["📤 Export to district template"]
  end

  P1 --> T1
  T1 --> A1 --> A2 --> T2
  S1 --> A1
  T2 --> T3 --> T4 --> T5
  A2 --> A3 --> T3
  S2 --> T4
  S3 --> T3
  T5 --> A5 --> P2 --> P3
```


## 📚 Content Libraries
- SMART Goal Bank with mastery criteria & probes  
- Accommodations & Modifications (classroom/testing/AT)  
- Interventions & Services with evidence + dosage ranges  
- Specialist roles, referrals, collaboration patterns  

---

## 🤖 Model Design & Guardrails
- NLP extraction with legal-phrase safeguards  
- Hybrid recommender; uncertainty & explanations  
- Fairness: subgroup monitoring, re-ranking constraints, bias audits  
- Safety: human-in-the-loop; transparent “why this?”  

---

## 🔌 Integrations & Interoperability
- Connects to SIS & IEP platforms; import/export;  
- SSO (SAML/OIDC)  
- Webhooks; optional therapy vendor feeds (segregated if PHI)  
- REST/GraphQL APIs  

---

## 🧩 UX & Accessibility
- **Team workspace:** side-by-side profile, recommendations, legal blocks; export to district templates  
- **Parent portal:** plain language, multilingual, e-consent, progress dashboards  
- **Accessibility:** WCAG 2.2 AA; keyboard & screen reader; dyslexia-friendly typography option  

---

## 🚀 Quick Start
```bash
# Clone the repo
git clone https://github.com/your-org/aiep.git
cd aiep

# Install dependencies
npm install

# Run development server
npm run dev
```

---

## 🗺️ Roadmap
- [x] Core recommender system  
- [x] Content library integration  
- [x] Parent portal (Q4 2025)  
- [x] Multilingual support (Q4 2025)  
- [x] SIS vendor integrations  

---

## 🤝 Contributing
Contributions are welcome! Please see [CONTRIBUTING.md](CONTRIBUTING.md).  
We especially encourage educators, parents, and accessibility experts to join.  

---

## 📜 License
Apache-2.0 © 2025 AIEP Project Team
