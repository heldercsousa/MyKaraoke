# ⚡ Quick Reference Guide

**Common Commands & Workflows for AI Development**

---

## 🐍 Python Commands

### Virtual Environment
```bash
# Create
python -m venv myvocalist-ai

# Activate
source myvocalist-ai/bin/activate  # Linux/Mac
myvocalist-ai\Scripts\activate     # Windows

# Deactivate
deactivate

# Delete (just remove folder)
rm -rf myvocalist-ai
```

### Package Management
```bash
# Install package
pip install pandas

# Install from requirements.txt
pip install -r requirements.txt

# Save current packages
pip freeze > requirements.txt

# Update package
pip install --upgrade pandas

# List installed
pip list
```

### Running Python
```bash
# Run script
python script.py

# Python REPL (interactive)
python

# Jupyter notebook
jupyter notebook

# Run with debugger
python -m pdb script.py
```

---

## 🚀 FastAPI Commands

### Development
```bash
# Run with auto-reload
uvicorn main:app --reload

# Run on specific port
uvicorn main:app --port 8000

# Run with different host
uvicorn main:app --host 0.0.0.0

# View logs
uvicorn main:app --log-level debug
```

### Testing
```bash
# Access API docs
http://localhost:8000/docs

# Test endpoint with curl
curl -X POST http://localhost:8000/recommend \
  -H "Content-Type: application/json" \
  -d '{"song_title": "Imagine"}'

# Test with httpie (install: pip install httpie)
http POST localhost:8000/recommend song_title="Imagine"
```

---

## ☁️ AWS SAM Commands

### Local Development
```bash
# Build Lambda package
sam build

# Test locally
sam local invoke FunctionName

# Start local API
sam local start-api

# Test with event file
sam local invoke --event events/test.json
```

### Deployment
```bash
# First deployment (guided)
sam deploy --guided

# Subsequent deployments
sam deploy

# Deploy specific stack
sam deploy --stack-name my-api

# Deploy to different region
sam deploy --region us-east-1
```

### Monitoring
```bash
# View logs (tail)
sam logs -n FunctionName --tail

# View logs (specific time)
sam logs -n FunctionName --start-time '10min ago'

# Invoke deployed function
sam remote invoke FunctionName

# Delete stack
sam delete
```

---

## 📦 ngrok Commands

### Basic Usage
```bash
# Expose local port
ngrok http 8000

# With custom subdomain (paid)
ngrok http -subdomain=myvocalist 8000

# With authentication
ngrok http 8000 -auth="user:pass"

# View web interface
http://localhost:4040
```

---

## 🔧 Git Commands (for tracking your work)

### Daily Workflow
```bash
# Check status
git status

# Stage changes
git add .

# Commit
git commit -m "Add recommendation API"

# Push to GitHub
git push origin main

# Pull latest
git pull
```

### Branching
```bash
# Create branch
git checkout -b feature/pitch-analysis

# Switch branch
git checkout main

# Merge branch
git merge feature/pitch-analysis

# Delete branch
git branch -d feature/pitch-analysis
```

---

## 💻 VS Code Shortcuts

### Essential
```
Ctrl/Cmd + P        - Quick file open
Ctrl/Cmd + Shift + P - Command palette
Ctrl/Cmd + `        - Toggle terminal
Ctrl/Cmd + /        - Comment line
F5                  - Start debugging
Ctrl/Cmd + Space    - Trigger autocomplete
```

### Python Specific
```
Shift + Enter       - Run line in Python REPL
F12                 - Go to definition
Shift + F12         - Find all references
Ctrl/Cmd + .        - Quick fix
```

---

## 🐛 Debugging Workflows

### Python Debugging
```python
# Basic print debugging
print(f"Value: {variable}")

# Better debugging
import pdb; pdb.set_trace()  # Breakpoint

# Or use VS Code debugger
# Click left of line number to add breakpoint
# Press F5 to start
```

### FastAPI Debugging
```python
# Add logging
import logging
logger = logging.getLogger(__name__)

@app.post("/endpoint")
async def endpoint():
    logger.info("Endpoint called")
    logger.debug(f"Data: {data}")
```

### Lambda Debugging
```bash
# View CloudWatch logs
aws logs tail /aws/lambda/FunctionName --follow

# Or use SAM
sam logs -n FunctionName --tail

# Test locally with debugger
sam local invoke -d 5858 FunctionName
```

---

## 📊 Common Data Operations

### Pandas
```python
import pandas as pd

# Load CSV
df = pd.read_csv('file.csv')

# Filter
df[df['column'] > 100]

# Group by
df.groupby('genre')['bpm'].mean()

# Sort
df.sort_values('bpm', ascending=False)

# Save
df.to_csv('output.csv', index=False)
```

### NumPy
```python
import numpy as np

# Create array
arr = np.array([1, 2, 3])

# Statistics
arr.mean()
arr.std()
arr.min()
arr.max()

# Math operations
arr * 2
arr + 10
np.sqrt(arr)
```

---

## 🧪 Testing Commands

### pytest
```bash
# Run all tests
pytest

# Run specific file
pytest test_recommender.py

# Run with coverage
pytest --cov=mymodule

# Run with output
pytest -v -s
```

### MAUI Testing
```bash
# Run from command line
dotnet test

# Run specific test
dotnet test --filter FullyQualifiedName~TestName
```

---

## 🌐 HTTP Status Codes (Quick Reference)

```
200 OK              - Success
201 Created         - Resource created
400 Bad Request     - Invalid input
401 Unauthorized    - Not authenticated
403 Forbidden       - Not authorized
404 Not Found       - Resource doesn't exist
500 Server Error    - Something broke
503 Service Unavailable - Service down
```

---

## 📝 Markdown Syntax

```markdown
# H1
## H2
### H3

**bold**
*italic*
`code`

- Bullet list
1. Numbered list

[Link](url)
![Image](url)

```code block```
```

---

## 💰 Cost Checking

### AWS
```bash
# Check current month costs
aws ce get-cost-and-usage \
  --time-period Start=2024-12-01,End=2024-12-31 \
  --granularity MONTHLY \
  --metrics "UnblendedCost"

# Check Lambda invocations
aws cloudwatch get-metric-statistics \
  --namespace AWS/Lambda \
  --metric-name Invocations \
  --dimensions Name=FunctionName,Value=MyFunction \
  --start-time 2024-12-01T00:00:00Z \
  --end-time 2024-12-31T23:59:59Z \
  --period 86400 \
  --statistics Sum
```

### Quick Check
- AWS Console > Billing Dashboard
- Look for "Month-to-date costs"
- Check "Free tier usage"

---

## 🔑 Environment Variables

### Set Temporarily
```bash
# Linux/Mac
export API_KEY="your-key"

# Windows (CMD)
set API_KEY=your-key

# Windows (PowerShell)
$env:API_KEY="your-key"
```

### Set Permanently
```bash
# Linux/Mac: Add to ~/.bashrc or ~/.zshrc
export API_KEY="your-key"

# Windows: Use System Properties > Environment Variables
```

### Use in Python
```python
import os
api_key = os.environ.get('API_KEY')
```

### Use in .env file
```bash
# Create .env file
API_KEY=your-key
DB_PATH=./data.db

# Load with python-dotenv
pip install python-dotenv
```

```python
from dotenv import load_dotenv
import os

load_dotenv()
api_key = os.environ['API_KEY']
```

---

## 🚨 Emergency Commands

### Kill Process on Port
```bash
# Linux/Mac
lsof -ti:8000 | xargs kill -9

# Windows
netstat -ano | findstr :8000
taskkill /PID [PID] /F
```

### Reset Python Environment
```bash
# Deactivate and delete
deactivate
rm -rf myvocalist-ai

# Recreate
python -m venv myvocalist-ai
source myvocalist-ai/bin/activate
pip install -r requirements.txt
```

### Clear pip cache
```bash
pip cache purge
```

### Fix MAUI Hot Reload Issues
```bash
# Clean and rebuild
dotnet clean
dotnet build

# Delete bin/obj folders
rm -rf bin obj
```

---

## 📚 Documentation Links

### Python
- Official Docs: https://docs.python.org/3/
- Pandas: https://pandas.pydata.org/docs/
- NumPy: https://numpy.org/doc/
- scikit-learn: https://scikit-learn.org/stable/

### FastAPI
- Docs: https://fastapi.tiangolo.com/
- Tutorial: https://fastapi.tiangolo.com/tutorial/

### AWS
- Lambda: https://docs.aws.amazon.com/lambda/
- SAM: https://docs.aws.amazon.com/serverless-application-model/

### MAUI
- Docs: https://learn.microsoft.com/en-us/dotnet/maui/

---

## 💡 Pro Tips

### Pandas
```python
# View first/last rows
df.head()
df.tail()

# Column info
df.info()
df.describe()

# Check for nulls
df.isnull().sum()

# Drop duplicates
df.drop_duplicates()
```

### FastAPI
```python
# Add CORS
from fastapi.middleware.cors import CORSMiddleware
app.add_middleware(CORSMiddleware, allow_origins=["*"])

# Validate with Pydantic
from pydantic import BaseModel, Field
class Song(BaseModel):
    title: str = Field(..., min_length=1)
    bpm: int = Field(..., gt=0, lt=300)
```

### AWS Lambda
```python
# Keep function warm
import json

def lambda_handler(event, context):
    # Check if warming ping
    if event.get('source') == 'aws.events':
        return {'statusCode': 200, 'body': 'warm'}
    
    # Normal processing
    ...
```

---

## 🎯 Troubleshooting Checklist

**API not responding?**
- [ ] Check if process is running
- [ ] Verify port is correct
- [ ] Check firewall settings
- [ ] Look at logs for errors

**Python import error?**
- [ ] Virtual environment activated?
- [ ] Package installed? (`pip list`)
- [ ] Correct Python version? (`python --version`)
- [ ] Working directory correct? (`pwd`)

**AWS deployment failed?**
- [ ] AWS credentials configured? (`aws sts get-caller-identity`)
- [ ] Correct region?
- [ ] IAM permissions sufficient?
- [ ] Check CloudFormation console for error

**MAUI app crashing?**
- [ ] Clean and rebuild
- [ ] Check Android emulator is running
- [ ] Look at Debug console
- [ ] Verify NuGet packages restored

---

**Keep this file open while coding! 📌**

**Pro tip:** Print common commands and stick next to your monitor!
