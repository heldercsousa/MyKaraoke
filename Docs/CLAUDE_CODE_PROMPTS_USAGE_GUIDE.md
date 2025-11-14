# Claude Code Prompts - Usage Guide

## 📋 Overview

You now have **3 specialized prompts** ready to use with Claude Code for implementing critical parts of Option B.

---

## 📦 Prompt Files Created

### **1. PROMPT_1_NavBarBehavior_GetMD3Color.md** (6.1 KB)
**Purpose:** Update NavBarBehavior.cs to use GetMD3Color() helper method  
**What it does:** Replaces 2 hardcoded color instances with semantic token lookups  
**Time:** 10-15 minutes  
**Priority:** High (Phase 6 of Option B)

### **2. PROMPT_2_1_Create_Test_Project.md** (12 KB)
**Purpose:** Set up comprehensive unit test infrastructure  
**What it does:** Creates test project, folder structure, and helper utilities  
**Time:** 30-45 minutes  
**Priority:** Medium (Can be done anytime)

### **3. PROMPT_2_2_MD3_Compliance_Tests.md** (20 KB)
**Purpose:** Write 24 unit tests for MD3 color system compliance  
**What it does:** Validates HCT palettes, WCAG contrast, semantic tokens  
**Time:** 45-60 minutes  
**Priority:** Medium (After test project created)

---

## 🚀 How to Use These Prompts

### **Method 1: Copy-Paste to Claude Code** (Recommended)

**Step 1: Open Claude Code**
```bash
cd /path/to/MyVocaList
claude-code
```

**Step 2: Open Prompt File**
- Open the `.md` file in your text editor
- Read the entire prompt (understand what it will do)

**Step 3: Copy and Paste**
- Copy the **entire content** of the `.md` file
- Paste it into Claude Code chat
- Press Enter

**Step 4: Review and Apply**
- Claude Code will analyze the task
- Review the proposed changes
- Approve the implementation

---

### **Method 2: Direct File Reference**

If Claude Code can access local files:

```
Claude Code, please read and implement the task described in:
/path/to/PROMPT_1_NavBarBehavior_GetMD3Color.md
```

---

## 📊 Execution Order

### **Phase 1: Fix Navigation Behavior** (Priority 1)

**Use:** PROMPT_1_NavBarBehavior_GetMD3Color.md

**When:** During Option B Phase 6 (after MaterialColors.xaml is replaced)

**Expected Output:**
- NavBarBehavior.cs updated
- 2 hardcoded colors replaced
- Build succeeds
- Nav bar uses Primary color from resources

**Verification:**
```bash
dotnet build -f net8.0-android
dotnet run -f net8.0-android
# Check: Nav bar border and separator are purple #7F41AC
```

---

### **Phase 2: Create Test Infrastructure** (Priority 2)

**Use:** PROMPT_2_1_Create_Test_Project.md

**When:** Anytime (can be parallel with implementation, or after)

**Expected Output:**
- MyVocaList.Tests project created
- xUnit + FluentAssertions + MaterialColorUtilities installed
- Folder structure created (View/ColorSystem, Services, etc.)
- ColorTestHelpers.cs utility class created
- Sample test passes

**Verification:**
```bash
dotnet test MyVocaList.Tests/MyVocaList.Tests.csproj
# Expected: 1 test passed (SampleTest)
```

---

### **Phase 3: Write Compliance Tests** (Priority 3)

**Use:** PROMPT_2_2_MD3_Compliance_Tests.md

**When:** After test project is created

**Expected Output:**
- MD3ComplianceTests.cs created with 24 tests
- All tests compile
- All 24 tests pass

**Verification:**
```bash
dotnet test MyVocaList.Tests/MyVocaList.Tests.csproj
# Expected: 24/24 tests passed
```

---

## ✅ Success Criteria

### After PROMPT_1:
- [ ] NavBarBehavior.cs uses GetMD3Color() (2 locations)
- [ ] No hardcoded colors in NavBarBehavior.cs
- [ ] Build succeeds
- [ ] Nav bar displays correct colors

### After PROMPT_2.1:
- [ ] MyVocaList.Tests project exists
- [ ] All packages installed
- [ ] Folder structure created
- [ ] ColorTestHelpers.cs exists
- [ ] 1 sample test passes

### After PROMPT_2.2:
- [ ] MD3ComplianceTests.cs exists with 24 tests
- [ ] All tests compile
- [ ] All 24 tests pass
- [ ] MD3 compliance verified programmatically

---

## 🎯 Integration with Option B

These prompts fit into **Option B** implementation as follows:

```
Option B Timeline:
├── Phase 1-5: Setup, Generator, Replace, Build (4 hours)
├── Phase 6: Fix Nav Components (30 min) ← USE PROMPT_1 HERE
├── Phase 7: Testing (1 hour)
├── Phase 8: Verification (30 min) ← USE PROMPTS 2.1 & 2.2 HERE
├── Phase 9: Documentation (30 min)
└── Phase 10: Final Checks (30 min)
```

**Alternative:** Run Prompts 2.1 and 2.2 anytime to establish automated testing early.

---

## 📝 Detailed Usage Examples

### **Example 1: Using PROMPT_1**

```bash
# 1. Open Claude Code
cd MyVocaList
claude-code

# 2. In Claude Code chat, paste:
```

```
[Paste entire content of PROMPT_1_NavBarBehavior_GetMD3Color.md here]
```

```
# 3. Claude Code will respond with:
"I'll update NavBarBehavior.cs to use GetMD3Color()..."
[Shows proposed changes]

# 4. Review changes, then say:
"Apply these changes"

# 5. Verify:
dotnet build -f net8.0-android
```

---

### **Example 2: Using PROMPT_2.1**

```bash
# 1. In Claude Code chat, paste:
```

```
[Paste entire content of PROMPT_2_1_Create_Test_Project.md here]
```

```
# 2. Claude Code will:
- Create MyVocaList.Tests project
- Install packages
- Create folder structure
- Create ColorTestHelpers.cs
- Create sample test

# 3. Verify:
dotnet test MyVocaList.Tests/MyVocaList.Tests.csproj
# Expected: 1 test passed
```

---

### **Example 3: Using PROMPT_2.2**

```bash
# Prerequisites: PROMPT_2.1 completed

# 1. In Claude Code chat, paste:
```

```
[Paste entire content of PROMPT_2_2_MD3_Compliance_Tests.md here]
```

```
# 2. Claude Code will:
- Create MD3ComplianceTests.cs
- Write 24 unit tests
- All tests should compile and pass

# 3. Verify:
dotnet test --filter "FullyQualifiedName~MD3ComplianceTests"
# Expected: 24/24 tests passed
```

---

## 🔍 What Each Prompt Contains

### **PROMPT_1 Structure:**
- Context explanation
- Current helper method code
- 2 specific changes needed (with line numbers)
- Before/after code examples
- Verification steps
- Commit message template

### **PROMPT_2.1 Structure:**
- Solution structure context
- Step-by-step project creation
- Package installations
- Folder structure specification
- Helper class code (ColorTestHelpers.cs)
- Sample test for verification
- Success criteria

### **PROMPT_2.2 Structure:**
- Test context and purpose
- Complete test file (MD3ComplianceTests.cs)
- 24 tests organized in 6 categories
- Expected results for each test
- Running instructions
- Success criteria

---

## 💡 Pro Tips

### **For PROMPT_1:**
- ✅ Simple, focused change
- ✅ Quick to implement (10-15 min)
- ✅ Immediate visual verification
- ⚠️ Make sure GetMD3Color() method already exists

### **For PROMPT_2.1:**
- ✅ Sets up testing infrastructure properly
- ✅ Reusable for all future tests
- ✅ Professional test project structure
- ⚠️ May need to adjust project paths if solution structure differs

### **For PROMPT_2.2:**
- ✅ Comprehensive test coverage
- ✅ All tests should pass if HCT palettes generated correctly
- ✅ Great for CI/CD integration
- ⚠️ Requires MaterialColorUtilities package
- ⚠️ If tests fail, review actual vs expected HCT values

---

## 🐛 Troubleshooting

### **PROMPT_1 Issues:**

**Issue:** GetMD3Color() method not found
**Solution:** Check if method was added to NavBarBehavior.cs. Refer to NavBarBehavior_MD3_Fix_Guide.md

**Issue:** Colors don't change after update
**Solution:** 
```bash
dotnet clean
dotnet build -f net8.0-android
# Restart emulator or reinstall app
```

---

### **PROMPT_2.1 Issues:**

**Issue:** Package restore fails
**Solution:** 
```bash
dotnet clean
dotnet restore
dotnet build
```

**Issue:** Project paths incorrect
**Solution:** Adjust project reference paths in prompt based on your actual solution structure

---

### **PROMPT_2.2 Issues:**

**Issue:** Tests fail with unexpected HCT values
**Solution:** This is normal if using different seed colors. Update expected ranges in tests.

**Issue:** WCAG contrast tests fail
**Solution:** Verify your tonal palette was generated correctly. HCT algorithm guarantees compliance.

**Issue:** Tests compile but show errors
**Solution:** Ensure ColorTestHelpers.cs was created by PROMPT_2.1 first.

---

## 📊 Prompt Complexity

| Prompt | Lines of Code | Complexity | Time | Dependencies |
|--------|---------------|------------|------|--------------|
| PROMPT_1 | ~50 lines | Low | 10-15 min | GetMD3Color() exists |
| PROMPT_2.1 | ~200 lines | Medium | 30-45 min | dotnet CLI |
| PROMPT_2.2 | ~400 lines | Medium-High | 45-60 min | PROMPT_2.1 done |

---

## 🎓 Learning Value

### **PROMPT_1 teaches:**
- Resource dictionary access in MAUI
- Fallback pattern for missing resources
- Semantic color token usage

### **PROMPT_2.1 teaches:**
- Professional test project structure
- xUnit + FluentAssertions setup
- Test helper utility patterns
- Cross-project testing strategy

### **PROMPT_2.2 teaches:**
- HCT color space validation
- WCAG contrast calculation
- Material Design 3 compliance testing
- Test-driven color system development

---

## 📞 When to Ask for Help

**Ask Helder/Team if:**
- Tests reveal actual MD3 non-compliance issues
- HCT values significantly different from expected
- Need to adjust seed colors based on test results

**Ask Claude Code if:**
- Syntax errors in generated code
- Package installation issues
- Path resolution problems

**Ask Claude (me) if:**
- Need to modify prompt requirements
- Want additional test scenarios
- Need explanation of HCT concepts

---

## ✅ Final Checklist

Before using prompts:
- [ ] Downloaded all 3 prompt files
- [ ] Read each prompt completely
- [ ] Understand what each prompt does
- [ ] Have Claude Code access to project

After using PROMPT_1:
- [ ] NavBarBehavior.cs updated
- [ ] Build succeeds
- [ ] Nav bar displays correctly
- [ ] Git commit created

After using PROMPT_2.1:
- [ ] Test project created
- [ ] Sample test passes
- [ ] Folder structure in place
- [ ] Git commit created

After using PROMPT_2.2:
- [ ] MD3ComplianceTests.cs created
- [ ] 24/24 tests pass
- [ ] Compliance verified
- [ ] Git commit created

---

## 🎉 Success State

**When all 3 prompts are completed:**

✅ NavBarBehavior uses semantic colors  
✅ Zero hardcoded colors remain  
✅ Professional test infrastructure exists  
✅ 24 automated compliance tests pass  
✅ MD3 compliance programmatically verified  
✅ Foundation for future testing established  

**You'll have:**
- Production-ready navigation component
- Automated quality gates
- Confidence in MD3 compliance
- Test infrastructure for future features

---

## 📚 Additional Resources

**After completing these prompts, consider:**

1. Adding more tests for other components
2. Integrating tests into CI/CD pipeline
3. Adding code coverage reporting
4. Writing integration tests for XAML resources
5. Performance testing for color lookups

---

**Ready to use the prompts? Start with PROMPT_1 during Option B Phase 6!** 🚀
