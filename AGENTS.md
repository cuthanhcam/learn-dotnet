# Learn .NET agent guide

This repository is a progressive learning path, not one production application. Keep examples small, runnable, and aligned with the topic and difficulty of their numbered section.

- Preserve the learning order from C# basics through OOP, core .NET, performance, and DSA.
- Explain non-obvious concepts near the example and favor clarity over premature abstraction.
- Use the SDK selected by `global.json` and repository-wide `Directory.Build.*` rules.
- Do not silently modernize an example when its purpose is to demonstrate an older or contrasting approach; label trade-offs instead.
- Add focused tests or observable sample output where they improve the lesson.

Build and test the smallest affected project first. Do not edit `bin`, `obj`, or temporary material unless explicitly requested.
