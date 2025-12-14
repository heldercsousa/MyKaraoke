# 🚀 MyVocaList Feature Roadmap (AI Engineering)

This document catalogs all the high-value features discussed, separated by focus areas (Efficiency, Performance, and Social), serving as a foundation for the next planning and implementation steps of your journey as an AI Engineer.

---

### I. 🎤 Value for Singer Performance (Focus: DSP and Audio)

Features that transform the app into a vocal improvement tool, increasing value for the audience focused on enhancement.

| Feature | Description | AI Engineering Focus |
| :--- | :--- | :--- |
| **1. AI Score (Auto-Score)** | Real-time or post-recording voice analysis (via microphone) to determine pitch accuracy and timing precision, generating an objective score. | **DSP** (Digital Signal Processing), Classification/Regression Models. |
| **2. Critiques and Improvement Suggestions** | **LLM (Gemini) applied to DSP:** Translate numerical pitch error data into useful and motivational textual *feedback*. | **Natural Language Generation (NLP),** LLM Integration. |
| **3. Training Mode (Note Visualization)** | Real-time note detection to display the reference melody against the singer's voice on a graph, allowing for guided vocal practice. | **DSP** (Pitch Detection/FFT), Optimized Frontend Engineering (MAUI). |
| **4. Lyrics Detection and Synchronization** | Fetch lyrics via internal catalog or API and synchronize each word with exact *timestamps* for the progressive "color passing" effect. | **Content and Timing Engineering,** API Integration. |
| **5. Integrated Audio Separation** | Use APIs (like Moises) to remove vocals from original tracks on demand, allowing musicians to create personalized *backing tracks* (useful in Bandokê mode). | **Backend Engineering**, Audio Processing API Integration. |

---

### II. 🧠 Value for Host Efficiency (Focus: Prediction and UX)

Features that use AI and automation to optimize the event organizer's experience.

| Feature | Description | AI Engineering Focus |
| :--- | :--- | :--- |
| **6. Smart Wait Time Estimation** | **Regression Prediction:** Estimate the wait time for the next singer or the round duration, based on historical data and the number of pending participants. | **Regression Modeling** (Time Series Forecasting), MLOps. |
| **7. Voice Command Management** | Allow the Host to perform critical queue actions (e.g., "Add Marcos", "Next singer") using only voice commands. | **Natural Language Processing** (ASR - Automatic Speech Recognition), UX/Workflow Optimization. |
| **8. Smart Next Song Suggestion** | Suggest the next song or genre to be sung, avoiding repetitions or ensuring optimal variety to keep the crowd's energy up. | **Recommender Systems** (Content-Based Filtering), Data Analysis. |

---

### III. 🧑‍🤝‍🧑 Value for Social Engagement (Focus: LLM and Community)

Features that fulfill the future social network goal, creating a community around the application.

| Feature | Description | AI Engineering Focus |
| :--- | :--- | :--- |
| **9. Karaoke Persona Generation (LLM)** | The singer enters *tags* (e.g., genre, mood), and the **Gemini API** generates a unique and engaging biography or "Karaoke Persona" for the profile. | **LLM (Gemini API) Integration,** Prompt Engineering. |
| **10. Audience Voting and Engagement** | Real-time voting system allowing the audience to interact with the singer and the performance (e.g., 'Applause', 'Fire'). | **Backend Engineering** (WebSockets/SignalR), Social User Experience (UX). |
| **11. Score Sheet/Chords for Bands** | In Bandokê mode, automatically generate or transpose a digital chord sheet or score for band musicians, based on the chosen song and key. | **Content Automation Engineering,** Musical Data Processing. |

---

### IV. 🛠️ Value for Development Team (Focus: AIA-D)

Features ensuring development is efficient, cost-effective, and of high code quality.

| Feature | Description | AI Engineering Focus |
| :--- | :--- | :--- |
| **12. MD3 Compliance/Architecture Agents** | Use the Gemini 3 Pro Agent (Jules) to ensure all screens are built following 100% MD3 guidelines and the .NET MAUI architecture from `CLAUDE.md`. | **MLOps for Code,** Agent Autonomy (Jules/Antigravity). |
| **13. Token Optimization** | Intelligent management of agent context to minimize token consumption and the cost of AI-assisted development. | **Cost/Resource Efficiency,** Prompt Engineering. |
